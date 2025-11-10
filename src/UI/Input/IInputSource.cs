namespace Weave.UI;

public interface IInputSource
{
    // Non-blocking read: return null if nothing is available this tick.
    KeyEvent? TryReadKey();
    // (Optional) Mouse later: MouseEvent? TryReadMouse();
}