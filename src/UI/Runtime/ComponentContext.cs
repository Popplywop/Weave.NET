namespace Weave.UI;

public sealed class ComponentContext
{
    private readonly List<object?> _state = [];
    private int _cursor = 0;

    internal Action? RequestRerender { get; set; }
    internal List<Action> PostCommitEffects { get; } = [];
    internal IFocusManager? FocusManager { get; set; }

    public (T value, Action<T> set) UseState<T>(T initial)
    {
        var idx = _cursor++;
        if (_state.Count <= idx)
        {
            _state.Add(initial);
        }

        return ((T)_state[idx]!, v => { _state[idx] = v; RequestRerender?.Invoke(); });
    }

    public void UseEffect(Action effect) => PostCommitEffects.Add(effect);

    internal void BeginRender() => _cursor = 0;
}
