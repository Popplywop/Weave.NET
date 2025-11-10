using System.Collections.Concurrent;

namespace Weave.UI;

public sealed class FocusManager : IFocusManager
{
    // "NodeId" is a unique identifier for an instance/component in your tree.
    // For MVP, use Guid or an incrementing int. Hook it to your reconciler's instances later.
    public sealed record NodeId(Guid Value)
    {
        public static NodeId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString();
    }

    private readonly List<NodeId> _ring = [];
    private readonly ConcurrentDictionary<NodeId, IInputHandler> _handlers = new();
    private int _focusedIndex = -1;

    public NodeId? Focused => _focusedIndex >= 0 && _focusedIndex < _ring.Count ? _ring[_focusedIndex] : null;

    // Registration lifecycle (call on mount/unmount)
    public void Register(NodeId id, IInputHandler handler, bool focusable = true)
    {
        _handlers[id] = handler;
        if (focusable)
        {
            _ring.Add(id);
        }

        if (_focusedIndex < 0 && _ring.Count > 0)
        {
            _focusedIndex = 0;
        }
    }

    public void Unregister(NodeId id)
    {
        _handlers.TryRemove(id, out _);
        var idx = _ring.IndexOf(id);
        if (idx >= 0)
        {
            _ring.RemoveAt(idx);
            if (_ring.Count == 0) { _focusedIndex = -1; return; }
            if (_focusedIndex >= _ring.Count)
            {
                _focusedIndex = _ring.Count - 1;
            }
        }
    }

    public bool TryGetHandler(NodeId id, out IInputHandler handler) => _handlers.TryGetValue(id, out handler!);

    public void FocusNext()
    {
        if (_ring.Count == 0)
        {
            return;
        }

        _focusedIndex = (_focusedIndex + 1) % _ring.Count;
    }

    public void FocusPrev()
    {
        if (_ring.Count == 0)
        {
            return;
        }

        _focusedIndex = (_focusedIndex - 1 + _ring.Count) % _ring.Count;
    }

    // Bubbling: focused -> (optional) ancestors -> globals. For MVP we do focused -> globals.
    public bool DispatchToFocused(InputEvent e, IEnumerable<IInputHandler>? globalHandlers = null)
    {
        if (Focused is NodeId id && _handlers.TryGetValue(id, out var h) && h.OnInput(e))
        {
            return true;
        }

        if (globalHandlers != null)
        {
            foreach (var g in globalHandlers)
            {
                if (g.OnInput(e))
                {
                    return true;
                }
            }
        }

        return false;
    }
}