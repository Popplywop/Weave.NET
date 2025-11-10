namespace Weave.UI;

public sealed class ConsoleInputSource : IInputSource
{
    // Pre-computed modifier lookup table for performance
    private static readonly KeyMods[] ModifierLookup = new KeyMods[8];

    static ConsoleInputSource()
    {
        // Initialize modifier lookup table (3 bits for Shift, Ctrl, Alt combinations)
        for (int i = 0; i < 8; i++)
        {
            var mods = KeyMods.None;
            if ((i & 1) != 0)
            {
                mods |= KeyMods.Shift;
            }

            if ((i & 2) != 0)
            {
                mods |= KeyMods.Ctrl;
            }

            if ((i & 4) != 0)
            {
                mods |= KeyMods.Alt;
            }

            ModifierLookup[i] = mods;
        }
    }

    public KeyEvent? TryReadKey()
    {
        if (!Console.KeyAvailable)
        {
            return null;
        }

        var ki = Console.ReadKey(intercept: true);

        // Fast modifier lookup using bit manipulation
        int modIndex = 0;
        if ((ki.Modifiers & ConsoleModifiers.Shift) != 0)
        {
            modIndex |= 1;
        }

        if ((ki.Modifiers & ConsoleModifiers.Control) != 0)
        {
            modIndex |= 2;
        }

        if ((ki.Modifiers & ConsoleModifiers.Alt) != 0)
        {
            modIndex |= 4;
        }

        var mods = ModifierLookup[modIndex];

        // Optimized character mapping
        char? ch = (mods == KeyMods.None && !char.IsControl(ki.KeyChar)) ? ki.KeyChar : null;

        return new KeyEvent(ki.Key, mods, ch);
    }
}