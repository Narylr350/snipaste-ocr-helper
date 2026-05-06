using System.Windows;

namespace SnipasteOcrHelper;

public partial class App : System.Windows.Application
{
    private AppHost? host;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        host = AppHost.CreateDefault();
        host.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        host?.Dispose();
        base.OnExit(e);
    }
}
