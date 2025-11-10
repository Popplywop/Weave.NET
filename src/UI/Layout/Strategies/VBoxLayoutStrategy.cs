using Weave.Structs;
using Weave.UI;

namespace Weave.UI.Layout.Strategies;

/// <summary>
/// Layout strategy for VBox nodes (containers)
/// </summary>
internal sealed class VBoxLayoutStrategy : ILayoutStrategy
{
    private readonly LayoutStrategyRegistry? _strategyRegistry;

    public VBoxLayoutStrategy(LayoutStrategyRegistry? strategyRegistry = null)
    {
        _strategyRegistry = strategyRegistry;
    }

    public bool CanHandle(VNode node) => node is VBox;

    public void LayoutNode(LayoutNode node, Action<LayoutNode> recursiveLayoutAction)
    {
        if (node.VNode is not VBox box)
        {
            return;
        }

        // Create layout nodes for children
        foreach (var child in box.Children)
        {
            node.Children.Add(new LayoutNode
            {
                VNode = child,
                Bounds = new Rect(0, 0, 0, 0), // Will be calculated
                Children = []
            });
        }

        // Apply padding
        var padding = box.Props.Padding ?? new Thickness();
        var contentBounds = node.Bounds.Deflate(padding.Left, padding.Top, padding.Right, padding.Bottom);

        if (box.Props.Direction == Direction.Column)
        {
            LayoutChildrenVertically(node.Children, contentBounds);
        }
        else
        {
            LayoutChildrenHorizontally(node.Children, contentBounds);
        }

        // Recursively layout children
        foreach (var child in node.Children)
        {
            recursiveLayoutAction(child);
        }
    }

    public int GetGrow(VNode node)
    {
        if (node is VBox box)
        {
            return box.Props.Grow;
        }

        return 0;
    }

    public int GetFixedHeight(VNode node)
    {
        if (node is VBox box)
        {
            return box.Props.Height ?? 1;
        }

        return 1;
    }

    public int GetFixedWidth(VNode node)
    {
        if (node is VBox box)
        {
            return box.Props.Width ?? 10;
        }

        return 10;
    }

    private void LayoutChildrenVertically(List<LayoutNode> children, Rect bounds)
    {
        if (children.Count == 0)
        {
            return;
        }

        var totalGrow = children.Sum(c => GetStrategyGrow(c.VNode));
        var fixedHeight = children.Sum(c => GetStrategyFixedHeight(c.VNode));
        var availableHeight = Math.Max(0, bounds.h - fixedHeight);

        var y = bounds.y;

        foreach (var child in children)
        {
            var grow = GetStrategyGrow(child.VNode);
            var childHeight = GetStrategyFixedHeight(child.VNode);

            if (grow > 0 && totalGrow > 0)
            {
                childHeight += (int)((float)availableHeight * grow / totalGrow);
            }

            child.Bounds = new Rect(bounds.x, y, bounds.w, Math.Max(0, childHeight));
            y += childHeight;
        }
    }

    private void LayoutChildrenHorizontally(List<LayoutNode> children, Rect bounds)
    {
        if (children.Count == 0)
        {
            return;
        }

        var totalGrow = children.Sum(c => GetStrategyGrow(c.VNode));
        var fixedWidth = children.Sum(c => GetStrategyFixedWidth(c.VNode));
        var availableWidth = Math.Max(0, bounds.w - fixedWidth);

        var x = bounds.x;

        foreach (var child in children)
        {
            var grow = GetStrategyGrow(child.VNode);
            var childWidth = GetStrategyFixedWidth(child.VNode);

            if (grow > 0 && totalGrow > 0)
            {
                childWidth += (int)((float)availableWidth * grow / totalGrow);
            }

            child.Bounds = new Rect(x, bounds.y, Math.Max(0, childWidth), bounds.h);
            x += childWidth;
        }
    }

    private int GetStrategyGrow(VNode node)
    {
        if (_strategyRegistry != null)
        {
            var strategy = _strategyRegistry.GetStrategy(node);
            return strategy.GetGrow(node);
        }
        // Fallback for when no registry is available
        return node is VBox box ? box.Props.Grow : 0;
    }

    private int GetStrategyFixedHeight(VNode node)
    {
        if (_strategyRegistry != null)
        {
            var strategy = _strategyRegistry.GetStrategy(node);
            return strategy.GetFixedHeight(node);
        }
        // Fallback for when no registry is available
        return node switch
        {
            VBox box => box.Props.Height ?? 1,
            VText _ => 1,
            _ => 1
        };
    }

    private int GetStrategyFixedWidth(VNode node)
    {
        if (_strategyRegistry != null)
        {
            var strategy = _strategyRegistry.GetStrategy(node);
            return strategy.GetFixedWidth(node);
        }
        // Fallback for when no registry is available
        return node switch
        {
            VBox box => box.Props.Width ?? 10,
            VText text => text.Text.Length,
            _ => 10
        };
    }
}