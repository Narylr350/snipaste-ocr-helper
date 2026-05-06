using Forms = System.Windows.Forms;
using SnipasteOcrHelper;
using SnipasteOcrHelper.Core;
using Strings = SnipasteOcrHelper.Localization.Resources;

namespace SnipasteOcrHelper.Tray;

public sealed class TrayController : IDisposable
{
    private readonly Forms.NotifyIcon notifyIcon;
    private readonly Action openSettings;
    private readonly Action openHistory;
    private readonly Action togglePaused;
    private readonly Action exit;
    private readonly Forms.ToolStripMenuItem pauseItem;
    private bool paused;

    public TrayController(Action openSettings, Action openHistory, Action togglePaused, Action exit)
    {
        this.openSettings = openSettings;
        this.openHistory = openHistory;
        this.togglePaused = togglePaused;
        this.exit = exit;
        pauseItem = new Forms.ToolStripMenuItem(Strings.TrayPauseMonitoring, null, (_, _) => TogglePaused());
        notifyIcon = new Forms.NotifyIcon
        {
            Icon = AppIconLoader.LoadIcon(),
            Visible = true,
            Text = Strings.AppName,
            ContextMenuStrip = new Forms.ContextMenuStrip()
        };
        notifyIcon.ContextMenuStrip.Items.Add(Strings.TrayOpenSettings, null, (_, _) => this.openSettings());
        notifyIcon.ContextMenuStrip.Items.Add(Strings.TrayOcrHistory, null, (_, _) => this.openHistory());
        notifyIcon.ContextMenuStrip.Items.Add(pauseItem);
        notifyIcon.ContextMenuStrip.Items.Add(Strings.TrayExit, null, (_, _) => this.exit());
    }

    public void UpdateStatus(AppStatusUpdate update)
    {
        notifyIcon.Text = string.Format(Strings.TrayTooltip, Strings.AppName, LocalizeStatus(update.Status));
    }

    private static string LocalizeStatus(AppStatus status)
    {
        return status switch
        {
            AppStatus.NeedsSetup => Strings.StatusNeedsSetup,
            AppStatus.Running => Strings.StatusRunning,
            AppStatus.Paused => Strings.StatusPaused,
            AppStatus.Processing => Strings.StatusProcessing,
            AppStatus.LastSuccess => Strings.StatusLastSuccess,
            AppStatus.Error => Strings.StatusError,
            AppStatus.NoText => Strings.StatusNoText,
            _ => status.ToString()
        };
    }

    private void TogglePaused()
    {
        paused = !paused;
        pauseItem.Text = paused ? Strings.TrayResumeMonitoring : Strings.TrayPauseMonitoring;
        togglePaused();
    }

    public void Dispose()
    {
        notifyIcon.Visible = false;
        notifyIcon.Dispose();
    }
}
