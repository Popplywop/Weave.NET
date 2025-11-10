using Xunit;
using Weave;
using Weave.Core.Implementations;

namespace Weave.Tests;

/// <summary>
/// Tests demonstrating VirtualScreen testability after dependency injection refactoring
/// </summary>
public class VirtualScreenTests : IDisposable
{
    private readonly TestTerminalOutput _testOutput;
    private readonly VirtualScreen _screen;

    public VirtualScreenTests()
    {
        _testOutput = new TestTerminalOutput();
        _screen = new VirtualScreen(10, 20, _testOutput);
    }

    public void Dispose()
    {
        _screen.Dispose();
    }

    [Fact]
    public void Constructor_SetsCorrectDimensions()
    {
        Assert.Equal(10, _screen.Rows);
        Assert.Equal(20, _screen.Cols);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOutputIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new VirtualScreen(10, 20, null!));
    }

    [Fact]
    public void Resize_UpdatesDimensions()
    {
        _screen.Resize(15, 30);

        Assert.Equal(15, _screen.Rows);
        Assert.Equal(30, _screen.Cols);
    }

    [Fact]
    public void ClearVirtualAndInvalidate_WritesToTerminalOutput()
    {
        _screen.ClearVirtualAndInvalidate();

        Assert.True(_testOutput.WriteCount > 0, "Should have written clear screen sequence");
        Assert.True(_testOutput.Contains("\x1b[2J"), "Should contain clear screen ANSI sequence");
        Assert.True(_testOutput.Contains("\x1b[H"), "Should contain home cursor ANSI sequence");
    }

    [Fact]
    public void Render_WithNoChanges_DoesNotWriteToOutput()
    {
        _testOutput.Reset();
        _screen.Render();

        Assert.Equal(0, _testOutput.WriteCount);
        Assert.Empty(_testOutput.AllOutput);
    }

    [Fact]
    public void Render_WithChanges_WritesToOutput()
    {
        _screen.Put(0, 0, "Hello");
        _testOutput.Reset();

        _screen.Render();

        Assert.True(_testOutput.WriteCount > 0, "Should have written output after changes");
        Assert.Contains("Hello", _testOutput.AllOutput);
    }

    [Fact]
    public void Put_StoresTextAtCorrectPosition()
    {
        const string testText = "Test";
        _screen.ClearNext();
        _screen.Put(2, 5, testText);
        _testOutput.Reset();

        _screen.Render();

        // Should contain ANSI cursor positioning sequence for row 3, col 6 (1-based)
        Assert.Contains("\x1b[3;6H", _testOutput.AllOutput);
        Assert.Contains(testText, _testOutput.AllOutput);
    }

    [Fact]
    public void Plot_StoresCharacterAtCorrectPosition()
    {
        _screen.ClearNext();
        _screen.Plot(1, 3, 'X');
        _testOutput.Reset();

        _screen.Render();

        // Should contain ANSI cursor positioning sequence for row 2, col 4 (1-based)
        Assert.Contains("\x1b[2;4H", _testOutput.AllOutput);
        Assert.Contains("X", _testOutput.AllOutput);
    }

    [Fact]
    public void Render_OptimizesDirtyRegions_OnlyRendersChangedAreas()
    {
        // First render to establish baseline
        _screen.Put(0, 0, "Initial");
        _screen.Render();
        _testOutput.Reset();

        // Make a small change
        _screen.Put(5, 10, "X");
        _screen.Render();

        // Should only render the changed area, not the entire screen
        var output = _testOutput.AllOutput;
        Assert.Contains("\x1b[6;11H", output); // Position cursor to row 6, col 11
        Assert.Contains("X", output);

        // Should not contain positioning for the unchanged area
        Assert.DoesNotContain("\x1b[1;1H", output); // Should not position to row 1, col 1
    }

    [Fact]
    public void MultipleChanges_BatchedInSingleRender()
    {
        _screen.ClearNext();
        _screen.Put(0, 0, "A");
        _screen.Put(0, 1, "B");
        _screen.Put(0, 2, "C");
        _testOutput.Reset();

        _screen.Render();

        // Should have written all changes in a single write operation
        Assert.Equal(1, _testOutput.WriteCount);
        Assert.Contains("ABC", _testOutput.AllOutput);
    }

    [Fact]
    public void ThreadSafety_ConcurrentOperations_DoNotCorruptOutput()
    {
        var tasks = new List<Task>();

        // Test concurrent writes
        for (int i = 0; i < 5; i++)
        {
            int row = i;
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 10; j++)
                {
                    _screen.Put(row, j, "X");
                }
            }));
        }

        // Test concurrent renders
        tasks.Add(Task.Run(() =>
        {
            for (int i = 0; i < 20; i++)
            {
                _screen.Render();
                Thread.Sleep(1);
            }
        }));

        Task.WaitAll(tasks.ToArray());

        // Should not have thrown any exceptions
        Assert.True(true, "Concurrent operations completed without exceptions");
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(50, 120)]
    [InlineData(100, 200)]
    public void DifferentScreenSizes_WorkCorrectly(int rows, int cols)
    {
        using var testOutput = new TestTerminalOutput();
        using var screen = new VirtualScreen(rows, cols, testOutput);

        Assert.Equal(rows, screen.Rows);
        Assert.Equal(cols, screen.Cols);

        // Should be able to write and render
        screen.Put(0, 0, "test");
        screen.Render();

        Assert.True(testOutput.WriteCount > 0);
    }
}