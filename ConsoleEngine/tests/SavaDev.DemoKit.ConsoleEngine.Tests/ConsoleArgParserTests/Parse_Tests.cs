using SavaDev.DemoKit.ConsoleEngine.Args;

namespace SavaDev.DemoKit.ConsoleEngine.Tests.ConsoleArgParserTests;

/// <summary>
/// Contains tests that verify the behavior of
/// <see cref="ConsoleArgParser.TryHandleArgs"/>.
/// </summary>
/// <remarks>
/// These tests focus on recognition of help and
/// version flags in command-line arguments.
/// </remarks>
public sealed class Parse_Tests
{
    /// <summary>
    /// Verifies that empty arguments return a result
    /// with no flags requested.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// <list type="bullet">
    /// <item>Help is not requested.</item>
    /// <item>Version is not requested.</item>
    /// </list>
    /// </remarks>
    [Fact]
    public void TryHandleArgs_WhenArgsEmpty_ShouldReturnNoFlags()
    {
        // Arrange
        var parser = new ConsoleArgParser(Array.Empty<string>());

        // Act
        var result = parser.TryHandleArgs();

        // Assert
        Assert.False(result.HelpRequested);
        Assert.False(result.VersionRequested);
    }

    /// <summary>
    /// Verifies that null arguments are treated as empty.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// <list type="bullet">
    /// <item>Help is not requested.</item>
    /// <item>Version is not requested.</item>
    /// </list>
    /// </remarks>
    [Fact]
    public void TryHandleArgs_WhenArgsNull_ShouldReturnNoFlags()
    {
        // Arrange
        var parser = new ConsoleArgParser(null!);

        // Act
        var result = parser.TryHandleArgs();

        // Assert
        Assert.False(result.HelpRequested);
        Assert.False(result.VersionRequested);
    }

    /// <summary>
    /// Verifies that the long help flag sets
    /// the help request.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// <list type="bullet">
    /// <item>Help is requested.</item>
    /// <item>Version is not requested.</item>
    /// </list>
    /// </remarks>
    [Fact]
    public void TryHandleArgs_WhenLongHelpFlagProvided_ShouldRequestHelp()
    {
        // Arrange
        var parser = new ConsoleArgParser(["--help"]);

        // Act
        var result = parser.TryHandleArgs();

        // Assert
        Assert.True(result.HelpRequested);
        Assert.False(result.VersionRequested);
    }

    /// <summary>
    /// Verifies that the short help flag sets
    /// the help request.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// <list type="bullet">
    /// <item>Help is requested.</item>
    /// <item>Version is not requested.</item>
    /// </list>
    /// </remarks>
    [Fact]
    public void TryHandleArgs_WhenShortHelpFlagProvided_ShouldRequestHelp()
    {
        // Arrange
        var parser = new ConsoleArgParser(["-h"]);

        // Act
        var result = parser.TryHandleArgs();

        // Assert
        Assert.True(result.HelpRequested);
        Assert.False(result.VersionRequested);
    }

    /// <summary>
    /// Verifies that the long version flag sets
    /// the version request.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// <list type="bullet">
    /// <item>Version is requested.</item>
    /// <item>Help is not requested.</item>
    /// </list>
    /// </remarks>
    [Fact]
    public void TryHandleArgs_WhenLongVersionFlagProvided_ShouldRequestVersion()
    {
        // Arrange
        var parser = new ConsoleArgParser(["--version"]);

        // Act
        var result = parser.TryHandleArgs();

        // Assert
        Assert.True(result.VersionRequested);
        Assert.False(result.HelpRequested);
    }

    /// <summary>
    /// Verifies that the short version flag sets
    /// the version request.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// <list type="bullet">
    /// <item>Version is requested.</item>
    /// <item>Help is not requested.</item>
    /// </list>
    /// </remarks>
    [Fact]
    public void TryHandleArgs_WhenShortVersionFlagProvided_ShouldRequestVersion()
    {
        // Arrange
        var parser = new ConsoleArgParser(["-v"]);

        // Act
        var result = parser.TryHandleArgs();

        // Assert
        Assert.True(result.VersionRequested);
        Assert.False(result.HelpRequested);
    }

    /// <summary>
    /// Verifies that requesting both help and version
    /// sets both flags.
    /// </summary>
    /// <remarks>
    /// Expected behavior:
    /// <list type="bullet">
    /// <item>Help is requested.</item>
    /// <item>Version is requested.</item>
    /// </list>
    /// </remarks>
    [Fact]
    public void TryHandleArgs_WhenHelpAndVersionProvided_ShouldRequestBoth()
    {
        // Arrange
        var parser = new ConsoleArgParser(["--help", "--version"]);

        // Act
        var result = parser.TryHandleArgs();

        // Assert
        Assert.True(result.HelpRequested);
        Assert.True(result.VersionRequested);
    }
}
