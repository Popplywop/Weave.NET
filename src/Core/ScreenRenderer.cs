using System.Text;
using Weave.Core.Abstractions;
using Weave.Structs;

namespace Weave.Core;

/// <summary>
/// Handles the actual rendering of screen changes to terminal output
/// </summary>
internal sealed class ScreenRenderer
{
    private readonly ITerminalOutput _output;
    private readonly StringBuilder _renderBuffer = new(4096);

    public ScreenRenderer(ITerminalOutput output)
    {
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public void RenderChanges(Cell[,] prev, Cell[,] next, DirtyRegionTracker dirtyTracker, int rows, int cols)
    {
        if (!dirtyTracker.HasDirtyRegions)
        {
            return;
        }

        _renderBuffer.Clear();

        int startRow = Math.Max(0, dirtyTracker.MinDirtyRow);
        int endRow = Math.Min(rows - 1, dirtyTracker.MaxDirtyRow);
        int startCol = Math.Max(0, dirtyTracker.MinDirtyCol);
        int endCol = Math.Min(cols - 1, dirtyTracker.MaxDirtyCol);

        for (int r = startRow; r <= endRow; r++)
        {
            int c = startCol;
            while (c <= endCol)
            {
                if (dirtyTracker.IsDirty(r, c) && prev[r, c].Ch != next[r, c].Ch)
                {
                    // Send cursor to position
                    _renderBuffer.Append(AnsiStrings.CSI).Append(r + 1).Append(';').Append(c + 1).Append('H');

                    // Write consecutive changed characters
                    while (c <= endCol && dirtyTracker.IsDirty(r, c) && prev[r, c].Ch != next[r, c].Ch)
                    {
                        _renderBuffer.Append(next[r, c].Ch);
                        c++;
                    }
                }
                else
                {
                    c++;
                }
            }
        }

        if (_renderBuffer.Length > 0)
        {
            _output.Write(_renderBuffer.ToString());
        }
    }

    public void ClearScreen()
    {
        _output.Write($"{AnsiStrings.CLEAR_SCREEN}{AnsiStrings.HOME}");
    }
}