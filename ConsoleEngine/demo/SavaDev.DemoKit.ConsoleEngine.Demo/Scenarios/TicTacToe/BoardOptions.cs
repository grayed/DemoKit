namespace SavaDev.DemoKit.ConsoleEngine.Demo.Scenarios.TicTacToe;

/// <summary>
/// Define characters used when drawing a tic-tac-toe board.
/// </summary>
public record BoardChars(
    char InnerHorzLine,
    char InnerVertLine,
    char InnerCross,
    char OuterHorzLine,
    char OuterVertLine,
    char LeftSideCross,
    char RightSideCross,
    char TopSideCross,
    char BottomSideCross,
    char TopLeftAngle,
    char TopRightAngle,
    char BottomLeftAngle,
    char BottomRightAngle,
    char XMark = 'X',
    char OMark = 'O',
    char Space = ' '
)
{
    /// <summary>All lines are single.</summary>
    public static readonly BoardChars AllSingle      = new('─', '│', '┼', '─', '│', '├', '┤', '┬', '┴', '┌', '┐', '└', '┘');

    /// <summary>All lines are double.</summary>
    public static readonly BoardChars AllDouble      = new('═', '║', '╬', '═', '║', '╠', '╢', '╦', '╩', '╔', '╗', '╚', '╝');

    /// <summary>Inner lines are single, outer are double.</summary>
    public static readonly BoardChars SingleInDouble = new('─', '│', '┼', '═', '║', '╟', '╢', '╤', '╧', '╔', '╗', '╚', '╝');

    /// <summary>Inner lines are double, outer are single.</summary>
    public static readonly BoardChars DoubleInSingle = new('═', '║', '╬', '─', '│', '╞', '╡', '╥', '╨', '┌', '┐', '└', '┘');
}

/// <summary>
/// Tic-tac-toe board drawing options.
/// </summary>
/// <param name="BoardChars">Set of characters used for drawing.</param>
/// <param name="StepDuration">Time before each mark being put on.</param>
/// <param name="HorzCellPadding">Size of padding between player's mark and cell border.</param>
/// <param name="DrawExternalBorder">
/// If <c>false</c>, only inner board lines will be drawn; thus, only <see cref="BoardChars.InnerHorzLine"/>,
/// <see cref="BoardChars.InnerVertLine"/> and <see cref="BoardChars.InnerCross"/> will be used.
/// </param>
/// <param name="XMark">Character used for crosses.</param>
/// <param name="OMark">Character used for noughts.</param>
/// <param name="Space">Character used for drawing empty (reserved) mark space.</param>
public record BoardOptions(
    BoardChars BoardChars,
    TimeSpan StepDuration,
    int HorzCellPadding = 2,
    int VertCellPadding = 1,
    bool DrawExternalBorder = true
) {
    public static readonly BoardOptions Default = new(BoardChars.DoubleInSingle, TimeSpan.FromSeconds(1));
}
