using Microsoft.VisualBasic.FileIO;

namespace SnipasteOcrHelper.Queue;

public sealed class ImageFileDeleter : IImageFileDeleter
{
    public Task MoveToRecycleBinAsync(string path, CancellationToken cancellationToken = default)
    {
        FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        return Task.CompletedTask;
    }
}
