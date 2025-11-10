namespace Weave.Core.Abstractions;

/// <summary>
/// Abstraction for platform-specific terminal initialization operations
/// </summary>
public interface ITerminalInitializer
{
    /// <summary>
    /// Configures the terminal encoding for proper text display
    /// </summary>
    void SetupEncoding();

    /// <summary>
    /// Enables virtual terminal processing for ANSI escape sequence support
    /// </summary>
    void EnableVirtualTerminal();

    /// <summary>
    /// Initializes terminal for alternate screen mode and hides cursor
    /// </summary>
    void EnterAlternateScreen();

    /// <summary>
    /// Restores terminal to original state (show cursor, exit alternate screen)
    /// </summary>
    void RestoreTerminal();

    /// <summary>
    /// Clears the terminal screen
    /// </summary>
    void ClearScreen();
}