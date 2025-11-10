using Weave.Structs;
using Weave.UI.Layout;

namespace Weave.UI.Renderer;

/// <summary>
/// Calculates layout positions and sizes for VNode trees using the Strategy pattern.
/// </summary>
internal sealed class LayoutEngine
{
    private readonly LayoutStrategyRegistry _strategyRegistry;

    public LayoutEngine(LayoutStrategyRegistry? strategyRegistry = null)
    {
        _strategyRegistry = strategyRegistry ?? new LayoutStrategyRegistry();
    }

    public LayoutNode CalculateLayout(VNode root, int width, int height)
    {
        var rootLayout = new LayoutNode
        {
            VNode = root,
            Bounds = new Rect(0, 0, width, height),
            Children = []
        };

        LayoutNodeRecursive(rootLayout);
        return rootLayout;
    }

    private void LayoutNodeRecursive(LayoutNode node)
    {
        var strategy = _strategyRegistry.GetStrategy(node.VNode);
        strategy.LayoutNode(node, LayoutNodeRecursive);
    }
}