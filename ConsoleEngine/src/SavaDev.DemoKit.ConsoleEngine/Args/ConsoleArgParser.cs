namespace SavaDev.DemoKit.ConsoleEngine.Args;

/// <summary>
/// Parses command-line arguments relevant to the demo host.
/// </summary>
internal sealed class ConsoleArgParser
{
    private readonly string[] _args;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleArgParser"/> class.
    /// </summary>
    /// <param name="args">Command-line arguments to parse.</param>
    public ConsoleArgParser(string[] args)
    {
        _args = args ?? Array.Empty<string>();
    }

    /// <summary>
    /// Parses arguments and returns recognized options.
    /// </summary>
    /// <returns>The parsed options.</returns>
    public ArgsOptions TryHandleArgs()
    {
        if (_args.Length == 0)
        {
            return ArgsOptions.Empty;
        }

        var helpRequested = _args.Any(static arg => arg is "--help" or "-h");
        var versionRequested = _args.Any(static arg => arg is "--version" or "-v");

        return new ArgsOptions(helpRequested, versionRequested);
    }
}
