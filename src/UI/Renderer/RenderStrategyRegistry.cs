using Weave.UI;
using Weave.UI.Renderer.Strategies;

namespace Weave.UI.Renderer;

/// <summary>
/// Registry that manages render strategies and selects the appropriate one for each VNode type
/// </summary>
internal sealed class RenderStrategyRegistry
{
    private readonly List<IRenderStrategy> _strategies = new();
    private readonly DefaultRenderStrategy _defaultStrategy = new();

    public RenderStrategyRegistry()
    {
        // Register built-in strategies in priority order
        // More specific strategies should be registered first
        RegisterStrategy(new VTextRenderStrategy());
        RegisterStrategy(new VBorderRenderStrategy());
        RegisterStrategy(new ContainerRenderStrategy());
    }

    /// <summary>
    /// Registers a new render strategy
    /// </summary>
    public void RegisterStrategy(IRenderStrategy strategy)
    {
        if (strategy == null)
        {
            throw new ArgumentNullException(nameof(strategy));
        }

        _strategies.Add(strategy);
    }

    /// <summary>
    /// Gets the appropriate render strategy for the given VNode
    /// </summary>
    public IRenderStrategy GetStrategy(VNode node)
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
    public IReadOnlyList<IRenderStrategy> GetAllStrategies() => _strategies.AsReadOnly();

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