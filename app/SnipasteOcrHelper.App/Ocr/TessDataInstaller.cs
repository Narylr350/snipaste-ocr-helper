using System.IO;
using System.Reflection;
using SnipasteOcrHelper.Settings;

namespace SnipasteOcrHelper.Ocr;

public sealed class TessDataInstaller
{
    private const string CurrentVersion = "eng-chi_sim-2026-05-06";
    private readonly string targetDirectory;
    private readonly IReadOnlyCollection<TessDataResource> resources;
    private readonly string version;

    public TessDataInstaller(string targetDirectory, IEnumerable<TessDataResource> resources)
        : this(targetDirectory, resources, CurrentVersion)
    {
    }

    public TessDataInstaller(string targetDirectory, IEnumerable<TessDataResource> resources, string version)
    {
        this.targetDirectory = targetDirectory;
        this.resources = resources.ToArray();
        this.version = version;
    }

    public static TessDataInstaller CreateDefault()
    {
        return CreateDefault(DefaultPaths.TessDataDirectory);
    }

    public static TessDataInstaller CreateDefault(string targetDirectory)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames()
            .Where(name => name.EndsWith(".traineddata", StringComparison.OrdinalIgnoreCase))
            .Select(name => new TessDataResource(
                name[(name.LastIndexOf('.', name.LastIndexOf('.') - 1) + 1)..],
                () => assembly.GetManifestResourceStream(name) ?? Stream.Null));
        return new TessDataInstaller(targetDirectory, resources);
    }

    public async Task EnsureInstalledAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(targetDirectory);
        var versionPath = Path.Combine(targetDirectory, ".version");
        var reinstall = !File.Exists(versionPath) || await File.ReadAllTextAsync(versionPath, cancellationToken) != version;
        foreach (var resource in resources)
        {
            var targetPath = Path.Combine(targetDirectory, resource.FileName);
            if (!reinstall && File.Exists(targetPath))
            {
                continue;
            }

            await using var source = resource.OpenStream();
            await using var target = File.Create(targetPath);
            await source.CopyToAsync(target, cancellationToken);
        }

        await File.WriteAllTextAsync(versionPath, version, cancellationToken);
    }
}
