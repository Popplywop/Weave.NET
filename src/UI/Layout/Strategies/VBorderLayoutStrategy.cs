using Weave.Structs;
using Weave.UI;

namespace Weave.UI.Layout.Strategies;

/// <summary>
/// Layout strategy for VBorder nodes
/// </summary>
internal sealed class VBorderLayoutStrategy : ILayoutStrategy
{
    public bool CanHandle(VNode node) => node is VBorder;

    public void LayoutNode(LayoutNode node, Action<LayoutNode> recursiveLayoutAction)
    {
        if (node.VNode is not VBorder border)
        {
            return;
        }

        // Border takes up 1 char on each side
        var innerBounds = node.Bounds.Deflate(1, 1, 1, 1);

        var childNode = new LayoutNode
        {
            VNode = border.Child,
            Bounds = innerBounds,
            Children = []
        };

        node.Children.Add(childNode);
        recursiveLayoutAction(childNode);
    }

    public int GetGrow(VNode node) => 0; // Borders don't grow by default

    public int GetFixedHeight(VNode node) => 3; // Minimum height for border (top + content + bottom)

    public int GetFixedWidth(VNode node) => 3; // Minimum width for border (left + content + right)
}