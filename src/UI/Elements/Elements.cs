namespace Weave.UI;

// Convenience factories to reduce noise in user code
public static class Elements
{
    public static VNode Row(params VNode[] children) =>
        new VBox(new Props(Direction.Row), children);

    public static VNode Col(params VNode[] children) =>
        new VBox(new Props(Direction.Column), children);

    public static VNode Box(Props props, params VNode[] children) =>
        new VBox(props, children);

    public static VNode Text(string s, TextStyle? style = null, Align align = Align.Start, string? key = null) =>
        new VText(s, style, align, key);

    public static VNode Spacer(int grow = 1, string? key = null) =>
        new VBox(new Props(Direction.Row, Grow: grow), [], key);
}