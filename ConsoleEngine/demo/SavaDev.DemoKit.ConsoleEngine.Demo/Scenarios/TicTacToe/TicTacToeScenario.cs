namespace SavaDev.DemoKit.ConsoleEngine.Demo.Scenarios.TicTacToe;

public class TicTacToeScenario : IConsoleDemoScenario
{
    /// <summary>
    /// Creates new Tic-Tac-Toe scenario.
    /// </summary>
    /// <param name="rules">Game rules.</param>
    /// <param name="boardOptions">Board drawing options.</param>
    /// <param name="name"><inheritdoc cref="IConsoleDemoScenario.Name"/>.</param>
    /// <exception cref="ArgumentException"></exception>
    public TicTacToeScenario(TicTacToeRules rules, BoardOptions boardOptions, string name = "TicTacToe") {
        if (rules.Width < 3)
            throw new ArgumentOutOfRangeException(nameof(rules), "minimal board width is 3 cells");
        if (rules.Height < 3)
            throw new ArgumentOutOfRangeException(nameof(rules), "minimal board height is 3 cells");
        if (rules.MarksToWin < 2)
            throw new ArgumentOutOfRangeException(nameof(rules), "count of marks for winning cannot be less than 2");
        if (rules.MarksToWin > rules.Width)
            throw new ArgumentException($"too small board width ({rules.Width}) for the given marks-to-win count ({rules.MarksToWin})", nameof(rules));
        if (rules.MarksToWin > rules.Height)
            throw new ArgumentException($"too small board height ({rules.Height}) for the given marks-to-win count ({rules.MarksToWin})", nameof(rules));

        if (boardOptions.BoardChars.XMark == boardOptions.BoardChars.OMark)
            throw new ArgumentException("X mark cannot be same as O mark", nameof(boardOptions));
        if (boardOptions.BoardChars.XMark == boardOptions.BoardChars.Space)
            throw new ArgumentException("X mark cannot be same as space character", nameof(boardOptions));
        if (boardOptions.BoardChars.OMark == boardOptions.BoardChars.Space)
            throw new ArgumentException("O mark cannot be same as space character", nameof(boardOptions));

        if (boardOptions.HorzCellPadding < 0)
            throw new ArgumentOutOfRangeException(nameof(boardOptions), "horizontal cell padding cannot be less than zero");
        if (boardOptions.VertCellPadding < 0)
            throw new ArgumentOutOfRangeException(nameof(boardOptions), "vertical cell padding cannot be less than zero");

        if (boardOptions.StepDuration.Ticks < 0)
            throw new ArgumentOutOfRangeException(nameof(boardOptions), "step duration cannot be negative");

        Rules = rules;
        BoardOptions = boardOptions;
        Name = name;

        marks = new char[rules.Width, rules.Height];
        ResetMarks();
    }

    public string Name { get; }

    /// <summary>
    /// Steps to be played.
    /// If empty, then random moves will occur, until board is full, or someone wins (see <see cref="Winner"/>).
    /// </summary>
    /// <remarks>
    /// Note: no validation of <see cref="TicTacToeStep.X"/> and <see cref="TicTacToeStep.Y"/> being made neither here,
    /// or in <see cref="RunAsync(CancellationToken)"/>.
    /// </remarks>
    public IEnumerable<TicTacToeStep> Steps { get; set; } = [];

    /// <summary>
    /// Game rules.
    /// </summary>
    public TicTacToeRules Rules { get; }

    /// <summary>
    /// Board drawing options.
    /// </summary>
    public BoardOptions BoardOptions { get; }

    /// <summary>
    /// Winner of game.
    /// The value is either <see cref="BoardChars.Space"/> (initial),
    /// <see cref="BoardChars.XMark"/> or <see cref="BoardChars.OMark"/>.
    /// </summary>
    public char Winner { get; private set; }

    private readonly char[,] marks;

