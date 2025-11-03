using System.Text;
using Weave.Structs;

namespace Weave;

internal sealed class VirtualScreen
{
    public int Rows { get; private set; }
    public int Cols { get; private set; }

    readonly Lock _sync = new();

    private Cell[,] _prev = new Cell[0, 0];
    private Cell[,] _next = new Cell[0, 0];

    public VirtualScreen(int rows, int cols) => Resize(rows, cols);

    public void Resize(int rows, int cols)
    {
        lock (_sync)
        {
            Rows = rows; Cols = cols;
            _prev = new Cell[rows, cols];
            _next = new Cell[rows, cols];
            ClearNext_Nolock();
        }
    }

    public void ClearNext()
    {
        lock (_sync)
        {
            ClearNext_Nolock();
        }
    }

    public void ClearNext_Nolock()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                _next[r, c] = Cell.Blank;
    }

    public void Put(int row, int col, string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        lock (_sync)
        {
            if (row < 0 || row >= Rows) return;
            for (int i = 0; i < text.Length; i++)
            {
                int cc = col + i;
                if (cc >= 0 && cc < Cols)
                    _next[row, cc].Ch = text[i];
            }
        }
    }

    public void Plot(int row, int col, char ch)
    {
        lock (_sync)
        {
            if (row >= 0 && row < Rows && col >= 0 && col < Cols)
                _next[row, col].Ch = ch;
        }
    }

    public void ClearVirtualAndInvalidate()
    {
        lock (_sync)
        {
            Console.Out.Write($"{AnsiStrings.CLEAR_SCREEN}{AnsiStrings.HOME}"); // clear + home
            ClearNext();
            _prev = new Cell[Rows, Cols]; // force full repaint 
        }
    }

    public async Task RenderAsync()
    {
        Cell[,] prevRef;
        Cell[,] snapshot;
        int rows, cols;

        lock (_sync)
        {
            rows = Rows;
            cols = Cols;
            prevRef = _prev;
            snapshot = (Cell[,])_next.Clone(); 
        }

        var sb = new StringBuilder();

        for (int r = 0; r < rows; r++)
        {
            int c = 0;
            while (c < Cols)
            {
                if (prevRef[r, c].Ch != snapshot[r, c].Ch)
                {
                    sb.Append(AnsiStrings.CSI).Append(r + 1).Append(';').Append(c + 1).Append('H');
                    while (c < cols && prevRef[r, c].Ch != snapshot[r, c].Ch)
                    {
                        sb.Append(snapshot[r, c].Ch); c++;
                    }
                }
                else c++;
            }
        }
        if (sb.Length > 0) await Console.Out.WriteAsync(sb.ToString());

        lock (_sync)
        {
            _prev = snapshot;

            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    _next[r, c] = snapshot[r, c];
        }
    }
}