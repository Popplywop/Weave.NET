using Weave.Services;
using Weave.UI;
using Weave.UI.Renderer;

namespace Weave;

/// <summary>
/// Main application host that manages the component lifecycle, input, and rendering.
/// Supports dependency injection for improved testability and extensibility.
/// </summary>
public sealed class WeaveApp : IDisposable
{
    private readonly IAppServices _services;
    private readonly TerminalHost _terminal;
    private readonly VirtualScreen _screen;
    private readonly IFocusManager _focus;
    private readonly InputManager _input;
    private readonly ComponentContext _rootContext;
    private readonly LayoutEngine _layoutEngine;
    private readonly VNodeRenderer _renderer;

    private Component? _rootComponent;
    private bool _running;
    private bool _needsRerender = true;
    private bool _rendering = false;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Task? _inputTask;
    private bool _hasCancelKeyPressHandler = false;

    /// <summary>
    /// Creates a new WeaveApp with default production dependencies and default options.
    /// </summary>
    public WeaveApp() : this(WeaveAppOptions.Default, new AppServices())
    {
    }

    /// <summary>
    /// Creates a new WeaveApp with custom options and default production dependencies.
    /// </summary>
    public WeaveApp(WeaveAppOptions options) : this(options, new AppServices())
    {
    }

    /// <summary>
    /// Creates a new WeaveApp with injected dependencies for improved testability.
    /// Internal constructor for dependency injection scenarios.
    /// </summary>
    /// <param name="options">Configuration options for the application behavior</param>
    /// <param name="services">Services container with all required dependencies</param>
    internal WeaveApp(WeaveAppOptions options, IAppServices services)
    {
        ArgumentNullException.ThrowIfNull(options);
        _services = services ?? throw new ArgumentNullException(nameof(services));

        // Create instances using the injected services
        _terminal = _services.CreateTerminalHost();
        _screen = _services.CreateVirtualScreen(Console.WindowHeight, Console.WindowWidth);
        _focus = _services.FocusManager;
        _input = _services.CreateInputManager();
        _rootContext = _services.RootContext;
        _layoutEngine = _services.LayoutEngine;
        _renderer = _services.CreateRenderer(_screen);

        // Set up root context to trigger re-renders
        _rootContext.RequestRerender = () => _needsRerender = true;
        _rootContext.FocusManager = _focus;

        // Configure exit keybind based on options (default Ctrl+C)
        _input.Bind(options.ExitKey, options.ExitKeyMods, Stop);

        // Set up console cancel handler for Ctrl+C to ensure proper terminal restoration
        if (options.ExitKey == ConsoleKey.C && options.ExitKeyMods == KeyMods.Ctrl)
        {
            Console.CancelKeyPress += OnCancelKeyPress;
            _hasCancelKeyPressHandler = true;
        }
    }

    /// <summary>
    /// Handles Console.CancelKeyPress events (Ctrl+C) to ensure proper terminal restoration.
    /// </summary>
    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        // Cancel the default behavior (process termination)
        e.Cancel = true;

