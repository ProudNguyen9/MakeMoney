using Microsoft.AspNetCore.Http;

namespace WebThuMuaPheLieu.Services;

public interface IBlogImageProcessor
{
    Task<IReadOnlyList<ProcessedBlogImageResult>> ProcessAndSaveAsync(
        IReadOnlyList<IFormFile> imageFiles,
        string uploadsDirectory,
        string fileBaseName,
        int startSequence,
        CancellationToken cancellationToken = default);

    string ResolveVariantImage(string? imageUrl, BlogImageVariant variant);

    Task<ProcessedBlogImageResult?> ProcessExistingImageAsync(
        string sourceImagePath,
        CancellationToken cancellationToken = default);
}

public sealed class ProcessedBlogImageResult
{
    public required string OriginalPath { get; init; }
    public required string MediumPath { get; init; }
    public required string ThumbPath { get; init; }
    public required string StoredUrl { get; init; }
}

public enum BlogImageVariant
{
    Original,
    Medium,
    Thumbnail
}

