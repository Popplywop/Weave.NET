namespace Weave.UI;

// ---------- TEXT ----------
public sealed record VText(
    string Text,
    TextStyle? Style = null,
    Align Align = Align.Start,
    string? Key = null
) : VNode(Key);

// ---------- BOX (Row / Column container) ----------
public sealed record VBox(
    Props Props,
    VNode[] Children,
    string? Key = null
) : VNode(Key);

// Convenience factories to reduce noise in user code
public static class UI
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

// ---------- BORDER ----------
public sealed record BorderStyle(
    char H = '─', char V = '│',
    char TL = '┌', char TR = '┐', char BL = '└', char BR = '┘'
);

public sealed record VBorder(
    BorderStyle Style,
    VNode Child,
    string? Key = null
) : VNode(Key);

// ---------- VIEWPORT (scrollable/clip area) ----------
public sealed record VViewport(
    int? Width, int? Height,
    Overflow Overflow = Overflow.Clip,
    VNode Child = null!,
    string? Key = null
) : VNode(Key);

// ---------- STATIC (non-diffed log area, optional MVP) ----------
public sealed record VStatic(
    VNode[] Children,
    string? Key = null
) : VNode(Key);
