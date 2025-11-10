namespace Weave.Core.Abstractions;

/// <summary>
/// Abstraction for terminal output operations, enabling dependency injection and testability
/// </summary>
public interface ITerminalOutput
{
    /// <summary>
    /// Writes text directly to the terminal output stream
    /// </summary>
    /// <param name="text">The text to write, may contain ANSI escape sequences</param>
    void Write(ReadOnlySpan<char> text);

    /// <summary>
    /// Writes text directly to the terminal output stream
    /// </summary>
    /// <param name="text">The text to write, may contain ANSI escape sequences</param>
    void Write(string text);

    /// <summary>
    /// Clears the terminal screen
    /// </summary>
    void Clear();

    /// <summary>
    /// Flushes any buffered output to the terminal
    /// </summary>
    void Flush();
}