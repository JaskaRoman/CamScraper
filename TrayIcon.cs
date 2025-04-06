namespace CamScraper;

public class TrayIcon : IDisposable
{
    private const string Title = nameof(CamScraper);
    public TrayIcon() => icon.DoubleClick += OnShowHide;

    private readonly NotifyIcon icon = new()
    {
        Visible = true,
        Text = Title,
        ContextMenuStrip = BuildContextMenu(),
        Icon = SystemIcons.GetStockIcon(StockIconId.DeviceCamera),
    };

    private static void OnShowHide(object? _, EventArgs __)
    {
        Action targetAction = ConsoleWindow.IsVisible() ? ConsoleWindow.Hide : ConsoleWindow.Show;
        targetAction.Invoke();
    }

    private static ContextMenuStrip BuildContextMenu() => new()
    {
        Items =
        {
            new ToolStripMenuItem(string.Join('/', nameof(ConsoleWindow.Show), nameof(ConsoleWindow.Hide)), null, OnShowHide),
            new ToolStripSeparator(),
            new ToolStripMenuItem(nameof(Application.Exit), null, (_, _) => Application.Exit())
        }
    };

    public void Dispose()
    {
        icon.Visible = false;
        icon.Dispose();
    }
}