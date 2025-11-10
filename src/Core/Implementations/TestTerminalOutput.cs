using System.Text;
using Weave.Core.Abstractions;

namespace Weave.Core.Implementations;

/// <summary>
/// Test implementation of terminal output that captures all writes for verification
/// </summary>
public sealed class TestTerminalOutput : ITerminalOutput, IDisposable
{
    private readonly StringBuilder _buffer = new();
    private readonly List<string> _writes = new();

    /// <summary>
    /// All text written to this output
    /// </summary>
    public string AllOutput => _buffer.ToString();

    /// <summary>
    /// Individual write operations for detailed verification
    /// </summary>
    public IReadOnlyList<string> Writes => _writes.AsReadOnly();

    /// <summary>
    /// Number of write operations performed
    /// </summary>
    public int WriteCount => _writes.Count;

    /// <summary>
    /// Clears all captured output
    /// </summary>
    public void Reset()
    {
        _buffer.Clear();
        _writes.Clear();
    }

    public void Write(ReadOnlySpan<char> text)
    {
        if (text.IsEmpty)
        {
            return;
        }

        var str = text.ToString();
        _buffer.Append(str);
        _writes.Add(str);
    }

    public void Write(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        _buffer.Append(text);
        _writes.Add(text);
    }

    public void Clear()
    {
        Write("\x1b[2J\x1b[H");
    }

    public void Flush()
    {
        // No-op for test implementation
    }

    /// <summary>
    /// Checks if the output contains the specified text
    /// </summary>
    public bool Contains(string text) => _buffer.ToString().Contains(text);

    /// <summary>
    /// Checks if any write operation matches the predicate
    /// </summary>
    public bool AnyWrite(Func<string, bool> predicate) => _writes.Any(predicate);

    /// <summary>
    /// Gets the last write operation, or null if no writes have occurred
    /// </summary>
    public string? LastWrite => _writes.Count > 0 ? _writes[^1] : null;

    public void Dispose()
    {
        // No resources to dispose for test implementation
        _buffer.Clear();
        _writes.Clear();
    }
}