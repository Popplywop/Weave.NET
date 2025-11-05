namespace Weave.UI;

public sealed record Color(byte R, byte G, byte B);

public sealed record TextStyle(
    bool Bold = false,
    bool Italic = false,
    bool Underline = false,
    Color? Fg = null,
    Color? Bg = null
);
