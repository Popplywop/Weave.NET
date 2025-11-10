using Weave.UI;

namespace Weave.UI.Layout.Strategies;

/// <summary>
/// Layout strategy for VText nodes
/// </summary>
internal sealed class VTextLayoutStrategy : ILayoutStrategy
{
    public bool CanHandle(VNode node) => node is VText;

    public void LayoutNode(LayoutNode node, Action<LayoutNode> recursiveLayoutAction)
    {
        if (node.VNode is not VText text)
        {
            return;
        }

        // Text nodes are leaf nodes - no children to layout
        // Size is determined by text content and available space
        var availableWidth = node.Bounds.w;
        var lines = WrapText(text.Text, availableWidth);

        // Update height based on wrapped text
        node.Bounds = node.Bounds with { h = Math.Max(1, lines.Count) };
    }

    public int GetGrow(VNode node) => 0; // Text nodes don't grow

    public int GetFixedHeight(VNode node) => 1; // Default height for text

    public int GetFixedWidth(VNode node)
    {
        if (node is VText text)
        {
            return text.Text.Length;
        }

        return 10; // Default width
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
}