namespace SavaDev.DemoKit.ConsoleEngine.Args;

/// <summary>
/// Represents parsed command-line flags for the demo host.
/// </summary>
internal sealed class ArgsOptions
{
    /// <summary>
    /// Gets an empty options instance with no flags requested.
    /// </summary>
    public static readonly ArgsOptions Empty = new(false, false);

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgsOptions"/> class.
    /// </summary>
    /// <param name="helpRequested">Indicates whether help was requested.</param>
    /// <param name="versionRequested">Indicates whether version was requested.</param>
    public ArgsOptions(bool helpRequested, bool versionRequested)
    {
        HelpRequested = helpRequested;
        VersionRequested = versionRequested;
    }

    /// <summary>
    /// Gets a value indicating whether help output is requested.
    /// </summary>
    public bool HelpRequested { get; }

    /// <summary>
    /// Gets a value indicating whether version output is requested.
    /// </summary>
    public bool VersionRequested { get; }
}
