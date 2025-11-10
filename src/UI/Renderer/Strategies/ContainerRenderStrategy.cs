using Weave.UI;
using Weave.UI.Layout;

namespace Weave.UI.Renderer.Strategies;

/// <summary>
/// Render strategy for container nodes (VBox, VViewport) that don't need special rendering
/// </summary>
internal sealed class ContainerRenderStrategy : IRenderStrategy
{
    public bool CanHandle(VNode node) => node is VBox or VViewport;

    public void Render(LayoutNode node, VirtualScreen screen)
    {
        // Container nodes don't render themselves - only their children are rendered
        // This is handled by the recursive rendering in VNodeRenderer
    }
}