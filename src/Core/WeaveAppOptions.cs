using Weave.UI;

namespace Weave;

/// <summary>
/// Configuration options for WeaveApp behavior.
/// </summary>
public sealed class WeaveAppOptions
{
    /// <summary>
    /// The key combination that will exit the application.
    /// Default is Ctrl+C for standard terminal behavior.
    /// </summary>
    public ConsoleKey ExitKey { get; init; } = ConsoleKey.C;

    /// <summary>
    /// The key modifiers for the exit key combination.
    /// Default is Ctrl for Ctrl+C behavior.
    /// </summary>
    public KeyMods ExitKeyMods { get; init; } = KeyMods.Ctrl;

    /// <summary>
    /// Creates default options with Ctrl+C as the exit keybind.
    /// </summary>
    public static WeaveAppOptions Default => new();
}