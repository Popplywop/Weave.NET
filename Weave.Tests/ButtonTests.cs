using Xunit;
using Weave.UI;

namespace Weave.Tests;

public class ButtonTests
{
    [Fact]
    public void Button_RendersCorrectly()
    {
        var context = new ComponentContext();
        var focusManager = new FocusManager();
        string buttonText = "Click Me";
        bool clicked = false;

        var props = new ButtonProps(buttonText, () => clicked = true);
        var vnode = Button.Create(context, focusManager, props);

        Assert.NotNull(vnode);
        Assert.Contains(buttonText, vnode.Key ?? string.Empty);
    }

    [Fact]
    public void Button_WithEmptyText_RendersEmpty()
    {
        var context = new ComponentContext();
        var focusManager = new FocusManager();
        bool clicked = false;

        var props = new ButtonProps("", () => clicked = true);
        var vnode = Button.Create(context, focusManager, props);

        Assert.NotNull(vnode);
    }

    [Fact]
    public void Button_ClickHandler_CreatesVNode()
    {
        var context = new ComponentContext();
        var focusManager = new FocusManager();
        int clickCount = 0;

        var props = new ButtonProps("Counter", () => clickCount++);
        var vnode = Button.Create(context, focusManager, props);

        // Verify the button creates a proper VNode
        Assert.NotNull(vnode);
        Assert.Equal(0, clickCount); // Not clicked yet
    }

    [Fact]
    public void Button_MultipleInstances_IndependentState()
    {
        var context1 = new ComponentContext();
        var context2 = new ComponentContext();
        var focusManager1 = new FocusManager();
        var focusManager2 = new FocusManager();

        int button1Clicks = 0;
        int button2Clicks = 0;

        var props1 = new ButtonProps("Button 1", () => button1Clicks++);
        var props2 = new ButtonProps("Button 2", () => button2Clicks++);

        var vnode1 = Button.Create(context1, focusManager1, props1);
        var vnode2 = Button.Create(context2, focusManager2, props2);

        Assert.NotNull(vnode1);
        Assert.NotNull(vnode2);
        Assert.NotEqual(vnode1.Key, vnode2.Key);
    }

    [Theory]
    [InlineData("OK")]
    [InlineData("Cancel")]
    [InlineData("Save Changes")]
    [InlineData("Delete Item")]
    [InlineData("A very long button text that might wrap")]
    public void Button_WithVariousTexts_RendersCorrectly(string buttonText)
    {
        var context = new ComponentContext();
        var focusManager = new FocusManager();
        bool clicked = false;

        var props = new ButtonProps(buttonText, () => clicked = true);
        var vnode = Button.Create(context, focusManager, props);

        Assert.NotNull(vnode);
        Assert.Contains(buttonText, vnode.Key ?? string.Empty);
    }

    [Fact]
    public void Button_ConsistentRendering()
    {
        var context = new ComponentContext();
        var focusManager = new FocusManager();
        bool clicked = false;

        var props = new ButtonProps("Test Button", () => clicked = true);

        // Test multiple renders for consistency
        var vnode1 = Button.Create(context, focusManager, props);
        var vnode2 = Button.Create(context, focusManager, props);

        Assert.NotNull(vnode1);
        Assert.NotNull(vnode2);
        Assert.Equal(vnode1.Key, vnode2.Key);
    }

    [Fact]
    public void ButtonInputHandler_RespondsToCustomActivationKeys()
    {
        // Arrange
        bool activated = false;
        var customKeys = new[]
        {
            new ButtonActivationKey(ConsoleKey.A, KeyMods.Ctrl),
            new ButtonActivationKey(ConsoleKey.B, KeyMods.Alt)
        };
        var handler = new ButtonInputHandler(() => false, () => activated = true, customKeys);

        // Act & Assert - Custom keys should activate
        var ctrlA = new KeyEvent(ConsoleKey.A, KeyMods.Ctrl);
        var result1 = handler.OnInput(ctrlA);
        Assert.True(result1);
        Assert.True(activated);

        // Reset and test second key
        activated = false;
        var altB = new KeyEvent(ConsoleKey.B, KeyMods.Alt);
        var result2 = handler.OnInput(altB);
        Assert.True(result2);
        Assert.True(activated);

        // Reset and test default keys should NOT work
        activated = false;
        var enter = new KeyEvent(ConsoleKey.Enter, KeyMods.None);
        var result3 = handler.OnInput(enter);
        Assert.False(result3);
        Assert.False(activated);

        var space = new KeyEvent(ConsoleKey.Spacebar, KeyMods.None);
        var result4 = handler.OnInput(space);
        Assert.False(result4);
        Assert.False(activated);
    }

