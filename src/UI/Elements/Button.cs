// File: UI/Elements/Button.cs
namespace Weave.UI;

public sealed record ButtonProps(string Label, Action OnPress, bool Disabled = false, TextStyle? Style = null);

// A small adaptor that lets the Button register itself as a focusable input handler.
public sealed class ButtonInput : IInputHandler
{
    private readonly Func<bool> _disabled;
    private readonly Action _activate;

    public ButtonInput(Func<bool> disabled, Action activate)
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

public static class Button
{
    public static VNode Create(ComponentContext ctx, FocusManager focus, ButtonProps p)
    {
        var (pressed, setPressed) = ctx.UseState(false);

        void Activate()
        {
            if (p.Disabled)
            {
                return;
            }

            setPressed(true);
            p.OnPress();
            setPressed(false);
        }

        // Register with focus manager on mount; unregister on unmount.
        var nodeId = FocusManager.NodeId.New();
        ctx.UseEffect(() =>
        {
            var handler = new ButtonInput(() => p.Disabled, Activate);
            focus.Register(nodeId, handler, focusable: true);
            return; // if you add cleanup support later, call focus.Unregister(nodeId)
        });

        var border = pressed ? new BorderStyle('═','║','╔','╗','╚','╝') : new BorderStyle();

        return new VBorder(border,
            new VBox(new Props(Direction.Row, Padding: Thickness.All(1)),
                [new VText(p.Label, p.Style ?? new TextStyle(Bold: true))]
            )
        ) { Key = $"btn:{p.Label}" };
    }
}