    private void ResetMarks() {
        // No Fill() for multidimensional arrays for now,
        // see https://github.com/dotnet/runtime/issues/47213
        for (int i = 0; i < Rules.Width; i++)
            for (int j = 0; j < Rules.Height; j++)
                marks[i, j] = BoardOptions.BoardChars.Space;

        Winner = BoardOptions.BoardChars.Space;
    }

    public async Task RunAsync(CancellationToken ct = default) {
        ResetMarks();
        char nextMark = BoardOptions.BoardChars.XMark;

        async Task SetMark(int col, int row) {
            marks[col, row] = nextMark;
            int x = col * (BoardOptions.HorzCellPadding * 2 + 2) + BoardOptions.HorzCellPadding;
            int y = row * (BoardOptions.VertCellPadding * 2 + 2) + BoardOptions.VertCellPadding;
            if (BoardOptions.DrawExternalBorder) {
                x++;
                y++;
            }
            Console.SetCursorPosition(x, y);
            await Console.Out.WriteAsync(nextMark);
            nextMark = nextMark == BoardOptions.BoardChars.XMark
                ? BoardOptions.BoardChars.OMark
                : BoardOptions.BoardChars.XMark;
            await Task.Delay(BoardOptions.StepDuration, ct);
        }

        var wasVisible = OperatingSystem.IsWindows() ? Console.CursorVisible : true;
        Console.CursorVisible = false;
        try {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            await DrawBoard();

            for (var j = 0; j < Rules.Height; j++)
                for (var i = 0; i < Rules.Width; i++)
                    marks[i, j] = BoardOptions.BoardChars.Space;

            var steps = Steps;
            if (steps.Any()) {
                foreach (var step in steps) {
                    // no validation if mark was already there!
                    await SetMark(step.X, step.Y);

                    // remember first winner, but allow game to continue
                    if (Winner == BoardOptions.BoardChars.Space) {
                        Winner = FindWinner();
                        if (Winner != BoardOptions.BoardChars.Space)
                            await DrawWinner();
                    }
                }

                // Since steps may be arbitrary, we don't know for sure if there are
                // any free cells left.
                if (Winner == BoardOptions.BoardChars.Space) {
                    bool hasSpace = false;
                    for (int i = 0; !hasSpace && i < Rules.Width; i++)
                        for (int j = 0; !hasSpace && j < Rules.Height; j++)
                            hasSpace = marks[i, j] == BoardOptions.BoardChars.Space;
                    if (!hasSpace)
                        await DrawWinner();
                }
            } else {
                // random moves until somebody wins, or the board is full
                for (int nMovesLeft = Rules.Width * Rules.Height; nMovesLeft > 0; nMovesLeft--) {
                    int idx = Random.Shared.Next(nMovesLeft);
                    int x = idx % Rules.Width;
                    int y = idx / Rules.Width;

                    for (int j = 0; j <= y; j++)
                        for (int i = 0; i <= (j == y ? x : Rules.Width - 1); i++)
                            if (marks[i, j] != BoardOptions.BoardChars.Space) {
                                idx++;
                                x = idx % Rules.Width;
                                y = idx / Rules.Width;
                            }

                    await SetMark(x, y);
                    Winner = FindWinner();
                    if (Winner != BoardOptions.BoardChars.Space) {
                        await DrawWinner();
                        break;
                    }
                }

                if (Winner == BoardOptions.BoardChars.Space)
                    await DrawWinner();
            }
        }
        finally {
            if (OperatingSystem.IsWindows())
                Console.CursorVisible = wasVisible;
            int row = Rules.Height * (BoardOptions.VertCellPadding * 2 + 2) + 1;
            if (BoardOptions.DrawExternalBorder)
                row++;
            Console.SetCursorPosition(0, row);
        }
    }