    [Fact]
    public void ButtonInputHandler_DefaultActivationKeys()
    {
        // Arrange
        bool activated = false;
        var handler = new ButtonInputHandler(() => false, () => activated = true);

        // Act & Assert - Default keys (Enter and Space) should activate
        var enter = new KeyEvent(ConsoleKey.Enter, KeyMods.None);
        var result1 = handler.OnInput(enter);
        Assert.True(result1);
        Assert.True(activated);

        // Reset and test space
        activated = false;
        var space = new KeyEvent(ConsoleKey.Spacebar, KeyMods.None);
        var result2 = handler.OnInput(space);
        Assert.True(result2);
        Assert.True(activated);

        // Test that other keys don't activate
        activated = false;
        var randomKey = new KeyEvent(ConsoleKey.A, KeyMods.None);
        var result3 = handler.OnInput(randomKey);
        Assert.False(result3);
        Assert.False(activated);
    }

    [Fact]
    public void ButtonInputHandler_AltC_SpecificTest()
    {
        // Arrange - Test the exact Alt+C combination used in the example
        bool activated = false;
        var altCKey = new ButtonActivationKey(ConsoleKey.C, KeyMods.Alt);
        var handler = new ButtonInputHandler(() => false, () => activated = true, [altCKey]);

        // Act
        var altCEvent = new KeyEvent(ConsoleKey.C, KeyMods.Alt);
        var result = handler.OnInput(altCEvent);

        // Assert
        Assert.True(result, "Alt+C should activate the button handler");
        Assert.True(activated, "Button action should have been called");
    }

    [Fact]
    public void ButtonInputHandler_ShiftC_SpecificTest()
    {
        // Arrange - Test Shift+C combination as used in the updated example
        bool activated = false;
        var shiftCKey = new ButtonActivationKey(ConsoleKey.C, KeyMods.Shift);
        var handler = new ButtonInputHandler(() => false, () => activated = true, [shiftCKey]);

        // Act
        var shiftCEvent = new KeyEvent(ConsoleKey.C, KeyMods.Shift);
        var result = handler.OnInput(shiftCEvent);

        // Assert
        Assert.True(result, "Shift+C should activate the button handler");
        Assert.True(activated, "Button action should have been called");
    }

    [Fact]
    public void ButtonCustomKeybind_WorksThoughInputManager()
    {
        // Integration test: Verify custom button keybinds work through the full input pipeline
        var focusManager = new FocusManager();
        var inputSource = new TestInputSource();
        var inputManager = new InputManager(focusManager, inputSource);

        // Set up global keybind (like app exit) to verify it doesn't interfere
        bool globalKeybindCalled = false;
        inputManager.Bind(ConsoleKey.Q, KeyMods.Ctrl, () => globalKeybindCalled = true);

        // Create button with custom keybind
        bool buttonActivated = false;
        var customKeys = new[] { new ButtonActivationKey(ConsoleKey.R, KeyMods.Ctrl) };
        var buttonHandler = new ButtonInputHandler(() => false, () => buttonActivated = true, customKeys);

        var buttonId = FocusManager.NodeId.New();
        focusManager.Register(buttonId, buttonHandler, focusable: true);

        // Simulate Ctrl+R key press
        inputSource.QueueKey(new KeyEvent(ConsoleKey.R, KeyMods.Ctrl));
        inputManager.Tick();

        // Verify button handled the key (not global keybind)
        Assert.True(buttonActivated, "Button with custom Ctrl+R keybind should be activated");
        Assert.False(globalKeybindCalled, "Global keybind should not be called when button handles key");

        // Test fallback: global keybind should work when no button handles the key
        buttonActivated = false;
        inputSource.QueueKey(new KeyEvent(ConsoleKey.Q, KeyMods.Ctrl));
        inputManager.Tick();

        Assert.False(buttonActivated, "Button should not be activated by different key");
        Assert.True(globalKeybindCalled, "Global keybind should work when button doesn't handle key");
    }

    // Test utility class for simulating input events
    private class TestInputSource : IInputSource
    {
        private readonly Queue<KeyEvent> _keyQueue = new();

        public void QueueKey(KeyEvent keyEvent)
        {
            _keyQueue.Enqueue(keyEvent);
        }

        public KeyEvent? TryReadKey()
        {
            return _keyQueue.TryDequeue(out var key) ? key : null;
        }
    }

}