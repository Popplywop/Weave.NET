using Weave.Core.Abstractions;

namespace Weave.Core.Implementations;

/// <summary>
/// Native terminal output implementation that uses platform-specific native methods
/// </summary>
public sealed class NativeTerminalOutput : ITerminalOutput
{
    public void Write(ReadOnlySpan<char> text)
    {
        // Convert span to string for NativeMethod compatibility
        // In a future optimization, we could implement span support in NativeMethod
        Write(text.ToString());
    }

    public void Write(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            NativeMethod.WriteToStdout(text);
        }
    }

    public void Clear()
    {
        // Use ANSI clear screen sequence
        Write("\x1b[2J\x1b[H");
    }

    public void Flush()
    {
        // Native methods write directly to stdout, so no explicit flush needed
        // This method exists for interface compatibility and future implementations
        // that might buffer output
    }
}