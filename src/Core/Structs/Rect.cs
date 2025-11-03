namespace Weave.Structs;

internal readonly record struct Rect(int X,int Y,int W,int H)
{
    public int Right => X+W-1;
    public int Bottom => Y+H-1;
    public Rect Deflate(int l,int t,int r,int b)
        => new(X+l, Y+t, Math.Max(0, W-l-r), Math.Max(0, H-t-b));
}