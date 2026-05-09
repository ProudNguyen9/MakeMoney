using System.Collections.Concurrent;

namespace WebThuMuaPheLieu.Services;

public interface IBlogImageMigrationService
{
    Task<BlogImageMigrationResult> RunAsync(CancellationToken cancellationToken = default);
}

public sealed class BlogImageMigrationService : IBlogImageMigrationService
{
    private static readonly HashSet<string> SupportedExtensions =
    [".jpg", ".jpeg", ".png", ".webp", ".bmp", ".tiff", ".avif"];

    private readonly IWebHostEnvironment _environment;
    private readonly IBlogImageProcessor _blogImageProcessor;
    private readonly ILogger<BlogImageMigrationService> _logger;

    public BlogImageMigrationService(
        IWebHostEnvironment environment,
        IBlogImageProcessor blogImageProcessor,
        ILogger<BlogImageMigrationService> logger)
    {
        _environment = environment;
        _blogImageProcessor = blogImageProcessor;
        _logger = logger;
    }

    public async Task<BlogImageMigrationResult> RunAsync(CancellationToken cancellationToken = default)
    {
        var startedAt = DateTimeOffset.UtcNow;
        var blogsRoot = Path.Combine(_environment.WebRootPath, "assets", "images", "blogs");

        if (!Directory.Exists(blogsRoot))
        {
            return new BlogImageMigrationResult(0, 0, 0, startedAt, DateTimeOffset.UtcNow);
        }

        var files = Directory
            .EnumerateFiles(blogsRoot, "*.*", SearchOption.AllDirectories)
            .Where(path => SupportedExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            .Where(path =>
            {
                var name = Path.GetFileNameWithoutExtension(path);
                return !name.EndsWith("-thumb", StringComparison.OrdinalIgnoreCase)
                    && !name.EndsWith("-md", StringComparison.OrdinalIgnoreCase);
            })
            .ToList();

        var processed = 0;
        var skipped = 0;
        var failed = 0;

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var result = await _blogImageProcessor.ProcessExistingImageAsync(file, cancellationToken);
                if (result is null)
                {
                    skipped++;
                }
                else
                {
                    processed++;
                }
            }
            catch (Exception ex)
            {
                failed++;
                _logger.LogWarning(ex, "Blog image migration failed for file {FilePath}", file);
            }
        }

        return new BlogImageMigrationResult(files.Count, processed, skipped + failed, startedAt, DateTimeOffset.UtcNow);
    }
}

public sealed record BlogImageMigrationResult(
    int Scanned,
    int Processed,
    int Skipped,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt);

