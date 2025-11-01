using System.Runtime.InteropServices;

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
}