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

}