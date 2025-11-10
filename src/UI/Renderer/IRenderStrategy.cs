using Weave.UI;
using Weave.UI.Layout;

namespace Weave.UI.Renderer;

/// <summary>
/// Strategy interface for rendering different VNode types
/// </summary>
internal interface IRenderStrategy
{
    /// <summary>
    /// Checks if this strategy can handle the given VNode type
    /// </summary>
    bool CanHandle(VNode node);

    /// <summary>
    /// Renders the VNode to the virtual screen
    /// </summary>
    void Render(LayoutNode node, VirtualScreen screen);
}