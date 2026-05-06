using System.Drawing;
using System.Windows;
using Forms = System.Windows.Forms;
using SnipasteOcrHelper.Core;

namespace SnipasteOcrHelper.Tray;

public sealed class TrayController : IDisposable
{
    private readonly Forms.NotifyIcon notifyIcon;
    private readonly Action openSettings;
    private readonly Action togglePaused;
    private readonly Action exit;
    private readonly Forms.ToolStripMenuItem pauseItem;
    private bool paused;

    public TrayController(Action openSettings, Action togglePaused, Action exit)
    {
        this.openSettings = openSettings;
        this.togglePaused = togglePaused;
        this.exit = exit;
        pauseItem = new Forms.ToolStripMenuItem("Pause Monitoring", null, (_, _) => TogglePaused());
        notifyIcon = new Forms.NotifyIcon
        {
            Icon = LoadAppIcon(),
            Visible = true,
            Text = "Snipaste OCR Helper",
            ContextMenuStrip = new Forms.ContextMenuStrip()
        };
        notifyIcon.ContextMenuStrip.Items.Add("Open Settings", null, (_, _) => this.openSettings());
        notifyIcon.ContextMenuStrip.Items.Add(pauseItem);
        notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (_, _) => this.exit());
    }

    public void UpdateStatus(AppStatusUpdate update)
    {
        notifyIcon.Text = $"Snipaste OCR Helper - {update.Status}";
    }

    private static Icon LoadAppIcon()
    {
        var info = System.Windows.Application.GetResourceStream(new Uri("/Assets/AppIcon.ico", UriKind.Relative))
            ?? throw new InvalidOperationException("Application icon resource is missing.");
        return new Icon(info.Stream);
    }

    private void TogglePaused()
    {
        paused = !paused;
        pauseItem.Text = paused ? "Resume Monitoring" : "Pause Monitoring";
        togglePaused();
    }

    public void Dispose()
    {
        notifyIcon.Visible = false;
        notifyIcon.Dispose();
    }
}
