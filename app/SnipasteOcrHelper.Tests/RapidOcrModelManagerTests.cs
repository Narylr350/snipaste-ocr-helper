using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using SnipasteOcrHelper.Ocr;
using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Tests;

public sealed class RapidOcrModelManagerTests
{
    [Fact]
    public void GetStatus_ReturnsMissing_WhenModelDirectoryDoesNotExist()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var manager = new RapidOcrModelManager(root);

        var status = manager.GetStatus();

        Assert.Equal(RapidOcrModelStatus.Missing, status);
    }

    [Fact]
    public void GetStatus_ReturnsInstalled_WhenRequiredFilesExist()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var manager = new RapidOcrModelManager(root);
        Directory.CreateDirectory(manager.ModelDirectory);
        foreach (var fileName in RapidOcrModelManager.RequiredFileNames)
        {
            File.WriteAllText(Path.Combine(manager.ModelDirectory, fileName), "model");
        }

        var status = manager.GetStatus();

        Assert.Equal(RapidOcrModelStatus.Installed, status);
    }

    [Fact]
    public async Task DownloadAsync_DownloadsEveryRequiredModelFile()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var downloadedFiles = new List<string>();
        var manager = new RapidOcrModelManager(root, RapidOcrModelPack.ChineseEnglish, (url, destinationPath, cancellationToken) =>
        {
            downloadedFiles.Add(Path.GetFileName(destinationPath));
            File.WriteAllText(destinationPath, url.ToString());
            return Task.CompletedTask;
        });

        await manager.DownloadAsync();

        Assert.Equal(RapidOcrModelManager.RequiredFileNames, downloadedFiles);
        foreach (var fileName in RapidOcrModelManager.RequiredFileNames)
        {
            Assert.True(File.Exists(Path.Combine(manager.ModelDirectory, fileName)));
        }
        Assert.Equal(RapidOcrModelStatus.Installed, manager.GetStatus());
    }

    [Fact]
    public async Task DownloadAsync_UsesSelectedRapidOcrModelPack()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var downloadedFiles = new List<string>();
        var manager = new RapidOcrModelManager(root, RapidOcrModelPack.Latin, (url, destinationPath, cancellationToken) =>
        {
            downloadedFiles.Add(Path.GetFileName(destinationPath));
            File.WriteAllText(destinationPath, url.ToString());
            return Task.CompletedTask;
        });

        await manager.DownloadAsync();

        Assert.Contains("latin_PP-OCRv5_rec_mobile.onnx", downloadedFiles);
        Assert.Contains("ppocrv5_latin_dict.txt", downloadedFiles);
        Assert.Equal(RapidOcrModelStatus.Installed, manager.GetStatus());
    }

    [Fact]
    public async Task DefaultDownloader_WritesDownloadedFile()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        var server = Task.Run(async () =>
        {
            using var client = await listener.AcceptTcpClientAsync();
            await using var stream = client.GetStream();
            var buffer = new byte[1024];
            var bytesRead = await stream.ReadAsync(buffer);
            var requestHeaders = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            var response = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nContent-Length: 5\r\nConnection: close\r\n\r\nmodel");
            await stream.WriteAsync(response);
            return requestHeaders;
        });
        var destinationPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var method = typeof(RapidOcrModelManager).GetMethod("DownloadFileAsync", BindingFlags.NonPublic | BindingFlags.Static)!;

        await (Task)method.Invoke(null, [new Uri($"http://127.0.0.1:{port}/model.onnx"), destinationPath, CancellationToken.None])!;
        var requestHeaders = await server;
        listener.Stop();

        Assert.Contains("User-Agent: SnipasteOcrHelper", requestHeaders);
        Assert.Equal("model", File.ReadAllText(destinationPath));
    }

    [Fact]
    public void ModelDirectory_ReturnsSelectedPackDirectory()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var manager = new RapidOcrModelManager(root, RapidOcrModelPack.Latin);

        Assert.Equal(Path.Combine(root, "latin"), manager.ModelDirectory);
    }
}
