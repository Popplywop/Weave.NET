// File: UI/Input/InputEvents.cs
namespace Weave.UI;

[Flags]
public enum KeyMods { None = 0, Shift = 1, Ctrl = 2, Alt = 4 }

public abstract record InputEvent;

public sealed record KeyEvent(
    ConsoleKey Key, 
    KeyMods Mods = KeyMods.None, 
    char? Character = null
) : InputEvent;

public enum MouseButton { Left, Middle, Right }
public enum MouseAction { Down, Up, Move, Wheel }
public sealed record MouseEvent(
    int X, int Y, MouseButton Button, MouseAction Action, int Delta = 0, KeyMods Mods = KeyMods.None
) : InputEvent;

// Handlers return true if they handled the event and want to stop bubbling.
public interface IInputHandler
{
    bool OnInput(InputEvent e);
}
