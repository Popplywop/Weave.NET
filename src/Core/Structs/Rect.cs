namespace Weave.Structs;

internal readonly record struct Rect(int x, int y, int w, int h)
{
    public int Right => x + w - 1;
    public int Bottom => y + h - 1;
    public Rect Deflate(int l, int t, int r, int b)
        => new(x + l, y + t, Math.Max(0, w - l - r), Math.Max(0, h - t - b));
}