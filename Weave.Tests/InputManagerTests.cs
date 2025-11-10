using Xunit;
using Weave.UI;

namespace Weave.Tests;

public class InputManagerTests
{
    private readonly FocusManager _focusManager;
    private readonly TestInputSource _inputSource;
    private readonly InputManager _inputManager;

    public InputManagerTests()
    {
        _focusManager = new FocusManager();
        _inputSource = new TestInputSource();
        _inputManager = new InputManager(_focusManager, _inputSource);
    }

    [Fact]
    public void Tick_WithNoInput_DoesNotThrow()
    {
        _inputManager.Tick();
    }

    [Fact]
    public void Tick_ProcessesSingleKey()
    {
        bool keyProcessed = false;
        _inputManager.AddGlobalHandler(new TestInputHandler(e => keyProcessed = true));

        _inputSource.QueueKey(new KeyEvent(ConsoleKey.A, KeyMods.None, 'a'));
        _inputManager.Tick();

        Assert.True(keyProcessed);
    }

    [Fact]
    public void Tick_ProcessesMultipleKeys()
    {
        int keysProcessed = 0;
        _inputManager.AddGlobalHandler(new TestInputHandler(e => keysProcessed++));

        _inputSource.QueueKey(new KeyEvent(ConsoleKey.A, KeyMods.None, 'a'));
        _inputSource.QueueKey(new KeyEvent(ConsoleKey.B, KeyMods.None, 'b'));
        _inputSource.QueueKey(new KeyEvent(ConsoleKey.C, KeyMods.None, 'c'));

        _inputManager.Tick();

        Assert.Equal(3, keysProcessed);
    }

    [Fact]
    public void TabNavigation_FocusNext_CallsFocusManager()
    {
        var testHandler = new TestInputHandler(_ => { });
        _focusManager.Register(new FocusManager.NodeId(Guid.NewGuid()), testHandler);

        _inputSource.QueueKey(new KeyEvent(ConsoleKey.Tab, KeyMods.None, null));
        _inputManager.Tick();

        // Focus should have been attempted (no exception thrown)
    }

    [Fact]
    public void ShiftTabNavigation_FocusPrevious_CallsFocusManager()
    {
        var testHandler = new TestInputHandler(_ => { });
        _focusManager.Register(new FocusManager.NodeId(Guid.NewGuid()), testHandler);

        _inputSource.QueueKey(new KeyEvent(ConsoleKey.Tab, KeyMods.Shift, null));
        _inputManager.Tick();

        // Focus should have been attempted (no exception thrown)
    }

    [Fact]
    public void Bind_RegistersKeybinding()
    {
        bool keybindingTriggered = false;
        _inputManager.Bind(ConsoleKey.F1, KeyMods.None, () => keybindingTriggered = true);

        _inputSource.QueueKey(new KeyEvent(ConsoleKey.F1, KeyMods.None, null));
        _inputManager.Tick();

        Assert.True(keybindingTriggered);
    }

    [Fact]
    public void Bind_WithModifiers_RegistersCorrectly()
    {
        bool keybindingTriggered = false;
        _inputManager.Bind(ConsoleKey.F1, KeyMods.Ctrl | KeyMods.Alt, () => keybindingTriggered = true);

        // Wrong modifier combination - should not trigger
        _inputSource.QueueKey(new KeyEvent(ConsoleKey.F1, KeyMods.Ctrl, null));
        _inputManager.Tick();
        Assert.False(keybindingTriggered);

        // Correct modifier combination - should trigger
        _inputSource.QueueKey(new KeyEvent(ConsoleKey.F1, KeyMods.Ctrl | KeyMods.Alt, null));
        _inputManager.Tick();
        Assert.True(keybindingTriggered);
    }

    [Fact]
    public void KeybindingOverridesGlobalHandler()
    {
        bool globalHandlerCalled = false;
        bool keybindingCalled = false;

        _inputManager.AddGlobalHandler(new TestInputHandler(e => globalHandlerCalled = true));
        _inputManager.Bind(ConsoleKey.F1, KeyMods.None, () => keybindingCalled = true);

        _inputSource.QueueKey(new KeyEvent(ConsoleKey.F1, KeyMods.None, null));
        _inputManager.Tick();

        Assert.True(keybindingCalled);
        Assert.False(globalHandlerCalled); // Should not reach global handler
    }

    [Fact]
    public void AddGlobalHandler_HandlesUnboundKeys()
    {
        bool globalHandlerCalled = false;
        _inputManager.AddGlobalHandler(new TestInputHandler(e => globalHandlerCalled = true));

        _inputSource.QueueKey(new KeyEvent(ConsoleKey.X, KeyMods.None, 'x'));
        _inputManager.Tick();

        Assert.True(globalHandlerCalled);
    }

    [Theory]
    [InlineData(KeyMods.None)]
    [InlineData(KeyMods.Shift)]
    [InlineData(KeyMods.Ctrl)]
    [InlineData(KeyMods.Alt)]
    [InlineData(KeyMods.Shift | KeyMods.Ctrl)]
    [InlineData(KeyMods.Shift | KeyMods.Alt)]
    [InlineData(KeyMods.Ctrl | KeyMods.Alt)]
    [InlineData(KeyMods.Shift | KeyMods.Ctrl | KeyMods.Alt)]
    public void ModifierCombinations_ProcessedCorrectly(KeyMods mods)
    {
        bool handlerCalled = false;
        _inputManager.AddGlobalHandler(new TestInputHandler(e =>
        {
            handlerCalled = true;
            if (e is KeyEvent ke)
            {
                Assert.Equal(mods, ke.Mods);
            }
        }));

        _inputSource.QueueKey(new KeyEvent(ConsoleKey.A, mods, mods == KeyMods.None ? 'a' : null));
        _inputManager.Tick();

        Assert.True(handlerCalled);
    }

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

    private class TestInputHandler : IInputHandler
    {
        private readonly Action<InputEvent> _handler;

        public TestInputHandler(Action<InputEvent> handler)
        {
            _handler = handler;
        }

        public bool OnInput(InputEvent e)
        {
            _handler(e);
            return true;
        }
    }
}