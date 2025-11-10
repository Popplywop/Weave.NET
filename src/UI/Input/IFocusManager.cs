namespace Weave.UI;

/// <summary>
/// Interface for focus management operations, supporting Interface Segregation Principle
/// </summary>
public interface IFocusManager
{
    /// <summary>
    /// Gets the currently focused node ID, or null if no node is focused
    /// </summary>
    FocusManager.NodeId? Focused { get; }

    /// <summary>
    /// Registers a component for focus management
    /// </summary>
    /// <param name="id">Unique identifier for the component</param>
    /// <param name="handler">Input handler for the component</param>
    /// <param name="focusable">Whether the component can receive focus</param>
    void Register(FocusManager.NodeId id, IInputHandler handler, bool focusable = true);

    /// <summary>
    /// Unregisters a component from focus management
    /// </summary>
    /// <param name="id">Unique identifier for the component</param>
    void Unregister(FocusManager.NodeId id);

    /// <summary>
    /// Attempts to get the input handler for a registered component
    /// </summary>
    /// <param name="id">Unique identifier for the component</param>
    /// <param name="handler">The handler if found</param>
    /// <returns>True if handler was found, false otherwise</returns>
    bool TryGetHandler(FocusManager.NodeId id, out IInputHandler handler);

    /// <summary>
    /// Moves focus to the next focusable component
    /// </summary>
    void FocusNext();

    /// <summary>
    /// Moves focus to the previous focusable component
    /// </summary>
    void FocusPrev();

    /// <summary>
    /// Dispatches input event to the focused component and optionally to global handlers
    /// </summary>
    /// <param name="e">The input event to dispatch</param>
    /// <param name="globalHandlers">Optional global handlers to try if focused component doesn't handle the event</param>
    /// <returns>True if the event was handled, false otherwise</returns>
    bool DispatchToFocused(InputEvent e, IEnumerable<IInputHandler>? globalHandlers = null);
}