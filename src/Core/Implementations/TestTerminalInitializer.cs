using Weave.Core.Abstractions;

namespace Weave.Core.Implementations;

/// <summary>
/// Test implementation of terminal initializer that tracks all operations for verification
/// </summary>
public sealed class TestTerminalInitializer : ITerminalInitializer
{
    private readonly List<string> _operations = new();

    /// <summary>
    /// All operations performed on this initializer
    /// </summary>
    public IReadOnlyList<string> Operations => _operations.AsReadOnly();

    /// <summary>
    /// Number of operations performed
    /// </summary>
    public int OperationCount => _operations.Count;

    /// <summary>
    /// Indicates if encoding was set up
    /// </summary>
    public bool EncodingSetup { get; private set; }

    /// <summary>
    /// Indicates if virtual terminal was enabled
    /// </summary>
    public bool VirtualTerminalEnabled { get; private set; }

    /// <summary>
    /// Indicates if alternate screen was entered
    /// </summary>
    public bool AlternateScreenEntered { get; private set; }

    /// <summary>
    /// Indicates if terminal was restored
    /// </summary>
    public bool TerminalRestored { get; private set; }

    /// <summary>
    /// Number of times screen was cleared
    /// </summary>
    public int ScreenClearCount { get; private set; }

    /// <summary>
    /// Clears all tracked operations and resets state
    /// </summary>
    public void Reset()
    {
        _operations.Clear();
        EncodingSetup = false;
        VirtualTerminalEnabled = false;
        AlternateScreenEntered = false;
        TerminalRestored = false;
        ScreenClearCount = 0;
    }

    public void SetupEncoding()
    {
        _operations.Add("SetupEncoding");
        EncodingSetup = true;
    }

    public void EnableVirtualTerminal()
    {
        _operations.Add("EnableVirtualTerminal");
        VirtualTerminalEnabled = true;
    }

    public void EnterAlternateScreen()
    {
        _operations.Add("EnterAlternateScreen");
        AlternateScreenEntered = true;
    }

    public void RestoreTerminal()
    {
        _operations.Add("RestoreTerminal");
        TerminalRestored = true;
    }

    public void ClearScreen()
    {
        _operations.Add("ClearScreen");
        ScreenClearCount++;
    }

    /// <summary>
    /// Checks if a specific operation was performed
    /// </summary>
    public bool HasOperation(string operation) => _operations.Contains(operation);

    /// <summary>
    /// Checks if operations were performed in the expected order
    /// </summary>
    public bool OperationsInOrder(params string[] expectedOperations)
    {
        if (expectedOperations.Length > _operations.Count)
        {
            return false;
        }

        for (int i = 0; i < expectedOperations.Length; i++)
        {
            if (i >= _operations.Count || _operations[i] != expectedOperations[i])
            {
                return false;
            }
        }
        return true;
    }
}