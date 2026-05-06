namespace SnipasteOcrHelper.Core;

public enum AppStatus
{
    NeedsSetup,
    Running,
    Paused,
    Processing,
    LastSuccess,
    Error,
    NoText
}

public sealed record AppStatusUpdate(AppStatus Status, string Message);
