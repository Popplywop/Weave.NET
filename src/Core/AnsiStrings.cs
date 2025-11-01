namespace Weave;

internal static class AnsiStrings
{
    private const string CSI = "\x1b[";

    public const string ENABLE_ALT_SCREEN = $"{CSI}?1049h";
    public const string DISABLE_ALT_SCREEN = $"{CSI}?1049l";
    public const string CLEAR_SCREEN = $"{CSI}2J";
    public const string HIDE_CURSOR = $"{CSI}?25l";
    public const string VISIBLE_CURSOR = $"{CSI}?25h";
}