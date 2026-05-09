using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace WebThuMuaPheLieu.Services;

public sealed class BlogImageProcessor : IBlogImageProcessor
{
    public async Task<IReadOnlyList<ProcessedBlogImageResult>> ProcessAndSaveAsync(
        IReadOnlyList<IFormFile> imageFiles,
        string uploadsDirectory,
        string fileBaseName,
        int startSequence,
        CancellationToken cancellationToken = default)
    {
        if (imageFiles.Count == 0)
        {
            return [];
        }

        Directory.CreateDirectory(uploadsDirectory);
        var results = new List<ProcessedBlogImageResult>(imageFiles.Count);
        var webpEncoderOriginal = new WebpEncoder { Quality = 82, Method = WebpEncodingMethod.BestQuality };
        var webpEncoderMedium = new WebpEncoder { Quality = 78, Method = WebpEncodingMethod.Default };
        var webpEncoderThumb = new WebpEncoder { Quality = 72, Method = WebpEncodingMethod.Default };

        for (var i = 0; i < imageFiles.Count; i++)
        {
            var file = imageFiles[i];
            var seq = startSequence + i;
            var baseName = $"{fileBaseName}{seq}";

            var originalFileName = $"{baseName}.webp";
            var mediumFileName = $"{baseName}-md.webp";
            var thumbFileName = $"{baseName}-thumb.webp";

            var originalPath = Path.Combine(uploadsDirectory, originalFileName);
            var mediumPath = Path.Combine(uploadsDirectory, mediumFileName);
            var thumbPath = Path.Combine(uploadsDirectory, thumbFileName);

            await using var inputStream = file.OpenReadStream();
            using var image = await Image.LoadAsync(inputStream, cancellationToken);

            using (var original = image.Clone(ctx =>
                       ctx.Resize(new ResizeOptions
                       {
                           Mode = ResizeMode.Max,
                           Size = new Size(1920, 1920),
                           Sampler = KnownResamplers.Lanczos3
                       })))
            {
                await original.SaveAsWebpAsync(originalPath, webpEncoderOriginal, cancellationToken);
            }

            using (var medium = image.Clone(ctx =>
                       ctx.Resize(new ResizeOptions
                       {
                           Mode = ResizeMode.Max,
                           Size = new Size(1280, 1280),
                           Sampler = KnownResamplers.Lanczos3
                       })))
            {
                await medium.SaveAsWebpAsync(mediumPath, webpEncoderMedium, cancellationToken);
            }

            using (var thumb = image.Clone(ctx =>
                       ctx.Resize(new ResizeOptions
                       {
                           Mode = ResizeMode.Crop,
                           Size = new Size(640, 480),
                           Position = AnchorPositionMode.Center,
                           Sampler = KnownResamplers.Lanczos3
                       })))
            {
                await thumb.SaveAsWebpAsync(thumbPath, webpEncoderThumb, cancellationToken);
            }

            var uploadsNormalized = uploadsDirectory.Replace("\\", "/");
            var marker = "/wwwroot/";
            var markerIndex = uploadsNormalized.LastIndexOf(marker, StringComparison.OrdinalIgnoreCase);
            var relativeFolder = markerIndex >= 0
                ? uploadsNormalized[(markerIndex + marker.Length)..]
                : uploadsNormalized;

            var storedUrl = $"/{relativeFolder.Trim('/')}/{originalFileName}";

            results.Add(new ProcessedBlogImageResult
            {
                OriginalPath = originalPath,
                MediumPath = mediumPath,
                ThumbPath = thumbPath,
                StoredUrl = storedUrl.Replace("\\", "/")
            });
        }

        return results;
    }

