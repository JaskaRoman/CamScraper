using System.Runtime.InteropServices;

namespace CamScraper;

public static class ConsoleWindow
{
    private const int SwHide = 0, SwShow = 5;

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    public static void Hide() => ShowWindow(GetConsoleWindow(), SwHide);
    public static void Show() => ShowWindow(GetConsoleWindow(), SwShow);
    public static bool IsVisible() => GetConsoleWindow() is var p && p != IntPtr.Zero && IsWindowVisible(p);
}