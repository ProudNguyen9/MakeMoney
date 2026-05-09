using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace WebThuMuaPheLieu.Services;

public sealed class ProductImageProcessor : IProductImageProcessor
{
    private const int MinQuality = 50;
    private const int StartQuality = 82;
    private const int QualityStep = 6;
    private const int TargetBytesUpper = 200 * 1024;
    private const int TargetBytesLower = 100 * 1024;
    private const int MaxWidth = 1600;
    private const int MaxHeight = 1600;

    public async Task<ProcessedProductImageResult> ProcessAndSaveAsync(
        IFormFile imageFile,
        string outputDirectory,
        string outputFileNameWithoutExtension,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(outputDirectory);

        await using var readStream = imageFile.OpenReadStream();
        using var image = await Image.LoadAsync(readStream, cancellationToken);

        image.Mutate(ctx =>
            ctx.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(MaxWidth, MaxHeight),
                Sampler = KnownResamplers.Lanczos3
            }));

        var finalFileName = $"{outputFileNameWithoutExtension}.webp";
        var finalPhysicalPath = Path.Combine(outputDirectory, finalFileName);

        var quality = StartQuality;
        byte[]? encodedBytes = null;

        while (quality >= MinQuality)
        {
            await using var buffer = new MemoryStream();
            var encoder = new WebpEncoder
            {
                Quality = quality,
                Method = WebpEncodingMethod.BestQuality,
                FileFormat = WebpFileFormatType.Lossy
            };

            await image.SaveAsWebpAsync(buffer, encoder, cancellationToken);
            var candidate = buffer.ToArray();

            encodedBytes = candidate;

            if (candidate.LongLength <= TargetBytesUpper)
            {
                break;
            }

            quality -= QualityStep;
        }

        encodedBytes ??= [];

        if (encodedBytes.LongLength < TargetBytesLower && imageFile.Length <= TargetBytesLower)
        {
            encodedBytes = await PreserveCloserQualityAsync(image, cancellationToken);
        }

        await File.WriteAllBytesAsync(finalPhysicalPath, encodedBytes, cancellationToken);

        var normalizedDirectory = outputDirectory.Replace('\\', '/');
        var marker = "/wwwroot/";
        var markerIndex = normalizedDirectory.LastIndexOf(marker, StringComparison.OrdinalIgnoreCase);
        var relativeFolder = markerIndex >= 0
            ? normalizedDirectory[(markerIndex + marker.Length)..]
            : normalizedDirectory;

        return new ProcessedProductImageResult
        {
            PhysicalPath = finalPhysicalPath,
            RelativeUrl = $"/{relativeFolder.Trim('/')}/{finalFileName}".Replace('\\', '/'),
            OriginalBytes = imageFile.Length,
            OptimizedBytes = encodedBytes.LongLength,
            Width = image.Width,
            Height = image.Height
        };
    }

    private static async Task<byte[]> PreserveCloserQualityAsync(Image image, CancellationToken cancellationToken)
    {
        await using var buffer = new MemoryStream();
        var encoder = new WebpEncoder
        {
            Quality = 88,
            Method = WebpEncodingMethod.Default,
            FileFormat = WebpFileFormatType.Lossy
        };

        await image.SaveAsWebpAsync(buffer, encoder, cancellationToken);
        return buffer.ToArray();
    }
}

