namespace SnipasteOcrHelper.Queue;

public interface IImageFileDeleter
{
    Task MoveToRecycleBinAsync(string path, CancellationToken cancellationToken = default);
}
