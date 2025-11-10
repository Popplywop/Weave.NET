using System.Runtime.InteropServices;
using System.Text;

namespace Weave;

internal sealed partial class NativeMethod
{
    [LibraryImport("kernel32.dll", SetLastError = true)]
    public static partial IntPtr GetStdHandle(int nStdHandle);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetConsoleMode(IntPtr hConsoleHandle, out int lpMode);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

    // Native output methods for instant terminal writes (bypass .NET Console API)
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool WriteConsoleA(IntPtr hConsoleOutput, byte[] lpBuffer, int nNumberOfCharsToWrite, out int lpNumberOfCharsWritten, IntPtr lpReserved);

    [LibraryImport("libc", SetLastError = true)]
    public static partial int write(int fd, byte[] buf, int count);

    // Linux C APIs for immediate input detection (no polling delay like Ctrl+C)
    [LibraryImport("libc", SetLastError = true)]
    public static partial int read(int fd, byte[] buf, int count);

    [LibraryImport("libc", SetLastError = true)]
    public static partial int select(int nfds, IntPtr readfds, IntPtr writefds, IntPtr exceptfds, IntPtr timeout);

    [LibraryImport("libc", SetLastError = true)]
    public static partial int poll(IntPtr fds, uint nfds, int timeout);

    // Linux terminal control for raw mode (immediate key detection)
    [LibraryImport("libc", SetLastError = true)]
    public static partial int tcgetattr(int fd, IntPtr termios_p);

    [LibraryImport("libc", SetLastError = true)]
    public static partial int tcsetattr(int fd, int optional_actions, IntPtr termios_p);

    [LibraryImport("libc", SetLastError = true)]
    public static partial int fcntl(int fd, int cmd, int arg);

    // Native console input APIs for immediate key detection (no polling delay)
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool ReadConsoleInput(IntPtr hConsoleInput, IntPtr lpBuffer, int nLength, out int lpNumberOfEventsRead);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PeekConsoleInput(IntPtr hConsoleInput, IntPtr lpBuffer, int nLength, out int lpNumberOfEventsRead);

    public static void WriteToStdout(string text)
    {
        try
        {
            var bytes = Encoding.UTF8.GetBytes(text);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var handle = GetStdHandle(-11); // STD_OUTPUT_HANDLE
                WriteConsoleA(handle, bytes, bytes.Length, out _, IntPtr.Zero);
            }
            else
            {
                write(1, bytes, bytes.Length); // STDOUT_FILENO = 1
            }
        }
        catch
        {
            // Fallback to managed API if native fails
            Console.Out.Write(text);
        }
    }
}