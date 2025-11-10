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