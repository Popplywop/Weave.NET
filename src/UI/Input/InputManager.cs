// File: UI/Input/InputManager.cs
using System.Collections.Concurrent;

namespace Weave.UI;

public interface IInputSource
{
    // Non-blocking read: return null if nothing is available this tick.
    KeyEvent? TryReadKey();
    // (Optional) Mouse later: MouseEvent? TryReadMouse();
}

public sealed class ConsoleInputSource : IInputSource
{
    public KeyEvent? TryReadKey()
    {
        if (!Console.KeyAvailable)
        {
            return null;
        }

        var ki = Console.ReadKey(intercept: true);

        var mods = KeyMods.None;
        if ((ki.Modifiers & ConsoleModifiers.Shift) != 0)
        {
            mods |= KeyMods.Shift;
        }

        if ((ki.Modifiers & ConsoleModifiers.Control) != 0)
        {
            mods |= KeyMods.Ctrl;
        }

        if ((ki.Modifiers & ConsoleModifiers.Alt) != 0)
        {
            mods |= KeyMods.Alt;
        }

        // Map printable char when useful (no Ctrl/Alt, or space/enter)
        char? ch = (!char.IsControl(ki.KeyChar) && mods == KeyMods.None) ? ki.KeyChar : null;

        return new KeyEvent(ki.Key, mods, ch);
    }
}

public sealed class InputManager
{
    private readonly FocusManager _focus;
    private readonly IInputSource _source;
    private readonly List<IInputHandler> _globalHandlers = []; // e.g., Esc to quit, F12 debug

    // Simple keybinding layer (optional MVP sugar)
    private readonly ConcurrentDictionary<(ConsoleKey, KeyMods), Action> _bindings = new();

    public InputManager(FocusManager focus, IInputSource source)
    {
        _focus = focus;
        _source = source;
    }

    public void AddGlobalHandler(IInputHandler handler) => _globalHandlers.Add(handler);

    public void Bind(ConsoleKey key, KeyMods mods, Action action) =>
        _bindings[(key, mods)] = action;

    public void Tick() // call this each frame before render
    {
        // Drain available keys
        for(;;)
        {
            var ke = _source.TryReadKey();
            if (ke is null)
            {
                break;
            }

            // Handle built-in focus traversal
            if (ke is { Key: ConsoleKey.Tab } && (ke.Mods & KeyMods.Shift) == 0) { _focus.FocusNext(); continue; }
            if (ke is { Key: ConsoleKey.Tab } && (ke.Mods & KeyMods.Shift) != 0) { _focus.FocusPrev(); continue; }

            // Keybindings (global)
            if (_bindings.TryGetValue((ke.Key, ke.Mods), out var bound))
            {
                bound();
                continue;
            }

            // Dispatch to focused, then globals
            _focus.DispatchToFocused(ke, _globalHandlers);
        }
    }
}
