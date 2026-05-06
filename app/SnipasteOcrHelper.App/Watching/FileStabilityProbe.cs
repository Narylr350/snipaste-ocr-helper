using System.IO;

namespace SnipasteOcrHelper.Watching;

public sealed class FileStabilityProbe
{
    private readonly TimeSpan interval;
    private readonly int requiredStableSamples;

    public FileStabilityProbe(TimeSpan? interval = null, int requiredStableSamples = 3)
    {
        this.interval = interval ?? TimeSpan.FromMilliseconds(250);
        this.requiredStableSamples = requiredStableSamples;
    }

    public async Task<bool> WaitUntilStableAsync(string path, CancellationToken cancellationToken = default)
    {
        long? previousLength = null;
        DateTime? previousLastWrite = null;
        var stableSamples = 0;

        while (stableSamples < requiredStableSamples)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!File.Exists(path))
            {
                return false;
            }

            var info = new FileInfo(path);
            if (previousLength == info.Length && previousLastWrite == info.LastWriteTimeUtc && CanOpenForRead(path))
            {
                stableSamples++;
            }
            else
            {
                stableSamples = 0;
                previousLength = info.Length;
                previousLastWrite = info.LastWriteTimeUtc;
            }

            if (stableSamples < requiredStableSamples)
            {
                await Task.Delay(interval, cancellationToken);
            }
        }

        return true;
    }

    private static bool CanOpenForRead(string path)
    {
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return stream.Length >= 0;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}
