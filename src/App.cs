using Weave.UI;

namespace Weave;

/// <summary>
/// Static helper for simple app creation.
/// </summary>
public static class App
{
    /// <summary>
    /// Creates and runs a Weave application with the specified root component and default options.
    /// Uses Ctrl+C as the exit keybind by default.
    /// </summary>
    public static void Run(Component rootComponent)
    {
        using var app = new WeaveApp();
        app.Run(rootComponent);
    }

    /// <summary>
    /// Creates and runs a Weave application with the specified root component and custom options.
    /// </summary>
    /// <param name="rootComponent">The root component to render</param>
    /// <param name="options">Configuration options for the application behavior</param>
    public static void Run(Component rootComponent, WeaveAppOptions options)
    {
        using var app = new WeaveApp(options);
        app.Run(rootComponent);
    }
}