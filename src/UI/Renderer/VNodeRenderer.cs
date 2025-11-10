using Weave.UI.Layout;

namespace Weave.UI.Renderer;

/// <summary>
/// Renders LayoutNode trees to VirtualScreen using the Strategy pattern.
/// </summary>
internal sealed class VNodeRenderer
{
    private readonly VirtualScreen _screen;
    private readonly RenderStrategyRegistry _strategyRegistry;

    public VNodeRenderer(VirtualScreen screen, RenderStrategyRegistry? strategyRegistry = null)
    {
        _screen = screen;
        _strategyRegistry = strategyRegistry ?? new RenderStrategyRegistry();
    }

    public void Render(LayoutNode root)
    {
        _screen.ClearNext();
        RenderNodeRecursive(root);
    }

    private void RenderNodeRecursive(LayoutNode node)
    {
        // Use strategy to render the node
        var strategy = _strategyRegistry.GetStrategy(node.VNode);
        strategy.Render(node, _screen);

        // Render all children
        foreach (var child in node.Children)
        {
            RenderNodeRecursive(child);
        }
    }
}