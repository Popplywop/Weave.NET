using Weave.Core.Abstractions;

namespace Weave;

/// <summary>
/// Terminal host that manages terminal initialization and lifecycle
/// </summary>
internal sealed class TerminalHost : IDisposable
{
    private readonly ITerminalInitializer _initializer;

    public TerminalHost(ITerminalInitializer initializer)
    {
        _initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));

        _initializer.SetupEncoding();
        _initializer.EnableVirtualTerminal();
        _initializer.EnterAlternateScreen();
    }

    public void Dispose()
    {
        _initializer.RestoreTerminal();
    }

    /// <summary>
    /// Static method for immediate terminal restoration (used for emergency shutdown)
    /// </summary>
    public static void RestoreTerminal()
    {
        // Create a temporary initializer for emergency restore
        var emergencyInitializer = new Core.Implementations.NativeTerminalInitializer();
        emergencyInitializer.RestoreTerminal();
    }
}