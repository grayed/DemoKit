using SavaDev.DemoKit.ConsoleEngine.Args;
using SavaDev.DemoKit.ConsoleEngine.Services;

namespace SavaDev.DemoKit.ConsoleEngine;

/// <summary>
/// Provides a simple interactive console demo engine
/// that renders a menu and executes selected scenarios.
/// </summary>
public sealed class ConsoleDemoEngine
{
    private readonly ConsoleDemoOptions _options;
    private readonly string[] _args;
    private readonly ConsoleArgParser _argProcessor;
    private readonly ConsoleCancelHandler _cancelHandler;
    private readonly ConsoleMenuLoop _menuLoop;
    private readonly ConsoleHelpPrinter _helpPrinter;
    private readonly AppVersionProvider _versionProvider;
    private CancellationTokenSource? _currentScenarioCts;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleDemoEngine"/> class.
    /// </summary>
    /// <param name="options">Optional configuration for the demo engine.</param>
    /// <param name="args">Optional command-line arguments passed to the host.</param>
    public ConsoleDemoEngine(ConsoleDemoOptions? options = null, string[]? args = null)
    {
        _options = options ?? new ConsoleDemoOptions();
        _args = args ?? Array.Empty<string>();

        _helpPrinter = new ConsoleHelpPrinter(_options);
        _versionProvider = new AppVersionProvider();

        var menuRenderer = new ConsoleMenuRenderer(_options);
        var inputReader = new ConsoleMenuInputReader(_options);
        var pause = new ConsolePause(_options);
        var scenarioRenderer = new ConsoleScenarioRenderer(_options);
        var scenarioRunner = new ScenarioRunner(scenarioRenderer, cts => _currentScenarioCts = cts);

        _menuLoop = new ConsoleMenuLoop(_options, menuRenderer, inputReader, scenarioRunner, pause);
        _argProcessor = new ConsoleArgParser(_args);
        _cancelHandler = new ConsoleCancelHandler(_options, () => _currentScenarioCts);
    }

    /// <summary>
    /// Runs the interactive demo menu and executes the selected scenarios.
    /// </summary>
    /// <param name="scenarios">The scenarios available in the menu.</param>
    /// <param name="ct">A cancellation token that can stop the menu loop.</param>
    public async Task RunAsync(
        IReadOnlyList<IConsoleDemoScenario> scenarios,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(scenarios);

        var argResult = _argProcessor.TryHandleArgs();
        if (TryHandleArgResult(argResult))
        {
            return;
        }

        using var demoCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        var handler = RegisterCancelKeyPressHandler();

        try
        {
            var menuActions = BuildMenuActions();
            await _menuLoop.RunAsync(scenarios, menuActions, demoCts.Token, ct);
        }
        finally
        {
            UnregisterCancelKeyPressHandler(handler);
        }
    }

    /// <summary>
    /// Handles parsed command-line options and prints output if needed.
    /// </summary>
    /// <param name="argResult">Parsed arguments.</param>
    /// <returns><c>true</c> when output was handled and execution should stop.</returns>
    private bool TryHandleArgResult(ArgsOptions argResult)
    {
        if (argResult.HelpRequested)
        {
            PrintHelp();
            return true;
        }

        if (argResult.VersionRequested)
        {
            PrintVersion();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Builds the menu actions displayed alongside the scenario list.
    /// </summary>
    /// <returns>A read-only list of menu actions.</returns>
    private IReadOnlyList<MenuAction> BuildMenuActions()
    {
        return new List<MenuAction>
        {
            new("H", "Help", PrintHelp),
            new("V", "Version", PrintVersion)
        };
    }

    /// <summary>
    /// Prints the help text for the demo host.
    /// </summary>
    private void PrintHelp()
    {
        _helpPrinter.Print();
    }

    /// <summary>
    /// Prints the current host version to the console.
    /// </summary>
    private void PrintVersion()
    {
        Console.WriteLine($"Version: {_versionProvider.GetVersion()}");
    }

    /// <summary>
    /// Registers a Ctrl+C handler that controls cancellation behavior
    /// during demo execution.
    /// </summary>
    /// <remarks>
    /// When a scenario is currently running, pressing Ctrl+C requests
    /// cancellation of that scenario via its linked
    /// <see cref="CancellationTokenSource"/>.
    ///
    /// When the demo engine is idle (no scenario is running), Ctrl+C is
    /// treated as an explicit user intent to terminate the demo application.
    /// In this case, and when enabled via
    /// <see cref="ConsoleDemoOptions.ExitOnCancelWhenIdle"/>,
    /// the handler invokes <see cref="Environment.Exit(int)"/> to immediately
    /// terminate the host process.
    ///
    /// This behavior is intentionally designed for standalone demo and
    /// showcase applications, where Ctrl+C is commonly expected to exit
    /// the program rather than return control to an interactive menu.
    /// Consumers embedding the demo engine into a larger host can disable
    /// this behavior via configuration.
    /// </remarks>
    /// <returns>
    /// The registered handler, or <c>null</c> when Ctrl+C handling
    /// is disabled via <see cref="ConsoleDemoOptions.HandleCancelKeyPress"/>.
    /// </returns>
    private ConsoleCancelEventHandler? RegisterCancelKeyPressHandler()
    {
        return _cancelHandler.Register();
    }

    /// <summary>
    /// Unregisters the previously attached Ctrl+C handler, if any.
    /// </summary>
    /// <param name="handler">
    /// The handler returned by <see cref="RegisterCancelKeyPressHandler"/>,
    /// or <c>null</c> when no handler was registered.
    /// </param>
    private static void UnregisterCancelKeyPressHandler(ConsoleCancelEventHandler? handler)
    {
        if (handler is not null)
        {
            Console.CancelKeyPress -= handler;
        }
    }
}
