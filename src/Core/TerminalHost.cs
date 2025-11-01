using System.Runtime.InteropServices;
using System.Text;

namespace Weave;

internal sealed class TerminalHost : IDisposable
{
    // Windows VT setup
    const int STD_OUTPUT_HANDLE = -11;
    const int STD_INPUT_HANDLE = -10;
    const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
    const int DISABLE_NEWLINE_AUTO_RETURN = 0x0008;
    const int ENABLE_PROCESSED_INPUT = 0x0001;
    const int ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;

    const string CSI = "\x1b[";

    public TerminalHost()
    {
        Console.OutputEncoding = new UTF8Encoding(false);
        EnableVT();

        // Enter alternate screen & hide cursor
        Write("\x1b[?1049h"); // alt screen
        Write("\x1b[?25l");   // hide cursor
        Clear();
    }

    public static void Clear() => Write($"{CSI}2J{CSI}H");
    public static void Write(string s) => Console.Out.Write(s);

    public static void EnableVT()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
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

    public void Dispose()
    {
        // Show cursor & leave alternate screen
        Write("\x1b[?25h");
        Write("\x1b[?1049l");
    }
}