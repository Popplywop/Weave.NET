namespace Weave.UI;

public sealed record ButtonProps(
    string Label,
    Action OnPress,
    bool Disabled = false,
    TextStyle? Style = null,
    IEnumerable<ButtonActivationKey>? ActivationKeys = null
);

public static class Button
{
    public static VNode Create(ComponentContext ctx, ButtonProps p)
    {
        var focus = ctx.FocusManager ?? throw new InvalidOperationException("FocusManager not available in ComponentContext");
        return CreateInternal(ctx, focus, p);
    }

    public static VNode Create(ComponentContext ctx, IFocusManager focus, ButtonProps p)
    {
        return CreateInternal(ctx, focus, p);
    }

    private static VBorder CreateInternal(ComponentContext ctx, IFocusManager focus, ButtonProps p)
    {
        var (pressed, setPressed) = ctx.UseState(false);
        var (isProcessing, setIsProcessing) = ctx.UseState(false);

        void Activate()
        {
            if (p.Disabled || isProcessing)
            {
                return; // Prevent multiple rapid presses
            }

            setIsProcessing(true);
            setPressed(true);

            // Execute the action immediately
            p.OnPress();

            // Reset state after visual feedback delay
            _ = Task.Delay(150).ContinueWith(_ =>
            {
                setPressed(false);
                setIsProcessing(false);
            });
        }

        // Create stable NodeId based on button label
        var stableId = $"btn-{p.Label}".GetHashCode();
        var nodeId = new FocusManager.NodeId(new Guid(stableId, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

        // Always update handler to get fresh closures, but prevent duplicate focus ring entries
        var activationKeys = p.ActivationKeys ?? ButtonInputHandler.DefaultActivationKeys;
        var handler = new ButtonInputHandler(() => p.Disabled || isProcessing, Activate, activationKeys);
        var alreadyRegistered = focus.TryGetHandler(nodeId, out _);

        if (alreadyRegistered)
        {
            // Update handler but don't add to focus ring again
            focus.Register(nodeId, handler, focusable: false);
        }
        else
        {
            // First registration - add to focus ring
            focus.Register(nodeId, handler, focusable: true);
        }

        var border = pressed ? new BorderStyle('═','║','╔','╗','╚','╝') : new BorderStyle();

        return new VBorder(border,
            new VBox(new Props(Direction.Row, Padding: Thickness.All(1)),
                [new VText(p.Label, p.Style ?? new TextStyle(Bold: true))]
            )
        ) { Key = $"btn:{p.Label}" };
    }
}