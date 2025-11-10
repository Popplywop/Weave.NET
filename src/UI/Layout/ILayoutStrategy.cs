using Weave.UI;

namespace Weave.UI.Layout;

/// <summary>
/// Strategy interface for calculating layout of different VNode types
/// </summary>
internal interface ILayoutStrategy
{
    /// <summary>
    /// Checks if this strategy can handle the given VNode type
    /// </summary>
    bool CanHandle(VNode node);

    /// <summary>
    /// Performs layout calculation for the node, including creating child nodes and recursive layout
    /// </summary>
    void LayoutNode(LayoutNode node, Action<LayoutNode> recursiveLayoutAction);

    /// <summary>
    /// Gets the grow factor for the node (how much it should expand)
    /// </summary>
    int GetGrow(VNode node);

    /// <summary>
    /// Gets the fixed height for the node
    /// </summary>
    int GetFixedHeight(VNode node);

    /// <summary>
    /// Gets the fixed width for the node
    /// </summary>
    int GetFixedWidth(VNode node);
}