    private async Task DrawBoard() {
        var opts = BoardOptions;
        var ch = opts.BoardChars;

        for (int j = 0; j < Rules.Height; j++) {
            // top border of the row
            if (j > 0 || opts.DrawExternalBorder) {
                for (int i = 0; i < Rules.Width; i++) {
                    // cell top-left angle
                    if (i == 0)
                        await Console.Out.WriteAsync(j == 0 ? ch.TopLeftAngle : ch.LeftSideCross);
                    else
                        await Console.Out.WriteAsync(j == 0 ? ch.TopSideCross : ch.InnerCross);

                    // cell top border line
                    for (int p = 0; p < opts.HorzCellPadding * 2 + 1; p++)
                        await Console.Out.WriteAsync(j == 0 ? ch.OuterHorzLine : ch.InnerHorzLine);
                }
                await Console.Out.WriteLineAsync(j == 0 ? ch.TopRightAngle : ch.RightSideCross);
            }

            // cell inner contents
            for (int q = 0; q < opts.VertCellPadding * 2 + 1; q++) {
                for (int i = 0; i < Rules.Width; i++) {
                    if (i > 0 || opts.DrawExternalBorder)
                        await Console.Out.WriteAsync(i == 0 ? ch.OuterVertLine : ch.InnerVertLine);
                    for (int p = 0; p < opts.HorzCellPadding; p++)
                        await Console.Out.WriteAsync(ch.Space);
                    await Console.Out.WriteAsync(q == opts.VertCellPadding ? marks[i, j] : ch.Space);
                    for (int p = 0; p < opts.HorzCellPadding; p++)
                        await Console.Out.WriteAsync(ch.Space);
                }
                if (opts.DrawExternalBorder)
                    await Console.Out.WriteAsync(ch.OuterVertLine);
                await Console.Out.WriteLineAsync();
            }
        }

        if (opts.DrawExternalBorder) {
            for (int i = 0; i < Rules.Width; i++) {
                // cell bottom-left angle
                await Console.Out.WriteAsync(i == 0 ? ch.BottomLeftAngle : ch.BottomSideCross);

                // cell bottom border line
                for (int p = 0; p < opts.HorzCellPadding * 2 + 1; p++)
                    await Console.Out.WriteAsync(ch.OuterHorzLine);
            }
            await Console.Out.WriteAsync(ch.BottomRightAngle);
        }
    }

    private char FindWinner() {
        int k;
        for (int i = 0; i < Rules.Width; i++) {
            for (int j = 0; j < Rules.Height; j++) {
                if (marks[i, j] == BoardOptions.BoardChars.Space)
                    continue;

                if (i + Rules.MarksToWin <= Rules.Width) {
                    for (k = 1; k < Rules.MarksToWin; k++)
                        if (marks[i, j] != marks[i + k, j])
                            break;
                    if (k == Rules.MarksToWin)
                        return marks[i, j];
                }

                if (j + Rules.MarksToWin <= Rules.Height) {
                    for (k = 1; k < Rules.MarksToWin; k++)
                        if (marks[i, j] != marks[i, j + k])
                            break;
                    if (k == Rules.MarksToWin)
                        return marks[i, j];
                }

                if (i + Rules.MarksToWin <= Rules.Width && j + Rules.MarksToWin <= Rules.Height) {
                    for (k = 1; k < Rules.MarksToWin; k++)
                        if (marks[i, j] != marks[i + k, j + k])
                            break;
                    if (k == Rules.MarksToWin)
                        return marks[i, j];
                }

                if (i >= Rules.MarksToWin - 1 && j + Rules.MarksToWin <= Rules.Height) {
                    for (k = 1; k < Rules.MarksToWin; k++)
                        if (marks[i, j] != marks[i - k, j + k])
                            break;
                    if (k == Rules.MarksToWin)
                        return marks[i, j];
                }
            }
        }
        return BoardOptions.BoardChars.Space;
    }

    private async Task DrawWinner() {
        int row = Rules.Height * (BoardOptions.VertCellPadding * 2 + 2);
        if (BoardOptions.DrawExternalBorder)
            row++;
        Console.SetCursorPosition(0, row);
        if (Winner != BoardOptions.BoardChars.Space) {
            await Console.Out.WriteAsync($"""
                "{Winner}" won!
                """);
        } else {
            await Console.Out.WriteAsync("Round draw!");
        }
    }
}
