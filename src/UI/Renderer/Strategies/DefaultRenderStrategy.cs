using Weave.UI;
using Weave.UI.Layout;

namespace Weave.UI.Renderer.Strategies;

/// <summary>
/// Default render strategy for unknown VNode types
/// </summary>
internal sealed class DefaultRenderStrategy : IRenderStrategy
{
    public bool CanHandle(VNode node) => true; // Handles any node as fallback

    public void Render(LayoutNode node, VirtualScreen screen)
    {
        // Default behavior: render a placeholder indicating unknown node type
        var bounds = node.Bounds;
        if (bounds.w > 0 && bounds.h > 0)
        {
            var nodeTypeName = node.VNode.GetType().Name;
            var placeholder = $"[{nodeTypeName}]";

            // Truncate if too long
            if (placeholder.Length > bounds.w)
            {
                placeholder = placeholder[..bounds.w];
            }

            screen.Put(bounds.y, bounds.x, placeholder);
        }
    }
}