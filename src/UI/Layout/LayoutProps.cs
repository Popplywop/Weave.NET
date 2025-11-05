namespace Weave.UI;

public enum Direction { Row, Column }
public enum Align { Start, Center, End, Stretch }
public enum Justify { Start, Center, End, SpaceBetween }
public enum Overflow { Visible, Clip, Scroll }

public sealed record Thickness(int Left = 0, int Top = 0, int Right = 0, int Bottom = 0)
{
    public static Thickness All(int v) => new(v, v, v, v);
};

public sealed record Props(
    Direction Direction = Direction.Column,
    int? Width = null,           // columns (null = auto)
    int? Height = null,          // rows (null = auto)
    int Grow = 0,                // flex-grow
    int Shrink = 1,              // flex-shrink
    int? Basis = null,           // preferred size
    Thickness? Padding = null,
    Align Cross = Align.Start,
    Justify Main = Justify.Start,
    Overflow Overflow = Overflow.Visible
);
