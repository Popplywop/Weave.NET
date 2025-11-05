namespace Weave.UI;

public abstract record VNode(string? Key = null);

// Delegate for function components. You can extend later with typed props, etc.
public delegate VNode Component(ComponentContext ctx, object? props = null);