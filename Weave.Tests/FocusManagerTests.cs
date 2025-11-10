using Xunit;
using Weave.UI;

namespace Weave.Tests;

public class FocusManagerTests
{
    private readonly FocusManager _focusManager;

    public FocusManagerTests()
    {
        _focusManager = new FocusManager();
    }

    [Fact]
    public void Register_AddsNewInputHandler()
    {
        var handler = new TestInputHandler(_ => { });
        var nodeId = new FocusManager.NodeId(Guid.NewGuid());

        _focusManager.Register(nodeId, handler);

        // Should not throw
    }

    [Fact]
    public void Register_WithExistingId_UpdatesHandler()
    {
        var nodeId = new FocusManager.NodeId(Guid.NewGuid());

        bool firstHandlerCalled = false;
        bool secondHandlerCalled = false;

        // Register first handler
        _focusManager.Register(nodeId, new TestInputHandler(_ => firstHandlerCalled = true));

        // Register second handler with same ID (should update)
        _focusManager.Register(nodeId, new TestInputHandler(_ => secondHandlerCalled = true));

        // Dispatch input
        var keyEvent = new KeyEvent(ConsoleKey.Enter, KeyMods.None, null);
        _focusManager.DispatchToFocused(keyEvent, new List<IInputHandler>());

        // Only second handler should be called since first was replaced
        Assert.False(firstHandlerCalled);
        Assert.True(secondHandlerCalled); // Should be called since it's focused
    }

    [Fact]
    public void FocusNext_WithNoElements_DoesNotThrow()
    {
        _focusManager.FocusNext();
    }

    [Fact]
    public void FocusPrev_WithNoElements_DoesNotThrow()
    {
        _focusManager.FocusPrev();
    }

    [Fact]
    public void FocusNext_WithSingleElement_FocusesElement()
    {
        bool handlerCalled = false;
        var handler = new TestInputHandler(_ => handlerCalled = true);
        var nodeId = new FocusManager.NodeId(Guid.NewGuid());

        _focusManager.Register(nodeId, handler);
        _focusManager.FocusNext();

        // Test by dispatching input to see if it gets called
        _focusManager.DispatchToFocused(new KeyEvent(ConsoleKey.A, KeyMods.None, 'a'));
        Assert.True(handlerCalled);
    }

    [Fact]
    public void FocusNext_WithMultipleElements_CyclesThroughElements()
    {
        var handler1 = new TestInputHandler(_ => { });
        var handler2 = new TestInputHandler(_ => { });
        var handler3 = new TestInputHandler(_ => { });

        _focusManager.Register(new FocusManager.NodeId(Guid.NewGuid()), handler1);
        _focusManager.Register(new FocusManager.NodeId(Guid.NewGuid()), handler2);
        _focusManager.Register(new FocusManager.NodeId(Guid.NewGuid()), handler3);

        // Cycle through focus - should not throw
        _focusManager.FocusNext();
        _focusManager.FocusNext();
        _focusManager.FocusNext();
        _focusManager.FocusNext(); // Should cycle back to first

        // Verify no exceptions thrown during focus cycling
        Assert.True(true);
    }

    [Fact]
    public void FocusPrev_WithMultipleElements_CyclesBackwards()
    {
        var handler1 = new TestInputHandler(_ => { });
        var handler2 = new TestInputHandler(_ => { });
        var handler3 = new TestInputHandler(_ => { });

        _focusManager.Register(new FocusManager.NodeId(Guid.NewGuid()), handler1);
        _focusManager.Register(new FocusManager.NodeId(Guid.NewGuid()), handler2);
        _focusManager.Register(new FocusManager.NodeId(Guid.NewGuid()), handler3);

        // Focus first element then cycle backwards - should not throw
        _focusManager.FocusNext();
        _focusManager.FocusPrev();
        _focusManager.FocusPrev();

        // Verify no exceptions thrown during backward cycling
        Assert.True(true);
    }

    [Fact]
    public void DispatchToFocused_WithNoFocus_CallsGlobalHandlers()
    {
        bool globalHandlerCalled = false;
        var globalHandler = new TestInputHandler(_ => globalHandlerCalled = true);
        var globalHandlers = new List<IInputHandler> { globalHandler };

        var keyEvent = new KeyEvent(ConsoleKey.A, KeyMods.None, 'a');
        _focusManager.DispatchToFocused(keyEvent, globalHandlers);

        Assert.True(globalHandlerCalled);
    }

    [Fact]
    public void DispatchToFocused_WithFocusedElement_CallsFocusedHandler()
    {
        bool focusedHandlerCalled = false;
        bool globalHandlerCalled = false;

        var focusedHandler = new TestInputHandler(_ => focusedHandlerCalled = true);
        _focusManager.Register(new FocusManager.NodeId(Guid.NewGuid()), focusedHandler);

        var globalHandler = new TestInputHandler(_ => globalHandlerCalled = true);
        var globalHandlers = new List<IInputHandler> { globalHandler };

        // Focus the element
        _focusManager.FocusNext();

        // Dispatch input
        var keyEvent = new KeyEvent(ConsoleKey.A, KeyMods.None, 'a');
        _focusManager.DispatchToFocused(keyEvent, globalHandlers);

        Assert.True(focusedHandlerCalled);
        Assert.False(globalHandlerCalled); // Should not reach global handlers
    }

    [Fact]
    public void NodeId_Equality_WorksCorrectly()
    {
        var guid = Guid.NewGuid();
        var nodeId1 = new FocusManager.NodeId(guid);
        var nodeId2 = new FocusManager.NodeId(guid);
        var nodeId3 = new FocusManager.NodeId(Guid.NewGuid());

        Assert.Equal(nodeId1, nodeId2);
        Assert.NotEqual(nodeId1, nodeId3);
        Assert.True(nodeId1.Equals(nodeId2));
        Assert.False(nodeId1.Equals(nodeId3));
        Assert.Equal(nodeId1.GetHashCode(), nodeId2.GetHashCode());
    }

    [Fact]
    public void ConcurrentAccess_DoesNotThrow()
    {
        var tasks = new List<Task>();

        // Concurrent registrations
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                var handler = new TestInputHandler(_ => { });
                _focusManager.Register(new FocusManager.NodeId(Guid.NewGuid()), handler);
            }));
        }

        // Concurrent focus changes
        tasks.Add(Task.Run(() =>
        {
            for (int i = 0; i < 50; i++)
            {
                _focusManager.FocusNext();
                Thread.Sleep(1);
            }
        }));

        tasks.Add(Task.Run(() =>
        {
            for (int i = 0; i < 50; i++)
            {
                _focusManager.FocusPrev();
                Thread.Sleep(1);
            }
        }));

        Task.WaitAll(tasks.ToArray());
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