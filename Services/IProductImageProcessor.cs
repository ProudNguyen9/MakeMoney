using Microsoft.AspNetCore.Http;

namespace WebThuMuaPheLieu.Services;

public interface IProductImageProcessor
{
    Task<ProcessedProductImageResult> ProcessAndSaveAsync(
        IFormFile imageFile,
        string outputDirectory,
        string outputFileNameWithoutExtension,
        CancellationToken cancellationToken = default);
}

public sealed class ProcessedProductImageResult
{
    public string RelativeUrl { get; init; } = string.Empty;

    public string PhysicalPath { get; init; } = string.Empty;

    public long OriginalBytes { get; init; }

    public long OptimizedBytes { get; init; }

    public int Width { get; init; }

    public int Height { get; init; }
}

