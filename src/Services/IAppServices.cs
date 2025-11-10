using Weave.Core.Abstractions;
using Weave.UI;
using Weave.UI.Renderer;

namespace Weave.Services;

/// <summary>
/// Interface that encapsulates all services required by WeaveApp
/// </summary>
internal interface IAppServices
{
    /// <summary>
    /// Terminal initialization service
    /// </summary>
    ITerminalInitializer TerminalInitializer { get; }

    /// <summary>
    /// Terminal output service
    /// </summary>
    ITerminalOutput TerminalOutput { get; }

    /// <summary>
    /// Input source for reading user input
    /// </summary>
    IInputSource InputSource { get; }

    /// <summary>
    /// Focus management service
    /// </summary>
    IFocusManager FocusManager { get; }

    /// <summary>
    /// Root component context
    /// </summary>
    ComponentContext RootContext { get; }

    /// <summary>
    /// Layout calculation engine
    /// </summary>
    LayoutEngine LayoutEngine { get; }

    /// <summary>
    /// Creates a VirtualScreen with the specified dimensions
    /// </summary>
    VirtualScreen CreateVirtualScreen(int height, int width);

    /// <summary>
    /// Creates a VNodeRenderer for the specified screen
    /// </summary>
    VNodeRenderer CreateRenderer(VirtualScreen screen);

    /// <summary>
    /// Creates a TerminalHost with the configured initializer
    /// </summary>
    TerminalHost CreateTerminalHost();

    /// <summary>
    /// Creates an InputManager with the configured focus manager and input source
    /// </summary>
    InputManager CreateInputManager();
}