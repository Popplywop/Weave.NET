namespace Weave.UI;

// A small adaptor that lets the Button register itself as a focusable input handler.
public sealed class ButtonInputHandler : IInputHandler
{
    private readonly Func<bool> _disabled;
    private readonly Action _activate;

    public ButtonInputHandler(Func<bool> disabled, Action activate)
    {
        _disabled = disabled;
        _activate = activate;
    }

    public bool OnInput(InputEvent e)
    {
        if (_disabled())
        {
            return false;
        }

        if (e is KeyEvent { Key: ConsoleKey.Enter } or KeyEvent { Key: ConsoleKey.Spacebar })
        {
            _activate();
            return true;
        }
        return false;
    }
}