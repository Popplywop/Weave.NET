namespace Weave.Core;

/// <summary>
/// Tracks which regions of the screen need to be re-rendered for optimal performance
/// </summary>
internal sealed class DirtyRegionTracker
{
    private bool[,] _dirty = new bool[0, 0];

    public int MinDirtyRow { get; private set; } = int.MaxValue;
    public int MaxDirtyRow { get; private set; } = int.MinValue;
    public int MinDirtyCol { get; private set; } = int.MaxValue;
    public int MaxDirtyCol { get; private set; } = int.MinValue;

    public bool HasDirtyRegions => MinDirtyRow != int.MaxValue;

    public void Resize(int rows, int cols)
    {
        _dirty = new bool[rows, cols];
        ResetBounds();
    }

    public void MarkDirty(int row, int col, int rows, int cols)
    {
        if (row >= 0 && row < rows && col >= 0 && col < cols)
        {
            _dirty[row, col] = true;
            MinDirtyRow = Math.Min(MinDirtyRow, row);
            MaxDirtyRow = Math.Max(MaxDirtyRow, row);
            MinDirtyCol = Math.Min(MinDirtyCol, col);
            MaxDirtyCol = Math.Max(MaxDirtyCol, col);
        }
    }

    public bool IsDirty(int row, int col) => _dirty[row, col];

    public void ClearDirtyRegion(int startRow, int endRow, int startCol, int endCol)
    {
        for (int r = startRow; r <= endRow; r++)
        {
            for (int c = startCol; c <= endCol; c++)
            {
                _dirty[r, c] = false;
            }
        }
    }

    public void ResetBounds()
    {
        MinDirtyRow = int.MaxValue;
        MaxDirtyRow = int.MinValue;
        MinDirtyCol = int.MaxValue;
        MaxDirtyCol = int.MinValue;
    }

    public void ClearAll()
    {
        Array.Clear(_dirty, 0, _dirty.Length);
        ResetBounds();
    }
}