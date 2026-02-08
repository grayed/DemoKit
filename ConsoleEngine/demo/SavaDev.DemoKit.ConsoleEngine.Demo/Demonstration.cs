using SavaDev.DemoKit.ConsoleEngine.Demo.Scenarios;
using SavaDev.DemoKit.ConsoleEngine.Demo.Scenarios.TicTacToe;

namespace SavaDev.DemoKit.ConsoleEngine.Demo;

/// <summary>
/// Serves as an entry point for running a console-based demonstration
/// composed of multiple interactive scenarios.
/// </summary>
/// <remarks>
/// This class is responsible for:
/// <list type="bullet">
/// <item>Defining the set of demo scenarios available to the user.</item>
/// <item>Configuring the demo engine.</item>
/// <item>Starting the interactive execution flow.</item>
/// </list>
///
/// It acts as a thin composition root for demo applications,
/// keeping scenario wiring and engine configuration in one place
/// while leaving execution logic to the demo engine itself.
///
/// The class intentionally contains no business logic and is meant
/// to be easy to read, modify, and adapt when creating new demos
/// or experimenting with different scenario combinations.
/// </remarks>
internal class Demonstration
{
    /// <summary>
    /// Runs the interactive demo menu and executes
    /// the selected scenarios.
    /// </summary>
    public static async Task RunAsync(string[]? args = null)
    {
        var scenarios = BuildScenarios();

        var options = new ConsoleDemoOptions
        {
            Title = "SavaDev.DemoKit.ConsoleEngine - Demo"
        };

        var engine = new ConsoleDemoEngine(options, args);
        await engine.RunAsync(scenarios);
    }

    /// <summary>
    /// Builds the list of available demo scenarios.
    /// </summary>
    /// <returns>
    /// A read-only list of scenarios displayed in the menu.
    /// </returns>
    private static IReadOnlyList<IConsoleDemoScenario> BuildScenarios()
        => [
            new HelloScenario(),
            new SleepScenario(),
            new CountdownScenario(),
            new ErrorScenario(),
            new CtrlCScenario(),
            new TicTacToeScenario(new(7, 3), BoardOptions.Default),
        ];
}
