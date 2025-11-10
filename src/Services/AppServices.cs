using Weave.Core.Abstractions;
using Weave.Core.Implementations;
using Weave.UI;
using Weave.UI.Renderer;

namespace Weave.Services;

/// <summary>
/// Default implementation of WeaveApp services using production dependencies
/// </summary>
internal sealed class AppServices : IAppServices
{
    public ITerminalInitializer TerminalInitializer { get; }
    public ITerminalOutput TerminalOutput { get; }
    public IInputSource InputSource { get; }
    public IFocusManager FocusManager { get; }
    public ComponentContext RootContext { get; }
    public LayoutEngine LayoutEngine { get; }

    public AppServices()
        : this(
            new NativeTerminalInitializer(),
            new NativeTerminalOutput(),
            new ConsoleInputSource(),
            new FocusManager(),
            new ComponentContext(),
            new LayoutEngine())
    {
    }

    public AppServices(
        ITerminalInitializer terminalInitializer,
        ITerminalOutput terminalOutput,
        IInputSource inputSource,
        IFocusManager focusManager,
        ComponentContext rootContext,
        LayoutEngine layoutEngine)
    {
        TerminalInitializer = terminalInitializer ?? throw new ArgumentNullException(nameof(terminalInitializer));
        TerminalOutput = terminalOutput ?? throw new ArgumentNullException(nameof(terminalOutput));
        InputSource = inputSource ?? throw new ArgumentNullException(nameof(inputSource));
        FocusManager = focusManager ?? throw new ArgumentNullException(nameof(focusManager));
        RootContext = rootContext ?? throw new ArgumentNullException(nameof(rootContext));
        LayoutEngine = layoutEngine ?? throw new ArgumentNullException(nameof(layoutEngine));
    }

    public VirtualScreen CreateVirtualScreen(int height, int width)
    {
        return new VirtualScreen(height, width, TerminalOutput);
    }

    public VNodeRenderer CreateRenderer(VirtualScreen screen)
    {
        return new VNodeRenderer(screen);
    }

    public TerminalHost CreateTerminalHost()
    {
        return new TerminalHost(TerminalInitializer);
    }

    public InputManager CreateInputManager()
    {
        return new InputManager(FocusManager, InputSource);
    }
}