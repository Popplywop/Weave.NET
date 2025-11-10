using Weave.UI;

namespace Weave.UI.Layout.Strategies;

/// <summary>
/// Layout strategy for VViewport nodes
/// </summary>
internal sealed class VViewportLayoutStrategy : ILayoutStrategy
{
    public bool CanHandle(VNode node) => node is VViewport;

    public void LayoutNode(LayoutNode node, Action<LayoutNode> recursiveLayoutAction)
    {
        if (node.VNode is not VViewport viewport)
        {
            return;
        }

        // For now, treat viewport like a simple container
        var childNode = new LayoutNode
        {
            VNode = viewport.Child,
            Bounds = node.Bounds,
            Children = []
        };

        node.Children.Add(childNode);
        recursiveLayoutAction(childNode);
    }

    public int GetGrow(VNode node) => 1; // Viewports typically want to grow to fill space

    public int GetFixedHeight(VNode node) => 1; // Minimum height

    public int GetFixedWidth(VNode node) => 1; // Minimum width
}