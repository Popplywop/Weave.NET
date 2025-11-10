using System.Collections.Concurrent;

namespace Weave.UI;

public sealed class InputManager
{
    private readonly IFocusManager _focus;
    private readonly IInputSource _source;
    private readonly List<IInputHandler> _globalHandlers = []; // e.g., Esc to quit, F12 debug

    // Simple keybinding layer (optional MVP sugar)
    private readonly ConcurrentDictionary<(ConsoleKey, KeyMods), Action> _bindings = new();

    // Pre-cached key combinations for performance
    private static readonly (ConsoleKey key, KeyMods mods) TabForward = (ConsoleKey.Tab, KeyMods.None);
    private static readonly (ConsoleKey key, KeyMods mods) TabBackward = (ConsoleKey.Tab, KeyMods.Shift);

    public InputManager(IFocusManager focus, IInputSource source)
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
        for (;;)
        {
            var ke = _source.TryReadKey();
            if (ke is null)
            {
                break;
            }

            // Fast focus traversal using cached key combinations
            var keyMods = (ke.Key, ke.Mods);
            if (keyMods == TabForward)
            {
                _focus.FocusNext();
                continue;
            }
            if (keyMods == TabBackward)
            {
                _focus.FocusPrev();
                continue;
            }

            // Keybindings (global) - already optimized with ConcurrentDictionary
            if (_bindings.TryGetValue(keyMods, out var bound))
            {
                bound();
                continue;
            }

            // Dispatch to focused, then globals
            _focus.DispatchToFocused(ke, _globalHandlers);
        }
    }
}