        // Trigger our normal shutdown process which includes terminal restoration
        Stop();
    }

    /// <summary>
    /// Runs the application with the specified root component.
    /// </summary>
    public void Run(Component rootComponent)
    {
        _rootComponent = rootComponent;
        _running = true;

        // Start dedicated input processing task for immediate responsiveness
        _inputTask = Task.Run(() => InputProcessingLoop(_cancellationTokenSource.Token));

        try
        {
            MainLoopAsync().Wait();
        }
        finally
        {
            _running = false;
        }
    }

    /// <summary>
    /// Stops the application immediately.
    /// </summary>
    public void Stop()
    {
        // Restore terminal immediately when ESC is pressed (like lazygit)
        TerminalHost.RestoreTerminal();

        _running = false;
        _cancellationTokenSource.Cancel(); // Immediate shutdown - interrupts Task.Delay instantly
    }

    /// <summary>
    /// Ultra-responsive input processing loop that eliminates all artificial delays.
    /// Uses busy polling for sub-millisecond ESC response time.
    /// </summary>
    private void InputProcessingLoop(CancellationToken cancellationToken)
    {
        try
        {
            while (_running && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Process input immediately as it becomes available
                    _input.Tick();

                    // Minimal CPU yield without sleep for ultra-low latency
                    // This allows other threads to run but doesn't introduce delay
                    if (!Console.KeyAvailable)
                    {
                        Thread.Yield(); // Cooperative yield, no forced delay
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected during shutdown
                    break;
                }
                catch
                {
                    // Ignore input errors during shutdown for faster exit
                    if (!_running || cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
        }
    }

    private async Task MainLoopAsync()
    {
        try
        {
            while (_running && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    // Handle terminal resize
                    var newHeight = Console.WindowHeight;
                    var newWidth = Console.WindowWidth;

                    if (newHeight != _screen.Rows || newWidth != _screen.Cols)
                    {
                        _screen.Resize(newHeight, newWidth);
                        _needsRerender = true;
                    }

                    // Input processing is now handled by dedicated InputProcessingLoop thread
                    // for immediate responsiveness - no frame delay for ESC or other keys

                    // IMMEDIATE exit check (ESC pressed via dedicated input thread)
                    if (!_running || _cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    // Render if needed (prevent overlapping renders)
                    if (_needsRerender && _rootComponent != null && !_rendering)
                    {
                        RenderFrame(_cancellationTokenSource.Token);
                        _needsRerender = false;
                    }

                    // Exit immediately if shutdown requested
                    if (!_running || _cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    // Run post-commit effects (skip during shutdown for instant exit)
                    if (_running && !_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var effects = _rootContext.PostCommitEffects.ToList();
                        _rootContext.PostCommitEffects.Clear();
                        foreach (var effect in effects)
                        {
                            effect();
                        }
                    }
                    else
                    {
                        // Clear effects without executing them during shutdown
                        _rootContext.PostCommitEffects.Clear();
                    }

                    // 16ms delay to lock ui at 60fps
                    await Task.Delay(16, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested - exit gracefully
                    break;
                }
                catch (Exception ex)
                {
                    // Log error and continue (for robustness)
                    Console.Error.WriteLine($"Error in main loop: {ex.Message}");
                    try
                    {
                        await Task.Delay(100, _cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown - exit gracefully
        }
    }

    private void RenderFrame(CancellationToken cancellationToken = default)
    {
        if (_rootComponent == null)
        {
            return;
        }

        _rendering = true;
        try
        {
            // Check for cancellation before starting render
            cancellationToken.ThrowIfCancellationRequested();

            // Reset component context for new render
            _rootContext.BeginRender();

            // Check for cancellation before expensive operations
            cancellationToken.ThrowIfCancellationRequested();

            // Render component tree to VNode tree
            var rootVNode = _rootComponent(_rootContext);

            // Check for cancellation before layout
            cancellationToken.ThrowIfCancellationRequested();

            // Calculate layout
            var layoutRoot = _layoutEngine.CalculateLayout(rootVNode, _screen.Cols, _screen.Rows);

            // Check for cancellation before rendering to screen
            cancellationToken.ThrowIfCancellationRequested();

            // Render to VirtualScreen
            _renderer.Render(layoutRoot);

            // Check for cancellation before terminal output
            cancellationToken.ThrowIfCancellationRequested();

            // Display to terminal (instant native output)
            _screen.Render();
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown - exit gracefully without error
            throw;
        }
        catch (Exception ex)
        {
            // Fallback: clear screen and show error (if not cancelled)
            if (!cancellationToken.IsCancellationRequested)
            {
                _screen.ClearVirtualAndInvalidate();
                _screen.Put(0, 0, $"Render Error: {ex.Message}");
                _screen.Render();
            }
        }
        finally
        {
            _rendering = false;
        }
    }

    public void Dispose()
    {
        _running = false;
        _cancellationTokenSource.Cancel();

        // Wait for input task to complete (with timeout for safety)
        _inputTask?.Wait(TimeSpan.FromMilliseconds(100));

        // Clean up console cancel handler if we set it up
        if (_hasCancelKeyPressHandler)
        {
            Console.CancelKeyPress -= OnCancelKeyPress;
            _hasCancelKeyPressHandler = false;
        }

        _cancellationTokenSource.Dispose();
        _terminal.Dispose();
    }
}