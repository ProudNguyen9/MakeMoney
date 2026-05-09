using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;
using WebThuMuaPheLieu.Services;
using WebThuMuaPheLieu.helpper;

namespace WebThuMuaPheLieu.Controllers;

public class BlogController : Controller
{
    private const string UsefulSessionKeyPrefix = "blog-useful:";
    private static readonly TimeSpan UsefulCooldown = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan BlogCacheDuration = TimeSpan.FromMinutes(10);
    private readonly AppDbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly IContactInfoHelper _contactInfoHelper;
    private readonly IBlogImageProcessor _blogImageProcessor;

    public BlogController(
        AppDbContext context,
        IMemoryCache memoryCache,
        IContactInfoHelper contactInfoHelper,
        IBlogImageProcessor blogImageProcessor)
    {
        _context = context;
        _memoryCache = memoryCache;
        _contactInfoHelper = contactInfoHelper;
        _blogImageProcessor = blogImageProcessor;
    }

    [OutputCache(PolicyName = "BlogList")]
    public async Task<IActionResult> Index(string? searchTerm, string? categoryName, int page = 1)
    {
        var viewModel = await BuildBlogIndexViewModelAsync(page, searchTerm, categoryName);
        return View(viewModel);
    }

    [OutputCache(PolicyName = "BlogDetail")]
    public async Task<IActionResult> Detail(string slug)
    {
        var normalizedSlug = slug?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedSlug))
        {
            return RedirectToAction(nameof(Index));
        }

        var parsedPostId = int.TryParse(normalizedSlug, out var postIdValue)
            ? postIdValue
            : (int?)null;

        var post = await _context.BlogPosts
            .AsNoTracking()
            .Include(item => item.Author)
            .Include(item => item.BlogImages)
            .FirstOrDefaultAsync(item => item.Status == "published"
                && (((item.Slug ?? string.Empty) == normalizedSlug)
                    || (parsedPostId.HasValue && item.Id == parsedPostId.Value)));

        if (post is null)
        {
            return NotFound();
        }

        var categoryFilters = await BuildCategoryFiltersAsync();

        var postCategories = await _context.BlogPostCategories
            .AsNoTracking()
            .Where(postCategory => postCategory.PostId == post.Id && postCategory.CategoryId.HasValue)
            .Join(
                _context.BlogCategories.AsNoTracking(),
                postCategory => postCategory.CategoryId,
                blogCategory => (int?)blogCategory.Id,
                (postCategory, blogCategory) => new
                {
                    blogCategory.Id,
                    blogCategory.Name
                })
            .Distinct()
            .ToListAsync();

        var primaryCategoryName = postCategories
            .Select(item => item.Name)
            .FirstOrDefault(name => !string.IsNullOrWhiteSpace(name)) ?? "Tin tức";

        var relatedCategoryIds = postCategories
            .Select(item => item.Id)
            .ToList();

        var relatedPostsQuery = _context.BlogPosts
            .AsNoTracking()
            .Where(item => item.Status == "published" && item.Id != post.Id);

        if (relatedCategoryIds.Count > 0)
        {
            var categoryPostIds = _context.BlogPostCategories
                .AsNoTracking()
                .Where(postCategory => postCategory.PostId.HasValue
                    && postCategory.CategoryId.HasValue
                    && relatedCategoryIds.Contains(postCategory.CategoryId.Value))
                .Select(postCategory => postCategory.PostId!.Value)
                .Distinct();

            relatedPostsQuery = relatedPostsQuery.Where(item => categoryPostIds.Contains(item.Id));
        }

        var relatedPosts = await relatedPostsQuery
            .OrderByDescending(item => item.PublishedAt ?? item.CreatedAt)
            .ThenByDescending(item => item.Id)
            .Take(4)
            .Select(item => new BlogPostListItem
            {
                Id = item.Id,
                Slug = item.Slug,
                Title = item.Title,
                Excerpt = item.Excerpt,
                Content = item.Content,
                CoverImage = item.CoverImage,
                PublishedAt = item.PublishedAt ?? item.CreatedAt,
                LikeCount = item.LikeCount ?? 0
            })
            .ToListAsync();

        var relatedBlogCards = await BuildBlogCardsAsync(relatedPosts);
        var currentUrl = Url.Action(nameof(Detail), "Blog", new { slug = normalizedSlug }, Request.Scheme) ?? string.Empty;
        var galleryImages = post.BlogImages
            .OrderBy(image => image.OrderIndex ?? int.MaxValue)
            .ThenBy(image => image.Id)
            .Select(image => NormalizeBlogImagePath(image.ImageUrl))
            .Where(imageUrl => !string.IsNullOrWhiteSpace(imageUrl))
            .Distinct()
            .ToList();

        var normalizedCoverImage = NormalizeBlogImagePath(post.CoverImage);

        if (!galleryImages.Any())
        {
            galleryImages.Add(normalizedCoverImage);
        }
        else if (!galleryImages.Any(imageUrl => string.Equals(imageUrl, normalizedCoverImage, StringComparison.OrdinalIgnoreCase)))
        {
            galleryImages.Insert(0, normalizedCoverImage);
        }

        var viewModel = new BlogDetailViewModel
        {
            Post = post,
            CoverImage = normalizedCoverImage,
            GalleryImages = galleryImages,
            PrimaryCategoryName = primaryCategoryName,
            AuthorName = post.Author?.FullName ?? string.Empty,
            CurrentUrl = currentUrl,
            RelatedPosts = relatedBlogCards,
            Categories = categoryFilters
        };

        return View(viewModel);
    }

    [HttpGet]
    [OutputCache(PolicyName = "BlogPagedApi")]
    public async Task<IActionResult> GetPagedBlogs(string? searchTerm, string? categoryName, int page = 1)
    {
        var viewModel = await BuildBlogIndexViewModelAsync(page, searchTerm, categoryName);

        return Json(new
        {
            currentPage = viewModel.CurrentPage,
            totalPages = viewModel.TotalPages,
            pageSize = viewModel.PageSize,
            totalItems = viewModel.TotalItems,
            searchTerm = viewModel.SearchTerm,
            selectedCategoryName = viewModel.SelectedCategoryName,
            contactPhone = viewModel.ContactPhone,
            posts = viewModel.Posts.Select(post => new
            {
                id = post.Id,
                slug = post.Slug,
                title = post.Title,
                summary = post.Summary,
                coverImage = post.CoverImage,
                imageSequence = post.ImageSequence,
                categoryName = post.CategoryName,
                publishedAt = post.PublishedAt?.ToString("dd/MM/yyyy"),
                likeCount = post.LikeCount,
                detailUrl = Url.Action(nameof(Detail), "Blog", new { slug = BuildBlogSlug(post.Slug, post.Id) }) ?? "/Blog"
            })
        });
    }

    [HttpPost]
    public async Task<IActionResult> MarkUseful([FromForm] int id)
    {
        if (id <= 0)
        {
            return Json(new
            {
                success = false,
                message = "Bài viết không hợp lệ."
            });
        }

        if (TryGetUsefulCooldownRemaining(id, out var cooldownSecondsRemaining))
        {
            return Json(new
            {
                success = false,
                cooldownSecondsRemaining,
                message = BuildUsefulCooldownMessage(cooldownSecondsRemaining)
            });
        }

        var post = await _context.BlogPosts
            .FirstOrDefaultAsync(item => item.Id == id && item.Status == "published");

        if (post is null)
        {
            return Json(new
            {
                success = false,
                message = "Không tìm thấy bài viết."
            });
        }

        post.LikeCount = (post.LikeCount ?? 0) + 1;
        await _context.SaveChangesAsync();

        HttpContext.Session.SetString(
            BuildUsefulSessionKey(id),
            DateTimeOffset.UtcNow.Add(UsefulCooldown).ToString("O"));

        return Json(new
        {
            success = true,
            likeCount = post.LikeCount ?? 0,
            cooldownSecondsRemaining = (int)UsefulCooldown.TotalSeconds,
            message = "Đã ghi nhận đánh giá hữu ích."
        });
    }

    private async Task<BlogIndexViewModel> BuildBlogIndexViewModelAsync(int page, string? searchTerm, string? categoryName)
    {
        const int pageSize = 6;

        var normalizedSearchTerm = searchTerm?.Trim() ?? string.Empty;
        var normalizedCategoryName = categoryName?.Trim() ?? string.Empty;

        var publishedPostsQuery = _context.BlogPosts
            .AsNoTracking()
            .Where(post => post.Status == "published");

        var categoryFilters = await BuildCategoryFiltersAsync(publishedPostsQuery);
        var contactInfo = await _contactInfoHelper.GetContactInfoAsync();

        var query = publishedPostsQuery;

        if (!string.IsNullOrWhiteSpace(normalizedCategoryName))
        {
            var categoryPostIds = _context.BlogPostCategories
                .AsNoTracking()
                .Where(postCategory => postCategory.PostId.HasValue)
                .Join(
                    _context.BlogCategories.AsNoTracking().Where(category => category.Name == normalizedCategoryName),
                    postCategory => postCategory.CategoryId,
                    category => (int?)category.Id,
                    (postCategory, category) => postCategory.PostId!.Value);

            query = query.Where(post => categoryPostIds.Contains(post.Id));
        }

        if (!string.IsNullOrWhiteSpace(normalizedSearchTerm))
        {
            var keywordPattern = $"%{normalizedSearchTerm}%";

            query = query.Where(post =>
                EF.Functions.Like(post.Title ?? string.Empty, keywordPattern)
                || EF.Functions.Like(post.Excerpt ?? string.Empty, keywordPattern)
                || EF.Functions.Like(post.Content ?? string.Empty, keywordPattern));
        }

        var totalItems = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        var currentPage = Math.Min(Math.Max(page, 1), totalPages);

        var pagedPosts = await query
            .OrderByDescending(post => post.PublishedAt ?? post.CreatedAt)
            .ThenByDescending(post => post.Id)
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .Select(post => new BlogPostListItem
            {
                Id = post.Id,
                Slug = post.Slug,
                Title = post.Title,
                Excerpt = post.Excerpt,
                Content = post.Content,
                CoverImage = post.CoverImage,
                PublishedAt = post.PublishedAt ?? post.CreatedAt,
                LikeCount = post.LikeCount ?? 0
            })
            .ToListAsync();

        var featuredPosts = await _memoryCache.GetOrCreateAsync(BlogCacheKeys.FeaturedPosts, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = BlogCacheDuration;

            return await publishedPostsQuery
                .OrderByDescending(post => post.LikeCount ?? 0)
                .ThenByDescending(post => post.PublishedAt ?? post.CreatedAt)
                .ThenByDescending(post => post.Id)
                .Take(5)
                .Select(post => new BlogPostListItem
                {
                    Id = post.Id,
                    Slug = post.Slug,
                    Title = post.Title,
                    Excerpt = post.Excerpt,
                    Content = null,
                    CoverImage = post.CoverImage,
                    PublishedAt = post.PublishedAt ?? post.CreatedAt,
                    LikeCount = post.LikeCount ?? 0
                })
                .ToListAsync();
        }) ?? [];

        var blogCards = await BuildBlogCardsAsync(pagedPosts);
        var featuredBlogCards = await BuildBlogCardsAsync(featuredPosts);
        var totalPublishedPosts = await _memoryCache.GetOrCreateAsync(BlogCacheKeys.TotalPublishedPosts, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = BlogCacheDuration;
            return await publishedPostsQuery.CountAsync();
        });

        return new BlogIndexViewModel
        {
            Posts = blogCards,
            FeaturedPosts = featuredBlogCards,
            Categories = categoryFilters,
            SearchTerm = normalizedSearchTerm,
            SelectedCategoryName = normalizedCategoryName,
            TotalPublishedPosts = totalPublishedPosts,
            CurrentPage = currentPage,
            TotalPages = totalPages,
            PageSize = pageSize,
            TotalItems = totalItems,
            ContactPhone = contactInfo.Phone
        };
    }

    private async Task<List<BlogCardViewModel>> BuildBlogCardsAsync(List<BlogPostListItem> posts)
    {
        if (posts.Count == 0)
        {
            return [];
        }

        var postIds = posts
            .Select(post => post.Id)
            .ToList();

        var categories = await _context.BlogPostCategories
            .AsNoTracking()
            .Where(postCategory => postCategory.PostId.HasValue && postIds.Contains(postCategory.PostId.Value))
            .Join(
                _context.BlogCategories.AsNoTracking(),
                postCategory => postCategory.CategoryId,
                blogCategory => (int?)blogCategory.Id,
                (postCategory, blogCategory) => new
                {
                    postCategory.PostId,
                    blogCategory.Name
                })
            .ToListAsync();

        var galleryImages = await _context.BlogImages
            .AsNoTracking()
            .Where(image => image.BlogId.HasValue && postIds.Contains(image.BlogId.Value))
            .OrderBy(image => image.OrderIndex ?? int.MaxValue)
            .ThenBy(image => image.Id)
            .Select(image => new
            {
                PostId = image.BlogId!.Value,
                ImageUrl = image.ImageUrl
            })
            .ToListAsync();

        var categoryByPostId = categories
            .Where(item => item.PostId.HasValue)
            .GroupBy(item => item.PostId!.Value)
            .ToDictionary(
                group => group.Key,
                group => group.Select(item => item.Name).FirstOrDefault(name => !string.IsNullOrWhiteSpace(name)) ?? "Tin tức");

        var imageSequenceByPostId = galleryImages
            .Where(item => !string.IsNullOrWhiteSpace(item.ImageUrl))
            .GroupBy(item => item.PostId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(item => NormalizeBlogImagePath(item.ImageUrl))
                    .Where(imageUrl => !string.IsNullOrWhiteSpace(imageUrl))
                    .ToList());

        return posts
            .Select(post =>
            {
                categoryByPostId.TryGetValue(post.Id, out var postCategoryName);
                var coverImage = NormalizeBlogImagePath(post.CoverImage);
                coverImage = _blogImageProcessor.ResolveVariantImage(coverImage, BlogImageVariant.Thumbnail);
                var gallerySequence = imageSequenceByPostId.GetValueOrDefault(post.Id) ?? [];
                var imageSequence = new List<string>();

                if (!string.IsNullOrWhiteSpace(coverImage))
                {
                    imageSequence.Add(coverImage);
                }

                imageSequence.AddRange(gallerySequence);
                imageSequence = imageSequence
                    .Where(imageUrl => !string.IsNullOrWhiteSpace(imageUrl))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (imageSequence.Count == 0)
                {
                    imageSequence.Add(NormalizeBlogImagePath(null));
                }

                return new BlogCardViewModel
                {
                    Id = post.Id,
                    Slug = BuildBlogSlug(post.Slug, post.Id),
                    Title = post.Title ?? "Tin tức đang cập nhật",
                    Summary = BuildBlogSummary(post.Excerpt, post.Content),
                    CoverImage = imageSequence[0],
                    ImageSequence = imageSequence,
                    CategoryName = postCategoryName ?? "Tin tức",
                    PublishedAt = post.PublishedAt,
                    LikeCount = post.LikeCount
                };
            })
            .ToList();
    }

    private async Task<List<BlogCategoryFilterViewModel>> BuildCategoryFiltersAsync(IQueryable<BlogPost>? publishedPostsQuery = null)
    {
        if (publishedPostsQuery is null)
        {
            return await _memoryCache.GetOrCreateAsync(BlogCacheKeys.CategoryFilters, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = BlogCacheDuration;
                return await BuildCategoryFiltersUncachedAsync();
            }) ?? [];
        }

        return await BuildCategoryFiltersUncachedAsync(publishedPostsQuery);
    }

    private async Task<List<BlogCategoryFilterViewModel>> BuildCategoryFiltersUncachedAsync(IQueryable<BlogPost>? publishedPostsQuery = null)
    {
        publishedPostsQuery ??= _context.BlogPosts
            .AsNoTracking()
            .Where(post => post.Status == "published");

        var categoryFilters = await _context.BlogCategories
            .AsNoTracking()
            .Select(category => new BlogCategoryFilterViewModel
            {
                Name = category.Name ?? string.Empty,
                PostCount = _context.BlogPostCategories
                    .Where(postCategory => postCategory.CategoryId == category.Id && postCategory.PostId.HasValue)
                    .Join(
                        publishedPostsQuery,
                        postCategory => postCategory.PostId!.Value,
                        post => post.Id,
                        (postCategory, post) => post.Id)
                    .Distinct()
                    .Count()
            })
            .ToListAsync();

        return categoryFilters
            .Where(category => !string.IsNullOrWhiteSpace(category.Name))
            .OrderBy(category => category.Name)
            .ToList();
    }

    private bool TryGetUsefulCooldownRemaining(int postId, out int cooldownSecondsRemaining)
    {
        cooldownSecondsRemaining = 0;

        var sessionKey = BuildUsefulSessionKey(postId);
        var rawNextAvailableAt = HttpContext.Session.GetString(sessionKey);

        if (string.IsNullOrWhiteSpace(rawNextAvailableAt)
            || !DateTimeOffset.TryParse(rawNextAvailableAt, out var nextAvailableAt))
        {
            return false;
        }

        var remaining = nextAvailableAt - DateTimeOffset.UtcNow;

        if (remaining <= TimeSpan.Zero)
        {
            HttpContext.Session.Remove(sessionKey);
            return false;
        }

        cooldownSecondsRemaining = Math.Max(1, (int)Math.Ceiling(remaining.TotalSeconds));
        return true;
    }

    private static string BuildUsefulCooldownMessage(int cooldownSecondsRemaining)
    {
        var cooldownMinutesRemaining = Math.Max(1, (int)Math.Ceiling(cooldownSecondsRemaining / 60d));
        return $"Bạn chỉ có thể nhấn lại sau {cooldownMinutesRemaining} phút.";
    }

    private static string BuildUsefulSessionKey(int postId)
    {
        return $"{UsefulSessionKeyPrefix}{postId}";
    }

    private static string BuildBlogSummary(string? excerpt, string? content)
    {
        var summary = !string.IsNullOrWhiteSpace(excerpt)
            ? excerpt
            : content;

        if (string.IsNullOrWhiteSpace(summary))
        {
            return "Nội dung bài viết đang được cập nhật.";
        }

        summary = WebUtility.HtmlDecode(summary);
        summary = Regex.Replace(summary, "<.*?>", string.Empty).Trim();
        return summary.Length > 180 ? $"{summary[..177]}..." : summary;
    }

    private static string BuildBlogSlug(string? slug, int id)
    {
        return !string.IsNullOrWhiteSpace(slug)
            ? slug.Trim()
            : id.ToString();
    }

    private static string NormalizeBlogImagePath(string? coverImage)
    {
        if (string.IsNullOrWhiteSpace(coverImage))
        {
            return "/assets/img/blog/blog-1.jpg";
        }

        var normalized = coverImage.Replace("\\", "/").Trim();

        if (normalized.StartsWith("~/", StringComparison.Ordinal))
        {
            return $"/{normalized[2..]}";
        }

        if (normalized.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || normalized.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || normalized.StartsWith("/", StringComparison.Ordinal))
        {
            return normalized;
        }

        return $"/{normalized.TrimStart('/')}";
    }

    private sealed class BlogPostListItem
    {
        public int Id { get; init; }

        public string? Slug { get; init; }

        public string? Title { get; init; }

        public string? Excerpt { get; init; }

        public string? Content { get; init; }

        public string? CoverImage { get; init; }

        public DateTime? PublishedAt { get; init; }

        public int LikeCount { get; init; }
    }
}
