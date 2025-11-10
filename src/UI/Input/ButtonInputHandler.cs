namespace Weave.UI;

/// <summary>
/// Represents a key combination that can activate a button.
/// </summary>
public readonly record struct ButtonActivationKey(ConsoleKey Key, KeyMods Mods = KeyMods.None);

/// <summary>
/// A small adaptor that lets the Button register itself as a focusable input handler.
/// </summary>
public sealed class ButtonInputHandler : IInputHandler
{
    private readonly Func<bool> _disabled;
    private readonly Action _activate;
    private readonly IEnumerable<ButtonActivationKey> _activationKeys;

    /// <summary>
    /// Creates a ButtonInputHandler with default activation keys (Enter and Space).
    /// </summary>
    public ButtonInputHandler(Func<bool> disabled, Action activate)
        : this(disabled, activate, DefaultActivationKeys)
    {
    }

    /// <summary>
    /// Creates a ButtonInputHandler with custom activation keys.
    /// </summary>
    public ButtonInputHandler(Func<bool> disabled, Action activate, IEnumerable<ButtonActivationKey> activationKeys)
    {
        _disabled = disabled;
        _activate = activate;
        _activationKeys = [.. activationKeys];
    }

    /// <summary>
    /// Default activation keys for buttons (Enter and Space).
    /// </summary>
    public static readonly ButtonActivationKey[] DefaultActivationKeys = [
        new(ConsoleKey.Enter),
        new(ConsoleKey.Spacebar)
    ];

    public bool OnInput(InputEvent e)
    {
        if (_disabled())
        {
            return false;
        }

        if (e is KeyEvent keyEvent)
        {
            var activationKey = new ButtonActivationKey(keyEvent.Key, keyEvent.Mods);
            if (_activationKeys.Contains(activationKey))
            {
                _activate();
                return true;
            }
        }

        return false;
    }
}