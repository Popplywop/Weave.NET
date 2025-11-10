using Weave.Core;
using Weave.Core.Abstractions;
using Weave.Structs;

namespace Weave;

/// <summary>
/// Virtual screen that manages screen buffers and coordinates rendering.
/// Now follows Single Responsibility Principle with extracted specialized classes.
/// </summary>
public sealed class VirtualScreen : IDisposable
{
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly DirtyRegionTracker _dirtyTracker = new();
    private readonly ScreenRenderer _renderer;
    private Cell[,] _prev = new Cell[0, 0];
    private Cell[,] _next = new Cell[0, 0];

    public VirtualScreen(int rows, int cols, ITerminalOutput output)
    {
        _renderer = new ScreenRenderer(output ?? throw new ArgumentNullException(nameof(output)));
        Resize(rows, cols);
    }

    public int Rows { get; private set; }
    public int Cols { get; private set; }

    public void Resize(int rows, int cols)
    {
        _lock.EnterWriteLock();
        try
        {
            Rows = rows;
            Cols = cols;
            _prev = new Cell[rows, cols];
            _next = new Cell[rows, cols];
            _dirtyTracker.Resize(rows, cols);
            ClearNext_Nolock();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void ClearNext()
    {
        _lock.EnterWriteLock();
        try
        {
            ClearNext_Nolock();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void ClearNext_Nolock()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                _next[r, c] = Cell.Blank;
            }
        }
        _dirtyTracker.ClearAll();
    }

    public void Put(int row, int col, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        _lock.EnterWriteLock();
        try
        {
            if (row < 0 || row >= Rows)
            {
                return;
            }

            for (int i = 0; i < text.Length; i++)
            {
                int cc = col + i;
                if (cc >= 0 && cc < Cols)
                {
                    _next[row, cc].Ch = text[i];
                    _dirtyTracker.MarkDirty(row, cc, Rows, Cols);
                }
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Plot(int row, int col, char ch)
    {
        _lock.EnterWriteLock();
        try
        {
            if (row >= 0 && row < Rows && col >= 0 && col < Cols)
            {
                _next[row, col].Ch = ch;
                _dirtyTracker.MarkDirty(row, col, Rows, Cols);
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void ClearVirtualAndInvalidate()
    {
        _lock.EnterWriteLock();
        try
        {
            _renderer.ClearScreen();
            ClearNext_Nolock();
            _prev = new Cell[Rows, Cols]; // force full repaint

            // Mark entire screen as dirty for full repaint
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    _dirtyTracker.MarkDirty(r, c, Rows, Cols);
                }
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Render()
    {
        if (!_dirtyTracker.HasDirtyRegions)
        {
            return; // Nothing to render
        }

        _lock.EnterReadLock();
        Cell[,] prevSnapshot, nextSnapshot;
        try
        {
            // Create snapshots for thread safety
            prevSnapshot = _prev;
            nextSnapshot = _next;
        }
        finally
        {
            _lock.ExitReadLock();
        }

        // Delegate rendering to specialized renderer
        _renderer.RenderChanges(prevSnapshot, nextSnapshot, _dirtyTracker, Rows, Cols);

        _lock.EnterWriteLock();
        try
        {
            // Swap arrays (O(1) operation)
            (_prev, _next) = (_next, _prev);

            // Clear dirty regions
            _dirtyTracker.ClearDirtyRegion(
                Math.Max(0, _dirtyTracker.MinDirtyRow),
                Math.Min(Rows - 1, _dirtyTracker.MaxDirtyRow),
                Math.Max(0, _dirtyTracker.MinDirtyCol),
                Math.Min(Cols - 1, _dirtyTracker.MaxDirtyCol)
            );

            // Clear next frame buffer
            for (int r = Math.Max(0, _dirtyTracker.MinDirtyRow); r <= Math.Min(Rows - 1, _dirtyTracker.MaxDirtyRow); r++)
            {
                for (int c = Math.Max(0, _dirtyTracker.MinDirtyCol); c <= Math.Min(Cols - 1, _dirtyTracker.MaxDirtyCol); c++)
                {
                    _next[r, c] = Cell.Blank;
                }
            }

            _dirtyTracker.ResetBounds();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void Dispose()
    {
        _lock.Dispose();
    }
}