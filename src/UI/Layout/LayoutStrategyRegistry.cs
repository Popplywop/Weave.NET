using Weave.UI.Layout.Strategies;
using Weave.UI;

namespace Weave.UI.Layout;

/// <summary>
/// Registry that manages layout strategies and selects the appropriate one for each VNode type
/// </summary>
internal sealed class LayoutStrategyRegistry
{
    private readonly List<ILayoutStrategy> _strategies = new();
    private readonly DefaultLayoutStrategy _defaultStrategy = new();

    public LayoutStrategyRegistry()
    {
        // Register built-in strategies in priority order
        // More specific strategies should be registered first
        RegisterStrategy(new VTextLayoutStrategy());
        RegisterStrategy(new VBoxLayoutStrategy(this));
        RegisterStrategy(new VBorderLayoutStrategy());
        RegisterStrategy(new VViewportLayoutStrategy());
    }

    /// <summary>
    /// Registers a new layout strategy
    /// </summary>
    public void RegisterStrategy(ILayoutStrategy strategy)
    {
        if (strategy == null)
        {
            throw new ArgumentNullException(nameof(strategy));
        }

        _strategies.Add(strategy);
    }

    /// <summary>
    /// Gets the appropriate layout strategy for the given VNode
    /// </summary>
    public ILayoutStrategy GetStrategy(VNode node)
    {
        if (node == null)
        {
            return _defaultStrategy;
        }

        // Find the first strategy that can handle this node type
        foreach (var strategy in _strategies)
        {
            if (strategy.CanHandle(node))
            {
                return strategy;
            }
        }

        // Fall back to default strategy
        return _defaultStrategy;
    }

    /// <summary>
    /// Gets all registered strategies (excluding default)
    /// </summary>
    public IReadOnlyList<ILayoutStrategy> GetAllStrategies() => _strategies.AsReadOnly();

    /// <summary>
    /// Clears all registered strategies
    /// </summary>
    public void ClearStrategies()
    {
        _strategies.Clear();
    }

    /// <summary>
    /// Checks if a strategy is registered for the given VNode type
    /// </summary>
    public bool HasStrategyFor(VNode node)
    {
        if (node == null)
        {
            return false;
        }

        return _strategies.Any(strategy => strategy.CanHandle(node));
    }
}