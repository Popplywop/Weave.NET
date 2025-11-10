using Weave.UI;

namespace Weave.UI.Layout.Strategies;

/// <summary>
/// Default layout strategy for unknown VNode types
/// </summary>
internal sealed class DefaultLayoutStrategy : ILayoutStrategy
{
    public bool CanHandle(VNode node) => true; // Handles any node as fallback

    public void LayoutNode(LayoutNode node, Action<LayoutNode> recursiveLayoutAction)
    {
        // Default behavior: unknown node type, skip layout
        // This is a safe fallback that doesn't create children or modify bounds
    }

    public int GetGrow(VNode node) => 0; // Unknown nodes don't grow by default

    public int GetFixedHeight(VNode node) => 1; // Default height

    public int GetFixedWidth(VNode node) => 10; // Default width
}