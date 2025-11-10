using Weave.UI;
using Weave.UI.Layout;

namespace Weave.UI.Renderer.Strategies;

/// <summary>
/// Render strategy for VBorder nodes
/// </summary>
internal sealed class VBorderRenderStrategy : IRenderStrategy
{
    public bool CanHandle(VNode node) => node is VBorder;

    public void Render(LayoutNode node, VirtualScreen screen)
    {
        if (node.VNode is not VBorder border)
        {
            return;
        }

        var bounds = node.Bounds;
        if (bounds.w < 2 || bounds.h < 2)
        {
            return;
        }

        var style = border.Style;

        // Top edge
        screen.Plot(bounds.y, bounds.x, style.TL);
        for (int x = bounds.x + 1; x < bounds.x + bounds.w - 1; x++)
        {
            screen.Plot(bounds.y, x, style.H);
        }
        screen.Plot(bounds.y, bounds.x + bounds.w - 1, style.TR);

        // Side edges
        for (int y = bounds.y + 1; y < bounds.y + bounds.h - 1; y++)
        {
            screen.Plot(y, bounds.x, style.V);
            screen.Plot(y, bounds.x + bounds.w - 1, style.V);
        }

        // Bottom edge
        screen.Plot(bounds.y + bounds.h - 1, bounds.x, style.BL);
        for (int x = bounds.x + 1; x < bounds.x + bounds.w - 1; x++)
        {
            screen.Plot(bounds.y + bounds.h - 1, x, style.H);
        }
        screen.Plot(bounds.y + bounds.h - 1, bounds.x + bounds.w - 1, style.BR);
    }
}