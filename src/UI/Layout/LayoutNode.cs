using Weave.Structs;
using Weave.UI;

namespace Weave.UI.Layout;

/// <summary>
/// Represents a node in the layout tree with calculated position and size.
/// </summary>
internal sealed class LayoutNode
{
    public required VNode VNode { get; init; }
    public required Rect Bounds { get; set; }
    public required List<LayoutNode> Children { get; init; } = [];

    /// <summary>
    /// Actual calculated height after layout
    /// </summary>
    public int ActualHeight { get; set; }

    /// <summary>
    /// Actual calculated width after layout
    /// </summary>
    public int ActualWidth { get; set; }
}