    public string ResolveVariantImage(string? imageUrl, BlogImageVariant variant)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return "/assets/img/blog/blog-1.jpg";
        }

        var normalized = imageUrl.Replace("\\", "/").Trim();
        if (!normalized.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
        {
            return normalized;
        }

        var suffix = variant switch
        {
            BlogImageVariant.Thumbnail => "-thumb.webp",
            BlogImageVariant.Medium => "-md.webp",
            _ => ".webp"
        };

        if (variant == BlogImageVariant.Original)
        {
            return normalized;
        }

        var variantPath = normalized[..^5] + suffix;
        var physicalPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            variantPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        return File.Exists(physicalPath) ? variantPath : normalized;
    }

    public async Task<ProcessedBlogImageResult?> ProcessExistingImageAsync(
        string sourceImagePath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sourceImagePath) || !File.Exists(sourceImagePath))
        {
            return null;
        }

        var ext = Path.GetExtension(sourceImagePath);
        if (string.Equals(ext, ".svg", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ext, ".gif", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ext, ".ico", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var directory = Path.GetDirectoryName(sourceImagePath);
        var fileNameNoExt = Path.GetFileNameWithoutExtension(sourceImagePath);
        if (string.IsNullOrWhiteSpace(directory) || string.IsNullOrWhiteSpace(fileNameNoExt))
        {
            return null;
        }

        if (fileNameNoExt.EndsWith("-thumb", StringComparison.OrdinalIgnoreCase)
            || fileNameNoExt.EndsWith("-md", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var originalWebpPath = Path.Combine(directory, $"{fileNameNoExt}.webp");
        var mediumPath = Path.Combine(directory, $"{fileNameNoExt}-md.webp");
        var thumbPath = Path.Combine(directory, $"{fileNameNoExt}-thumb.webp");

        if (File.Exists(mediumPath) && File.Exists(thumbPath))
        {
            if (!File.Exists(originalWebpPath) && !string.Equals(ext, ".webp", StringComparison.OrdinalIgnoreCase))
            {
                await ConvertToWebpAsync(sourceImagePath, originalWebpPath, cancellationToken);
            }

            return new ProcessedBlogImageResult
            {
                OriginalPath = File.Exists(originalWebpPath) ? originalWebpPath : sourceImagePath,
                MediumPath = mediumPath,
                ThumbPath = thumbPath,
                StoredUrl = string.Empty
            };
        }

        using var image = await Image.LoadAsync(sourceImagePath, cancellationToken);
        var webpEncoderOriginal = new WebpEncoder { Quality = 82, Method = WebpEncodingMethod.BestQuality };
        var webpEncoderMedium = new WebpEncoder { Quality = 78, Method = WebpEncodingMethod.Default };
        var webpEncoderThumb = new WebpEncoder { Quality = 72, Method = WebpEncodingMethod.Default };

        if (!File.Exists(originalWebpPath))
        {
            using var original = image.Clone(ctx =>
                ctx.Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(1920, 1920), Sampler = KnownResamplers.Lanczos3 }));
            await original.SaveAsWebpAsync(originalWebpPath, webpEncoderOriginal, cancellationToken);
        }

        if (!File.Exists(mediumPath))
        {
            using var medium = image.Clone(ctx =>
                ctx.Resize(new ResizeOptions { Mode = ResizeMode.Max, Size = new Size(1280, 1280), Sampler = KnownResamplers.Lanczos3 }));
            await medium.SaveAsWebpAsync(mediumPath, webpEncoderMedium, cancellationToken);
        }

        if (!File.Exists(thumbPath))
        {
            using var thumb = image.Clone(ctx =>
                ctx.Resize(new ResizeOptions { Mode = ResizeMode.Crop, Size = new Size(640, 480), Position = AnchorPositionMode.Center, Sampler = KnownResamplers.Lanczos3 }));
            await thumb.SaveAsWebpAsync(thumbPath, webpEncoderThumb, cancellationToken);
        }

        return new ProcessedBlogImageResult
        {
            OriginalPath = File.Exists(originalWebpPath) ? originalWebpPath : sourceImagePath,
            MediumPath = mediumPath,
            ThumbPath = thumbPath,
            StoredUrl = string.Empty
        };
    }

    private static async Task ConvertToWebpAsync(string inputPath, string outputPath, CancellationToken cancellationToken)
    {
        using var image = await Image.LoadAsync(inputPath, cancellationToken);
        var encoder = new WebpEncoder { Quality = 82, Method = WebpEncodingMethod.BestQuality };
        await image.SaveAsWebpAsync(outputPath, encoder, cancellationToken);
    }
}

