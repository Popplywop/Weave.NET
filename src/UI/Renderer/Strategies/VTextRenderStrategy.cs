using Weave.UI;
using Weave.UI.Layout;

namespace Weave.UI.Renderer.Strategies;

/// <summary>
/// Render strategy for VText nodes
/// </summary>
internal sealed class VTextRenderStrategy : IRenderStrategy
{
    public bool CanHandle(VNode node) => node is VText;

    public void Render(LayoutNode node, VirtualScreen screen)
    {
        if (node.VNode is not VText text)
        {
            return;
        }

        var bounds = node.Bounds;
        if (bounds.w <= 0 || bounds.h <= 0)
        {
            return;
        }

        var lines = WrapText(text.Text, bounds.w);

        for (int i = 0; i < Math.Min(lines.Count, bounds.h); i++)
        {
            var line = lines[i];
            var row = bounds.y + i;

            // Apply text alignment
            var (startCol, renderedText) = AlignText(line, bounds.w, text.Align);

            screen.Put(row, bounds.x + startCol, renderedText);
        }
    }

    private static List<string> WrapText(string text, int maxWidth)
    {
        if (maxWidth <= 0)
        {
            return [text];
        }

        var lines = new List<string>();
        var currentLine = "";

        foreach (var word in text.Split(' '))
        {
            if (currentLine.Length + word.Length + 1 <= maxWidth)
            {
                if (currentLine.Length > 0)
                {
                    currentLine += " ";
                }

                currentLine += word;
            }
            else
            {
                if (currentLine.Length > 0)
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    // Word is longer than maxWidth, just add it
                    lines.Add(word);
                }
            }
        }

        if (currentLine.Length > 0)
        {
            lines.Add(currentLine);
        }

        return lines.Count > 0 ? lines : [""];
    }

    private static (int startCol, string text) AlignText(string text, int width, Align align)
    {
        if (text.Length >= width)
        {
            return (0, text[..width]);
        }

        return align switch
        {
            Align.Start => (0, text),
            Align.Center => ((width - text.Length) / 2, text),
            Align.End => (width - text.Length, text),
            _ => (0, text)
        };
    }
}