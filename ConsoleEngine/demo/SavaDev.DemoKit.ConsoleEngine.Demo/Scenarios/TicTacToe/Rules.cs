namespace SavaDev.DemoKit.ConsoleEngine.Demo.Scenarios.TicTacToe;

public record TicTacToeRules(
    int Width = 3,
    int Height = 3,
    int MarksToWin = 3
);

public record TicTacToeStep(int X, int Y);
