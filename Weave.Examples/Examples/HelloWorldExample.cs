using Weave;
using Weave.UI;

using static Weave.UI.Elements;

namespace Examples;

/// <summary>
/// Simple Hello World example demonstrating basic Weave usage.
/// </summary>
public static class HelloWorldExample
{
    public static VNode HelloWorld(ComponentContext ctx, object? props = null)
    {
        var (counter, setCounter) = ctx.UseState(0);

        return new VBorder(
            new BorderStyle(),
            Col(
                Text("🌟 Welcome to Weave.NET! 🌟", align: Align.Center),
                Text(""),
                Text($"Button pressed: {counter} times", align: Align.Center),
                Text(""),
                Row(
                    Spacer(),
                    Button.Create(ctx, new ButtonProps(
                        Label: "Click me!",
                        OnPress: () => setCounter(counter + 1)
                        // Uses default keys: Enter and Space
                    )),
                    Spacer()
                ),
                Text(""),
                Row(
                    Spacer(),
                    Button.Create(ctx, new ButtonProps(
                        Label: "Shift+C = +5!",
                        OnPress: () => setCounter(counter + 5),
                        ActivationKeys: [new ButtonActivationKey(ConsoleKey.C, KeyMods.Shift)]
                    )),
                    Spacer()
                ),
                Text(""),
                Row(
                    Spacer(),
                    Button.Create(ctx, new ButtonProps(
                        Label: "Ctrl+R = Reset",
                        OnPress: () => setCounter(0),
                        ActivationKeys: [new ButtonActivationKey(ConsoleKey.R, KeyMods.Ctrl)]
                    )),
                    Spacer()
                ),
                Text(""),
                Text("Tab to navigate, Enter/Space for default, Shift+C for +5, Ctrl+R to reset, Ctrl+C to exit", align: Align.Center)
            )
        );
    }

    /// <summary>
    /// Runs the example with default options (Ctrl+C to exit).
    /// </summary>
    public static void Run()
    {
        App.Run(HelloWorld);
    }

    /// <summary>
    /// Runs the example with ESC key configured as the exit keybind (legacy behavior).
    /// </summary>
    public static void RunWithEscapeExit()
    {
        App.Run(HelloWorld, new WeaveAppOptions { ExitKey = ConsoleKey.Escape, ExitKeyMods = KeyMods.None });
    }

    /// <summary>
    /// Runs the example with a custom exit keybind (Alt+Q).
    /// </summary>
    public static void RunWithCustomExit()
    {
        var options = new WeaveAppOptions
        {
            ExitKey = ConsoleKey.Q,
            ExitKeyMods = KeyMods.Alt
        };
        App.Run(HelloWorld, options);
    }
}