using System.Runtime.InteropServices;
using System.Text;
using Weave.Core.Abstractions;

namespace Weave.Core.Implementations;

/// <summary>
/// Native implementation of terminal initialization using platform-specific APIs
/// </summary>
public sealed class NativeTerminalInitializer : ITerminalInitializer
{
    // Windows VT setup constants
    private const int STD_OUTPUT_HANDLE = -11;
    private const int STD_INPUT_HANDLE = -10;
    private const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
    private const int DISABLE_NEWLINE_AUTO_RETURN = 0x0008;
    private const int ENABLE_PROCESSED_INPUT = 0x0001;
    private const int ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;

    public void SetupEncoding()
    {
        Console.OutputEncoding = new UTF8Encoding(false);
    }

    public void EnableVirtualTerminal()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return; // Unix-like systems support ANSI by default
        }

        var hOut = NativeMethod.GetStdHandle(STD_OUTPUT_HANDLE);
        if (NativeMethod.GetConsoleMode(hOut, out int modeOut))
        {
            modeOut |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            NativeMethod.SetConsoleMode(hOut, modeOut);
        }

        var hIn = NativeMethod.GetStdHandle(STD_INPUT_HANDLE);
        if (NativeMethod.GetConsoleMode(hIn, out int modeIn))
        {
            modeIn |= ENABLE_PROCESSED_INPUT | ENABLE_VIRTUAL_TERMINAL_INPUT;
            NativeMethod.SetConsoleMode(hIn, modeIn);
        }
    }

    public void EnterAlternateScreen()
    {
        // Enter alternate screen & hide cursor
        NativeMethod.WriteToStdout(AnsiStrings.ENABLE_ALT_SCREEN);
        NativeMethod.WriteToStdout(AnsiStrings.HIDE_CURSOR);
        ClearScreen();
    }

    public void RestoreTerminal()
    {
        try
        {
            // Use native APIs for instant output (bypass .NET Console overhead)
            var restoreSequence = AnsiStrings.VISIBLE_CURSOR + AnsiStrings.DISABLE_ALT_SCREEN;
            NativeMethod.WriteToStdout(restoreSequence);
        }
        catch
        {
            // Ignore errors during shutdown
        }
    }

    public void ClearScreen()
    {
        NativeMethod.WriteToStdout(AnsiStrings.CLEAR_SCREEN);
    }
}