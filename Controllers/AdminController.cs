using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.Controllers;

[Route("admin")]
public class AdminController : Controller
{
    private const string EditorInputPrefix = "Editor.Input";
    private const string CategoryEditorInputPrefix = "CategoryEditor.Input";
    private const string SuccessTempDataKey = "AdminPostsSuccess";
    private const string ErrorTempDataKey = "AdminPostsError";
    private const int PostsPageSize = 8;
    private static readonly string[] CategoryTonePalette = ["blue", "green", "violet", "orange", "rose", "slate"];
    private static readonly HashSet<string> ValidBlogStatuses = ["draft", "review", "published", "hidden"];
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    [HttpGet("overview")]
    public async Task<IActionResult> Overview()
    {
        var model = new AdminOverviewViewModel
        {
            TotalProducts = await _context.Products.CountAsync(),
            TotalPublishedPosts = await _context.BlogPosts.CountAsync(post => post.PublishedAt != null),
            NewContactRequests = await _context.ContactRequests.CountAsync(request =>
                request.Status == null || request.Status == "new" || request.Status == "moi"),
            LatestPriceUpdateDate = await _context.PriceHistories
                .OrderByDescending(price => price.EffectiveDate)
                .Select(price => price.EffectiveDate)
                .FirstOrDefaultAsync(),
            ProductsWithoutImages = await _context.Products.CountAsync(product =>
                (product.PrimaryImage == null || product.PrimaryImage == string.Empty) && !product.ProductImages.Any()),
            BlogsWithoutImages = await _context.BlogPosts.CountAsync(post =>
                (post.CoverImage == null || post.CoverImage == string.Empty) && !post.BlogImages.Any())
        };

        ViewData["Title"] = "Tổng quan";
        ViewData["AdminSection"] = "Overview";
        return View(model);
    }

    [HttpGet("products")]
    public IActionResult Products()
    {
        ViewData["Title"] = "Sản phẩm";
        ViewData["AdminSection"] = "Products";
        return View();
    }

    [HttpGet("prices")]
    public IActionResult Prices()
    {
        ViewData["Title"] = "Bảng giá";
        ViewData["AdminSection"] = "Prices";
        return View();
    }

    [HttpGet("posts")]
    public async Task<IActionResult> Posts(string? searchTerm, string? status, int? categoryId, int? editId, int? page)
    {
        var model = await BuildAdminPostsPageViewModelAsync(
            searchTerm,
            status,
            categoryId,
            editId,
            null,
            TempData[SuccessTempDataKey]?.ToString(),
            TempData[ErrorTempDataKey]?.ToString(),
            page: page);

        if (editId.HasValue && model.Editor.Input.Id != editId.Value)
        {
            model.ErrorMessage = string.IsNullOrWhiteSpace(model.ErrorMessage)
                ? "Không tìm thấy bài viết cần chỉnh sửa."
                : model.ErrorMessage;
        }

        return RenderPostsView(model);
    }

    [HttpPost("posts/save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePost([Bind(Prefix = EditorInputPrefix)] AdminPostEditorInputModel input, List<IFormFile>? imageFiles, List<IFormFile>? galleryFiles)
    {
        input.Title = input.Title?.Trim() ?? string.Empty;
        input.Slug = input.Slug?.Trim() ?? string.Empty;
        input.Excerpt = input.Excerpt?.Trim() ?? string.Empty;
        input.Content = input.Content?.Trim() ?? string.Empty;
        input.CoverImage = input.CoverImage?.Trim() ?? string.Empty;
        input.GalleryInput = input.GalleryInput?.Trim() ?? string.Empty;
        input.SearchTerm = input.SearchTerm?.Trim() ?? string.Empty;
        input.ReturnStatus = NormalizeStatusFilter(input.ReturnStatus);
        input.ReturnCategoryId = input.ReturnCategoryId > 0 ? input.ReturnCategoryId : null;
        input.ReturnPage = input.ReturnPage > 0 ? input.ReturnPage : 1;
        input.PrimaryCategoryId = input.PrimaryCategoryId > 0 ? input.PrimaryCategoryId : null;
        input.SelectedCategoryIds = input.SelectedCategoryIds
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        if (!input.PrimaryCategoryId.HasValue && input.SelectedCategoryIds.Count > 0)
        {
            input.PrimaryCategoryId = input.SelectedCategoryIds[0];
        }
        input.SelectedProductIds = input.SelectedProductIds
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        var uploadedFiles = (imageFiles ?? [])
            .Where(file => file is { Length: > 0 })
            .ToList();
        var uploadedGalleryFiles = (galleryFiles ?? [])
            .Where(file => file is { Length: > 0 })
            .ToList();

        var allowedImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };
        const long maxImageFileSize = 5 * 1024 * 1024;

        foreach (var imageFile in uploadedFiles)
        {
            var extension = Path.GetExtension(imageFile.FileName);

            if (!allowedImageExtensions.Contains(extension))
            {
                ModelState.AddModelError("ImageFiles", "Chỉ cho phép tải lên ảnh JPG, JPEG, PNG hoặc WEBP.");
            }

            if (imageFile.Length > maxImageFileSize)
            {
                ModelState.AddModelError("ImageFiles", "Mỗi ảnh tải lên phải nhỏ hơn 5MB.");
            }
        }

        foreach (var imageFile in uploadedGalleryFiles)
        {
            var extension = Path.GetExtension(imageFile.FileName);

            if (!allowedImageExtensions.Contains(extension))
            {
                ModelState.AddModelError("GalleryFiles", "Chỉ cho phép tải lên ảnh JPG, JPEG, PNG hoặc WEBP.");
            }

            if (imageFile.Length > maxImageFileSize)
            {
                ModelState.AddModelError("GalleryFiles", "Mỗi ảnh tải lên phải nhỏ hơn 5MB.");
            }
        }

        var galleryItems = ParseGalleryInput(input.GalleryInput);
        var galleryUploadTokenCount = galleryItems.Count(item => IsGalleryUploadToken(item.ImageUrl));
        var unifiedGalleryUploadCount = Math.Max(0, uploadedFiles.Count - 1);
        var usesUnifiedUploadFlow = uploadedGalleryFiles.Count == 0 && galleryUploadTokenCount > 0;

        if (uploadedGalleryFiles.Count > 0 && galleryUploadTokenCount != uploadedGalleryFiles.Count)
        {
            ModelState.AddModelError("GalleryFiles", "Dữ liệu ảnh gallery không khớp với caption đã nhập. Vui lòng chọn lại ảnh gallery trước khi lưu.");
        }
        else if (usesUnifiedUploadFlow && galleryUploadTokenCount != unifiedGalleryUploadCount)
        {
            ModelState.AddModelError("ImageFiles", "Số ảnh gallery không khớp với danh sách upload hiện tại. Vui lòng chọn lại ảnh trước khi lưu.");
        }

        TryValidateModel(input, EditorInputPrefix);

        if (!IsValidBlogStatus(input.Status))
        {
            ModelState.AddModelError($"{EditorInputPrefix}.{nameof(AdminPostEditorInputModel.Status)}", "Trạng thái bài viết không hợp lệ.");
        }

        if (input.SelectedCategoryIds.Count == 0)
        {
            ModelState.AddModelError($"{EditorInputPrefix}.{nameof(AdminPostEditorInputModel.SelectedCategoryIds)}", "Chọn ít nhất một chuyên mục cho bài viết.");
        }

        var existingAuthorIds = await _context.Admins
            .AsNoTracking()
            .Select(admin => admin.Id)
            .ToListAsync();

        if (input.AuthorId.HasValue && !existingAuthorIds.Contains(input.AuthorId.Value))
        {
            ModelState.AddModelError($"{EditorInputPrefix}.{nameof(AdminPostEditorInputModel.AuthorId)}", "Tác giả được chọn không tồn tại.");
        }

        var existingCategoryIds = await _context.BlogCategories
            .AsNoTracking()
            .Select(category => category.Id)
            .ToListAsync();

        if (input.SelectedCategoryIds.Any(id => !existingCategoryIds.Contains(id)))
        {
            ModelState.AddModelError($"{EditorInputPrefix}.{nameof(AdminPostEditorInputModel.SelectedCategoryIds)}", "Có chuyên mục không hợp lệ trong danh sách đã chọn.");
        }

        if (input.PrimaryCategoryId.HasValue && !input.SelectedCategoryIds.Contains(input.PrimaryCategoryId.Value))
        {
            ModelState.AddModelError($"{EditorInputPrefix}.{nameof(AdminPostEditorInputModel.PrimaryCategoryId)}", "Chuyên mục lưu ảnh phải nằm trong danh sách chuyên mục đã chọn.");
        }

        var selectedCategories = await _context.BlogCategories
            .AsNoTracking()
            .Where(category => input.SelectedCategoryIds.Contains(category.Id))
            .ToListAsync();

        var existingProductIds = await _context.Products
            .AsNoTracking()
            .Select(product => product.Id)
            .ToListAsync();

        if (input.SelectedProductIds.Any(id => !existingProductIds.Contains(id)))
        {
            ModelState.AddModelError($"{EditorInputPrefix}.{nameof(AdminPostEditorInputModel.SelectedProductIds)}", "Có sản phẩm liên quan không hợp lệ trong danh sách đã chọn.");
        }

        if (!ModelState.IsValid)
        {
            input.GalleryInput = SerializeGalleryInput(galleryItems.Where(item => !IsGalleryUploadToken(item.ImageUrl)));

            var invalidModel = await BuildAdminPostsPageViewModelAsync(
                input.SearchTerm,
                input.ReturnStatus,
                input.ReturnCategoryId,
                input.Id,
                input,
                null,
                "Vui lòng kiểm tra lại dữ liệu bài viết trước khi lưu.",
                page: input.ReturnPage);

            return RenderPostsView(invalidModel);
        }

        var now = DateTime.Now;
        var normalizedStatus = NormalizeBlogStatus(input.Status);
        var savedUploadedFilePaths = new List<string>();
        HashSet<string> previousPostImageUrls = [];

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            BlogPost post;
            var isNewPost = !input.Id.HasValue;

            if (isNewPost)
            {
                post = new BlogPost
                {
                    CreatedAt = now,
                    LikeCount = 0
                };

                _context.BlogPosts.Add(post);
            }
            else
            {
                post = await _context.BlogPosts.FirstOrDefaultAsync(item => item.Id == input.Id!.Value)
                    ?? throw new InvalidOperationException("Không tìm thấy bài viết để cập nhật.");

                previousPostImageUrls = await GetPostImageUrlsAsync(post.Id);
            }

            var uniqueSlug = await BuildUniqueBlogSlugAsync(input.Slug, input.Title, post.Id == 0 ? null : post.Id);
            var categoryFolderName = ResolveBlogImageCategoryFolder(selectedCategories, input.SelectedCategoryIds, input.PrimaryCategoryId);
            var uploadedImageUrls = await SaveUploadedPostImagesAsync(uploadedFiles, uniqueSlug, categoryFolderName, savedUploadedFilePaths);
            var uploadedGalleryUrls = await SaveUploadedPostImagesAsync(uploadedGalleryFiles, uniqueSlug, categoryFolderName, savedUploadedFilePaths);

            if (uploadedGalleryFiles.Count > 0)
            {
                galleryItems = ReplaceGalleryUploadTokens(galleryItems, uploadedGalleryUrls);
            }
            else if (usesUnifiedUploadFlow)
            {
                galleryItems = ReplaceGalleryUploadTokens(galleryItems, uploadedImageUrls.Skip(1).ToList());
            }

            if (uploadedImageUrls.Count > 0)
            {
                input.CoverImage = uploadedImageUrls[0];

                if (!usesUnifiedUploadFlow)
                {
                    var uploadedCoverGalleryItems = uploadedImageUrls
                        .Skip(1)
                        .Select((imageUrl, index) => new AdminGalleryInputItem
                        {
                            ImageUrl = imageUrl,
                            Caption = string.Empty,
                            OrderIndex = index + 1
                        })
                        .ToList();

                    if (uploadedCoverGalleryItems.Count > 0)
                    {
                        galleryItems = uploadedCoverGalleryItems
                            .Concat(galleryItems)
                            .ToList();
                    }
                }
            }

            input.CoverImage = NormalizeStoredImagePath(input.CoverImage) ?? string.Empty;

            galleryItems = galleryItems
                .Select(item => new AdminGalleryInputItem
                {
                    ImageUrl = NormalizeStoredImagePath(item.ImageUrl) ?? string.Empty,
                    Caption = item.Caption?.Trim() ?? string.Empty,
                    OrderIndex = item.OrderIndex
                })
                .Where(item => !string.IsNullOrWhiteSpace(item.ImageUrl))
                .Select((item, index) => new AdminGalleryInputItem
                {
                    ImageUrl = item.ImageUrl,
                    Caption = item.Caption,
                    OrderIndex = index + 1
                })
                .ToList();

            post.Title = input.Title;
            post.Slug = uniqueSlug;
            post.Excerpt = NormalizeNullableText(input.Excerpt);
            post.Content = input.Content;
            post.CoverImage = NormalizeNullableText(input.CoverImage);
            post.AuthorId = input.AuthorId;
            post.Status = normalizedStatus;
            post.PublishedAt = ResolvePublishedAt(input.PublishedAt, normalizedStatus, post.PublishedAt, now);
            post.UpdatedAt = now;

            await _context.SaveChangesAsync();

            await ReplacePostCategoriesAsync(post.Id, input.SelectedCategoryIds);
            await ReplacePostProductsAsync(post.Id, input.SelectedProductIds);
            await ReplacePostGalleryAsync(post.Id, galleryItems);

            await _context.SaveChangesAsync();

            var currentPostImageUrls = await GetPostImageUrlsAsync(post.Id);
            await transaction.CommitAsync();

            await DeleteUnreferencedBlogImageFilesAsync(
                previousPostImageUrls.Except(currentPostImageUrls, StringComparer.OrdinalIgnoreCase));

            TempData[SuccessTempDataKey] = isNewPost
                ? "Đã tạo bài viết mới thành công."
                : "Đã cập nhật bài viết thành công.";

            return RedirectToAction(nameof(Posts), new
            {
                searchTerm = input.SearchTerm,
                status = string.IsNullOrWhiteSpace(input.ReturnStatus) ? null : input.ReturnStatus,
                categoryId = input.ReturnCategoryId,
                editId = post.Id,
                page = input.ReturnPage > 1 ? input.ReturnPage : null
            });
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();

            foreach (var savedFilePath in savedUploadedFilePaths)
            {
                if (System.IO.File.Exists(savedFilePath))
                {
                    System.IO.File.Delete(savedFilePath);
                }
            }

            input.GalleryInput = SerializeGalleryInput(galleryItems.Where(item => !IsGalleryUploadToken(item.ImageUrl)));

            var errorModel = await BuildAdminPostsPageViewModelAsync(
                input.SearchTerm,
                input.ReturnStatus,
                input.ReturnCategoryId,
                input.Id,
                input,
                null,
                $"Không thể lưu bài viết. {exception.Message}",
                page: input.ReturnPage);

            return RenderPostsView(errorModel);
        }
    }

    [HttpPost("posts/change-status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePostStatus(int id, string status, string? searchTerm, string? returnStatus, int? returnCategoryId, int? editId, int? returnPage)
    {
        var normalizedStatus = NormalizeBlogStatus(status);
        returnPage = returnPage > 0 ? returnPage : 1;

        if (!IsValidBlogStatus(status))
        {
            TempData[ErrorTempDataKey] = "Trạng thái cần cập nhật không hợp lệ.";
            return RedirectToPosts(searchTerm, returnStatus, returnCategoryId, editId, returnPage);
        }

        var post = await _context.BlogPosts.FirstOrDefaultAsync(item => item.Id == id);

        if (post is null)
        {
            TempData[ErrorTempDataKey] = "Không tìm thấy bài viết để cập nhật trạng thái.";
            return RedirectToPosts(searchTerm, returnStatus, returnCategoryId, editId, returnPage);
        }

        post.Status = normalizedStatus;
        post.PublishedAt = ResolvePublishedAt(post.PublishedAt, normalizedStatus, post.PublishedAt, DateTime.Now);
        post.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        TempData[SuccessTempDataKey] = $"Đã chuyển bài viết #{post.Id} sang trạng thái {BuildBlogStatusLabel(normalizedStatus).ToLowerInvariant()}.";
        return RedirectToPosts(searchTerm, returnStatus, returnCategoryId, editId == id ? id : editId, returnPage);
    }

    [HttpPost("posts/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePost(int id, string? searchTerm, string? returnStatus, int? returnCategoryId, int? editId, int? returnPage)
    {
        returnPage = returnPage > 0 ? returnPage : 1;
        var post = await _context.BlogPosts.FirstOrDefaultAsync(item => item.Id == id);

        if (post is null)
        {
            TempData[ErrorTempDataKey] = "Không tìm thấy bài viết để xoá.";
            return RedirectToPosts(searchTerm, returnStatus, returnCategoryId, editId, returnPage);
        }

        var deletedImageUrls = await GetPostImageUrlsAsync(id);

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var categoryMappings = await _context.BlogPostCategories
                .Where(mapping => mapping.PostId == id)
                .ToListAsync();

            var productMappings = await _context.BlogPostProducts
                .Where(mapping => mapping.PostId == id)
                .ToListAsync();

            var galleryImages = await _context.BlogImages
                .Where(image => image.BlogId == id)
                .ToListAsync();

            _context.BlogPostCategories.RemoveRange(categoryMappings);
            _context.BlogPostProducts.RemoveRange(productMappings);
            _context.BlogImages.RemoveRange(galleryImages);
            _context.BlogPosts.Remove(post);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            await DeleteUnreferencedBlogImageFilesAsync(deletedImageUrls);

            TempData[SuccessTempDataKey] = $"Đã xoá bài viết #{id} cùng toàn bộ dữ liệu liên quan.";

            return RedirectToPosts(
                searchTerm,
                returnStatus,
                returnCategoryId,
                editId == id ? null : editId,
                returnPage);
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            TempData[ErrorTempDataKey] = $"Không thể xoá bài viết. {exception.Message}";
            return RedirectToPosts(searchTerm, returnStatus, returnCategoryId, editId, returnPage);
        }
    }

    [HttpPost("posts/categories/save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCategory([Bind(Prefix = CategoryEditorInputPrefix)] AdminCategoryEditorInputModel input)
    {
        input.Name = input.Name?.Trim() ?? string.Empty;
        input.Slug = input.Slug?.Trim();
        input.Description = input.Description?.Trim();
        input.SearchTerm = input.SearchTerm?.Trim();
        input.ReturnStatus = NormalizeStatusFilter(input.ReturnStatus);
        input.ReturnCategoryId = input.ReturnCategoryId > 0 ? input.ReturnCategoryId : null;
        input.ReturnPage = input.ReturnPage > 0 ? input.ReturnPage : 1;
        input.Slug = BuildSeoFriendlySlug(!string.IsNullOrWhiteSpace(input.Slug) ? input.Slug : input.Name);

        TryValidateModel(input, CategoryEditorInputPrefix);

        if (string.IsNullOrWhiteSpace(input.Slug))
        {
            ModelState.AddModelError($"{CategoryEditorInputPrefix}.{nameof(AdminCategoryEditorInputModel.Slug)}", "Slug chuyên mục không hợp lệ.");
        }

        var existingCategories = await _context.BlogCategories
            .AsNoTracking()
            .Select(category => new
            {
                category.Name,
                category.Slug
            })
            .ToListAsync();

        if (existingCategories.Any(category => string.Equals(category.Name?.Trim(), input.Name, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError($"{CategoryEditorInputPrefix}.{nameof(AdminCategoryEditorInputModel.Name)}", "Tên chuyên mục đã tồn tại.");
        }

        if (existingCategories.Any(category => string.Equals(category.Slug?.Trim(), input.Slug, StringComparison.OrdinalIgnoreCase)))
        {
            ModelState.AddModelError($"{CategoryEditorInputPrefix}.{nameof(AdminCategoryEditorInputModel.Slug)}", "Slug chuyên mục đã tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildAdminPostsPageViewModelAsync(
                input.SearchTerm,
                input.ReturnStatus,
                input.ReturnCategoryId,
                null,
                null,
                null,
                "Vui lòng kiểm tra lại dữ liệu chuyên mục trước khi lưu.",
                input,
                "categories",
                input.ReturnPage);

            return RenderPostsView(invalidModel);
        }

        try
        {
            var now = DateTime.Now;
            var category = new BlogCategory
            {
                Name = input.Name,
                Slug = input.Slug,
                Description = NormalizeNullableText(input.Description),
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.BlogCategories.Add(category);
            await _context.SaveChangesAsync();

            TempData[SuccessTempDataKey] = $"Đã thêm chuyên mục \"{input.Name}\" thành công.";
            return RedirectToPostsSection(input.SearchTerm, input.ReturnStatus, input.ReturnCategoryId, null, "categories", input.ReturnPage);
        }
        catch (Exception exception)
        {
            var errorModel = await BuildAdminPostsPageViewModelAsync(
                input.SearchTerm,
                input.ReturnStatus,
                input.ReturnCategoryId,
                null,
                null,
                null,
                $"Không thể lưu chuyên mục. {exception.Message}",
                input,
                "categories",
                input.ReturnPage);

            return RenderPostsView(errorModel);
        }
    }

    [HttpGet("marketing")]
    public IActionResult Marketing()
    {
        ViewData["Title"] = "Banner & SEO";
        ViewData["AdminSection"] = "Marketing";
        return View();
    }

    private async Task<AdminPostsPageViewModel> BuildAdminPostsPageViewModelAsync(
        string? searchTerm,
        string? status,
        int? categoryId,
        int? editId,
        AdminPostEditorInputModel? editorInput,
        string? successMessage,
        string? errorMessage,
        AdminCategoryEditorInputModel? categoryInput = null,
        string? activeSection = null,
        int? page = null)
    {
        var culture = new CultureInfo("vi-VN");
        var now = DateTime.Now;
        var last7Days = now.AddDays(-7);
        var refreshThreshold = now.AddDays(-30);
        var normalizedSearchTerm = searchTerm?.Trim() ?? editorInput?.SearchTerm?.Trim() ?? string.Empty;
        var normalizedStatusFilter = NormalizeStatusFilter(status ?? editorInput?.ReturnStatus);
        var selectedCategoryId = categoryId > 0 ? categoryId : editorInput?.ReturnCategoryId > 0 ? editorInput.ReturnCategoryId : null;

        var authors = await _context.Admins
            .AsNoTracking()
            .OrderBy(admin => admin.FullName ?? admin.Username)
            .ThenBy(admin => admin.Id)
            .Select(admin => new AdminAuthorRecord
            {
                Id = admin.Id,
                FullName = admin.FullName,
                Username = admin.Username,
                Status = admin.Status
            })
            .ToListAsync();

        var categories = await _context.BlogCategories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ThenBy(category => category.Id)
            .ToListAsync();

        var products = await _context.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Id)
            .ToListAsync();

        var postRecords = await _context.BlogPosts
            .AsNoTracking()
            .OrderByDescending(post => post.PublishedAt ?? post.UpdatedAt ?? post.CreatedAt)
            .ThenByDescending(post => post.Id)
            .Select(post => new AdminBlogPostRecord
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Excerpt = post.Excerpt,
                Content = post.Content,
                CoverImage = post.CoverImage,
                AuthorId = post.AuthorId,
                AuthorName = post.Author != null ? post.Author.FullName : null,
                Status = post.Status,
                PublishedAt = post.PublishedAt,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                LikeCount = post.LikeCount ?? 0
            })
            .ToListAsync();

        var postIds = postRecords
            .Select(post => post.Id)
            .ToList();

        var categoryMappings = postIds.Count == 0
            ? []
            : await _context.BlogPostCategories
                .AsNoTracking()
                .Where(mapping =>
                    mapping.PostId.HasValue
                    && mapping.CategoryId.HasValue
                    && postIds.Contains(mapping.PostId.Value))
                .Join(
                    _context.BlogCategories.AsNoTracking(),
                    mapping => mapping.CategoryId!.Value,
                    category => category.Id,
                    (mapping, category) => new AdminBlogCategoryMapping
                    {
                        PostId = mapping.PostId!.Value,
                        CategoryId = category.Id,
                        CategoryName = category.Name,
                        CategorySlug = category.Slug
                    })
                .ToListAsync();

        var productMappings = postIds.Count == 0
            ? []
            : await _context.BlogPostProducts
                .AsNoTracking()
                .Where(mapping =>
                    mapping.PostId.HasValue
                    && mapping.ProductId.HasValue
                    && postIds.Contains(mapping.PostId.Value))
                .Join(
                    _context.Products.AsNoTracking(),
                    mapping => mapping.ProductId!.Value,
                    product => product.Id,
                    (mapping, product) => new AdminBlogProductMapping
                    {
                        PostId = mapping.PostId!.Value,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ProductSlug = product.Slug
                    })
                .ToListAsync();

        var galleryRecords = postIds.Count == 0
            ? []
            : await _context.BlogImages
                .AsNoTracking()
                .Where(image => image.BlogId.HasValue && postIds.Contains(image.BlogId.Value))
                .OrderBy(image => image.OrderIndex ?? int.MaxValue)
                .ThenBy(image => image.Id)
                .Select(image => new AdminBlogImageRecord
                {
                    PostId = image.BlogId!.Value,
                    ImageUrl = image.ImageUrl,
                    Caption = image.Caption,
                    OrderIndex = image.OrderIndex ?? 0
                })
                .ToListAsync();

        var postRecordById = postRecords.ToDictionary(post => post.Id);
        var categoryMappingsByPostId = categoryMappings
            .GroupBy(mapping => mapping.PostId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(mapping => mapping.CategoryName)
                    .ThenBy(mapping => mapping.CategoryId)
                    .ToList());
        var productMappingsByPostId = productMappings
            .GroupBy(mapping => mapping.PostId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(mapping => mapping.ProductName)
                    .ThenBy(mapping => mapping.ProductId)
                    .ToList());
        var galleryByPostId = galleryRecords
            .GroupBy(record => record.PostId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderBy(record => record.OrderIndex)
                    .ThenBy(record => record.ImageUrl)
                    .ToList());

        var categoryPostCounts = categoryMappings
            .GroupBy(mapping => mapping.CategoryId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(mapping => mapping.PostId)
                    .Distinct()
                    .Count());

        var allPosts = postRecords
            .Select(post =>
            {
                var postCategories = categoryMappingsByPostId.GetValueOrDefault(post.Id) ?? [];
                var postProducts = productMappingsByPostId.GetValueOrDefault(post.Id) ?? [];
                var postGallery = galleryByPostId.GetValueOrDefault(post.Id) ?? [];
                var primaryCategory = postCategories.FirstOrDefault();
                var normalizedStatus = NormalizeBlogStatus(post.Status);
                var createdAt = post.CreatedAt ?? post.PublishedAt ?? post.UpdatedAt ?? now;
                var updatedAt = post.UpdatedAt ?? post.PublishedAt ?? post.CreatedAt ?? createdAt;
                var publishedAt = normalizedStatus == "published"
                    ? post.PublishedAt ?? createdAt
                    : post.PublishedAt;
                var hasCover = !string.IsNullOrWhiteSpace(post.CoverImage);
                var hasCategory = postCategories.Count > 0;
                var seoScore = CalculateSeoScore(
                    post,
                    hasCover || postGallery.Count > 0,
                    hasCategory,
                    postProducts.Count,
                    normalizedStatus == "published");
                var statusActions = BuildStatusActions(normalizedStatus);
                var publicUrl = Url.Action("Detail", "Blog", new { slug = BuildBlogSlug(post.Slug, post.Id) }) ?? string.Empty;

                return new AdminPostRowViewModel
                {
                    Id = post.Id,
                    Title = !string.IsNullOrWhiteSpace(post.Title) ? post.Title.Trim() : $"Bài viết #{post.Id}",
                    Slug = BuildBlogSlug(post.Slug, post.Id),
                    PrimaryCategoryId = primaryCategory?.CategoryId,
                    CategoryIds = postCategories.Select(item => item.CategoryId).Distinct().ToList(),
                    Category = primaryCategory?.CategoryName?.Trim() ?? "Chưa phân loại",
                    CategorySlug = primaryCategory?.CategorySlug?.Trim() ?? "no-category",
                    Author = !string.IsNullOrWhiteSpace(post.AuthorName) ? post.AuthorName.Trim() : "Chưa gán tác giả",
                    Status = normalizedStatus,
                    StatusLabel = BuildBlogStatusLabel(normalizedStatus),
                    StatusClass = BuildBlogStatusClass(normalizedStatus),
                    PublishedAt = publishedAt,
                    UpdatedAt = updatedAt,
                    CreatedAt = createdAt,
                    PublishText = BuildPublishText(normalizedStatus, publishedAt, culture),
                    PublishHint = BuildPublishHint(normalizedStatus, publishedAt),
                    RelatedProducts = postProducts.Count,
                    RelatedProductSummary = BuildProductSummary(postProducts),
                    GalleryImages = postGallery.Count,
                    GallerySummary = postGallery.Count == 0 ? "Chưa có gallery" : $"{postGallery.Count} ảnh gallery",
                    Likes = post.LikeCount,
                    HasCover = hasCover,
                    CoverText = hasCover ? "Có cover" : postGallery.Count > 0 ? "Dùng gallery" : "Thiếu cover",
                    CoverClass = hasCover ? "ready" : "missing",
                    SeoScore = seoScore,
                    SeoClass = BuildSeoClass(seoScore),
                    Excerpt = BuildAdminExcerpt(post.Excerpt, post.Content),
                    PrimaryAction = statusActions.PrimaryLabel,
                    PrimaryActionValue = statusActions.PrimaryValue,
                    SecondaryAction = statusActions.SecondaryLabel,
                    SecondaryActionValue = statusActions.SecondaryValue,
                    CreatedText = createdAt.ToString("dd/MM/yyyy", culture),
                    UpdatedText = updatedAt.ToString("dd/MM HH:mm", culture),
                    PublicUrl = publicUrl
                };
            })
            .OrderByDescending(post => post.PublishedAt ?? post.UpdatedAt)
            .ThenByDescending(post => post.Id)
            .ToList();

        var filteredPosts = allPosts
            .Where(post => MatchesSearch(post, normalizedSearchTerm))
            .Where(post => MatchesStatusFilter(post, normalizedStatusFilter))
            .Where(post => !selectedCategoryId.HasValue || post.CategoryIds.Contains(selectedCategoryId.Value))
            .ToList();

        var totalFilteredPosts = filteredPosts.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalFilteredPosts / (double)PostsPageSize));
        var currentPage = Math.Clamp(page ?? editorInput?.ReturnPage ?? categoryInput?.ReturnPage ?? 1, 1, totalPages);
        var pagedPosts = filteredPosts
            .Skip((currentPage - 1) * PostsPageSize)
            .Take(PostsPageSize)
            .ToList();
        var startItemIndex = totalFilteredPosts == 0 ? 0 : ((currentPage - 1) * PostsPageSize) + 1;
        var endItemIndex = totalFilteredPosts == 0 ? 0 : Math.Min(currentPage * PostsPageSize, totalFilteredPosts);

        var totalPosts = allPosts.Count;
        var publishedCount = allPosts.Count(post => post.Status == "published");
        var draftCount = allPosts.Count(post => post.Status == "draft");
        var reviewCount = allPosts.Count(post => post.Status == "review");
        var hiddenCount = allPosts.Count(post => post.Status == "hidden");
        var missingCoverCount = allPosts.Count(post => !post.HasCover);
        var activeCategoryCount = categoryPostCounts.Count(item => item.Value > 0);
        var recentUpdatesCount = allPosts.Count(post => post.UpdatedAt >= last7Days || post.CreatedAt >= last7Days);
        var needsUpdateCount = allPosts.Count(post => post.Status == "published" && post.UpdatedAt < refreshThreshold);
        var averageSeoScore = totalPosts == 0 ? 0 : (int)Math.Round(allPosts.Average(post => post.SeoScore));
        var totalGalleryImages = galleryRecords.Count;
        var totalRelationMappings = productMappings.Count;
        var hasActiveFilters = !string.IsNullOrWhiteSpace(normalizedSearchTerm)
            || !string.IsNullOrWhiteSpace(normalizedStatusFilter)
            || selectedCategoryId.HasValue;

        var basePostsUrl = Url.Action(nameof(Posts), "Admin", new
        {
            searchTerm = normalizedSearchTerm,
            status = string.IsNullOrWhiteSpace(normalizedStatusFilter) ? null : normalizedStatusFilter,
            categoryId = selectedCategoryId,
            page = currentPage > 1 ? (int?)currentPage : null
        }) ?? "/admin/posts";

        var categoryCards = categories
            .Select((category, index) => new AdminPostCategoryCardViewModel
            {
                Id = category.Id,
                Name = category.Name?.Trim() ?? $"Chuyên mục #{category.Id}",
                Slug = category.Slug?.Trim() ?? $"category-{category.Id}",
                TotalPosts = categoryPostCounts.GetValueOrDefault(category.Id),
                Description = !string.IsNullOrWhiteSpace(category.Description)
                    ? category.Description.Trim()
                    : "Chuyên mục chưa có mô tả, nên bổ sung để editor phân luồng nội dung dễ hơn.",
                Tone = CategoryTonePalette[index % CategoryTonePalette.Length],
                FilterUrl = Url.Action(nameof(Posts), "Admin", new
                {
                    searchTerm = normalizedSearchTerm,
                    status = string.IsNullOrWhiteSpace(normalizedStatusFilter) ? null : normalizedStatusFilter,
                    categoryId = category.Id
                }) ?? basePostsUrl
            })
            .OrderByDescending(category => category.TotalPosts)
            .ThenBy(category => category.Name)
            .ToList();

        var editor = BuildEditorViewModel(
            editId,
            editorInput,
            normalizedSearchTerm,
            normalizedStatusFilter,
            selectedCategoryId,
            currentPage,
            basePostsUrl,
            authors,
            categories,
            products,
            postRecordById,
            categoryMappingsByPostId,
            productMappingsByPostId,
            galleryByPostId);

        var normalizedCategoryInput = categoryInput is null
            ? new AdminCategoryEditorInputModel
            {
                SearchTerm = normalizedSearchTerm,
                ReturnStatus = normalizedStatusFilter,
                ReturnCategoryId = selectedCategoryId,
                ReturnPage = currentPage
            }
            : new AdminCategoryEditorInputModel
            {
                Name = categoryInput.Name?.Trim() ?? string.Empty,
                Slug = categoryInput.Slug?.Trim(),
                Description = categoryInput.Description?.Trim(),
                SearchTerm = categoryInput.SearchTerm?.Trim() ?? normalizedSearchTerm,
                ReturnStatus = string.IsNullOrWhiteSpace(categoryInput.ReturnStatus)
                    ? normalizedStatusFilter
                    : NormalizeStatusFilter(categoryInput.ReturnStatus),
                ReturnCategoryId = categoryInput.ReturnCategoryId > 0 ? categoryInput.ReturnCategoryId : selectedCategoryId,
                ReturnPage = categoryInput.ReturnPage > 0 ? categoryInput.ReturnPage : currentPage
            };

        return new AdminPostsPageViewModel
        {
            HeaderTitle = "Quản lý bài viết blog",
            HeaderDescription = hasActiveFilters
                ? $"Đang hiển thị {totalFilteredPosts}/{totalPosts} bài viết phù hợp bộ lọc. Dữ liệu vẫn đồng bộ trực tiếp từ database blog_*."
                : totalPosts > 0
                    ? $"Đang đồng bộ {totalPosts} bài viết và {categories.Count} chuyên mục trực tiếp từ database blog_* cho khu vực admin blog."
                    : "Chưa có bài viết blog trong database để hiển thị ở khu vực admin.",
            HeaderChips = new List<string>
            {
                $"{totalFilteredPosts}/{totalPosts} bài đang hiển thị",
                $"{categories.Count} chuyên mục",
                $"{missingCoverCount} bài thiếu cover"
            },
            SearchTerm = normalizedSearchTerm,
            SelectedStatus = normalizedStatusFilter,
            SelectedCategoryId = selectedCategoryId,
            StatusFilters = BuildStatusFilters(totalPosts, publishedCount, draftCount, reviewCount, hiddenCount, missingCoverCount, normalizedStatusFilter),
            CategoryFilters = BuildCategoryFilters(categories, selectedCategoryId),
            MetricCards = new List<AdminPostMetricCardViewModel>
            {
                new() { Label = "Tổng bài viết", Value = totalPosts.ToString(culture), Meta = $"{recentUpdatesCount} cập nhật trong 7 ngày gần nhất", Tone = "blue" },
                new() { Label = "Đã xuất bản", Value = publishedCount.ToString(culture), Meta = hiddenCount > 0 ? $"{hiddenCount} bài đang ẩn khỏi site" : "Đang hiển thị ngoài site", Tone = "green" },
                new() { Label = "Nháp / chờ duyệt", Value = (draftCount + reviewCount).ToString(culture), Meta = $"{draftCount} nháp · {reviewCount} chờ duyệt", Tone = "amber" },
                new() { Label = "Thiếu cover", Value = missingCoverCount.ToString(culture), Meta = "Cần bổ sung cover_image hoặc gallery đại diện", Tone = "rose" },
                new() { Label = "Chuyên mục", Value = categories.Count.ToString(culture), Meta = $"{activeCategoryCount} chuyên mục đang có bài", Tone = "violet" },
                new() { Label = "SEO trung bình", Value = averageSeoScore.ToString(culture), Meta = "Tính từ title, slug, excerpt, media, taxonomy và liên kết", Tone = "slate" }
            },
            WorkflowItems = new List<AdminPostWorkflowItemViewModel>
            {
                new() { Label = "Bản nháp", Count = draftCount, Hint = "Nội dung đang viết hoặc chưa đủ dữ liệu để xuất bản.", Tone = "draft" },
                new() { Label = "Chờ duyệt", Count = reviewCount, Hint = "Đợi editor rà soát nội dung, CTA và cấu trúc hiển thị.", Tone = "review" },
                new() { Label = "Đã xuất bản", Count = publishedCount, Hint = hiddenCount > 0 ? $"Ngoài ra có {hiddenCount} bài đang tạm ẩn khỏi ngoài site." : "Đang hiển thị ngoài site và có thể tối ưu tiếp.", Tone = "published" },
                new() { Label = "Cần cập nhật", Count = needsUpdateCount, Hint = "Bài đã publish lâu hơn 30 ngày nên rà soát lại giá, hình và internal link.", Tone = "warning" }
            },
            QuickActions = new List<AdminPostQuickActionViewModel>
            {
                new() { Title = "Tạo bài viết mới", Description = "Khởi tạo nhanh bài viết mới với đủ title, slug, content, lịch đăng, taxonomy và media.", Cta = "Mở form biên tập", ActionUrl = $"{basePostsUrl}#editor-panel" },
                new() { Title = "Lọc và rà soát", Description = "Tìm bài theo tiêu đề, trạng thái hoặc chuyên mục để xử lý nhanh các bài cần cập nhật.", Cta = "Tới danh sách bài", ActionUrl = $"{basePostsUrl}#posts-panel" },
                new() { Title = "Gắn sản phẩm liên quan", Description = "Liên kết bài với sản phẩm bằng bảng blog_post_products để kéo lead sang trang bán hàng.", Cta = "Quản lý liên kết", ActionUrl = $"{basePostsUrl}#editor-products" },
                new() { Title = "Bổ sung gallery", Description = "Quản lý cover_image và blog_images ngay trong form chỉnh sửa bằng danh sách URL + caption.", Cta = "Quản lý media", ActionUrl = $"{basePostsUrl}#editor-media" }
            },
            EditorialChecklist = new List<string>
            {
                "Tiêu đề rõ ý định tìm kiếm và không trùng slug",
                "Excerpt đủ ngắn để làm mô tả listing và chia sẻ mạng xã hội",
                "Content có heading, CTA, liên kết nội bộ và thông tin liên hệ",
                "Cover image đúng chủ đề, gallery có caption và thứ tự hiển thị",
                "Có chuyên mục chính và ít nhất một sản phẩm liên quan khi cần chốt lead"
            },
            ImplementationNote = "Gallery dùng định dạng mỗi dòng: url | caption. Nếu chỉ có URL, hệ thống vẫn lưu ảnh với caption để trống.",
            Categories = categoryCards,
            FeatureItems = new List<AdminPostFeatureItemViewModel>
            {
                new() { Title = "CRUD bài viết", Description = "Tạo mới, chỉnh sửa, đổi trạng thái và xoá bài trực tiếp trong admin." },
                new() { Title = "Taxonomy", Description = "Gắn nhiều chuyên mục qua blog_post_categories và lọc theo category." },
                new() { Title = "Media", Description = "Cover image, gallery, caption và thứ tự ảnh trong blog_images." },
                new() { Title = "Liên kết sản phẩm", Description = "Gắn nhiều sản phẩm liên quan từ blog_post_products." },
                new() { Title = "Workflow", Description = "Quản lý draft, review, published, hidden và lịch xuất bản." }
            },
            FilterTabs = BuildStatusFilters(totalPosts, publishedCount, draftCount, reviewCount, hiddenCount, missingCoverCount, normalizedStatusFilter)
                .Select(filter => filter.Label)
                .ToList(),
            Posts = pagedPosts,
            CurrentPage = currentPage,
            TotalPages = totalPages,
            PageSize = PostsPageSize,
            TotalFilteredPosts = totalFilteredPosts,
            StartItemIndex = startItemIndex,
            EndItemIndex = endItemIndex,
            ResourceCards = new List<AdminPostResourceCardViewModel>
            {
                new() { Title = "Ảnh bìa", Description = "Kiểm tra nhanh bài đã publish nhưng chưa có cover_image để tránh listing bị rỗng ảnh.", Summary = $"{missingCoverCount:00} bài cần xử lý" },
                new() { Title = "Gallery ảnh", Description = "Theo dõi số lượng ảnh thật đang gắn trong blog_images để editor kiểm soát tư liệu cho từng bài.", Summary = $"{totalGalleryImages} ảnh đã được gắn" },
                new() { Title = "Sản phẩm liên quan", Description = "Rà soát liên kết qua blog_post_products để tối ưu điều hướng từ nội dung sang trang sản phẩm.", Summary = $"{totalRelationMappings} mapping đang hoạt động" }
            },
            ConflictNote = "Các thao tác của trang này chỉ tập trung vào module blog admin: bài viết, chuyên mục liên quan, sản phẩm liên quan và gallery ảnh.",
            SuccessMessage = successMessage ?? string.Empty,
            ErrorMessage = errorMessage ?? string.Empty,
            ActiveSection = activeSection?.Trim() ?? string.Empty,
            Editor = editor,
            CategoryEditor = new AdminCategoryEditorViewModel
            {
                SubmitLabel = "Thêm chuyên mục",
                Input = normalizedCategoryInput
            }
        };
    }

    private AdminPostEditorViewModel BuildEditorViewModel(
        int? editId,
        AdminPostEditorInputModel? editorInput,
        string normalizedSearchTerm,
        string normalizedStatusFilter,
        int? selectedCategoryId,
        int currentPage,
        string cancelUrl,
        List<AdminAuthorRecord> authors,
        List<BlogCategory> categories,
        List<Product> products,
        IReadOnlyDictionary<int, AdminBlogPostRecord> postRecordById,
        IReadOnlyDictionary<int, List<AdminBlogCategoryMapping>> categoryMappingsByPostId,
        IReadOnlyDictionary<int, List<AdminBlogProductMapping>> productMappingsByPostId,
        IReadOnlyDictionary<int, List<AdminBlogImageRecord>> galleryByPostId)
    {
        AdminPostEditorInputModel input;

        if (editorInput is not null)
        {
            input = editorInput;
            input.ReturnPage = input.ReturnPage > 0 ? input.ReturnPage : currentPage;
        }
        else if (editId.HasValue && postRecordById.TryGetValue(editId.Value, out var post))
        {
            var categoriesOfPost = categoryMappingsByPostId.GetValueOrDefault(post.Id) ?? [];
            var productsOfPost = productMappingsByPostId.GetValueOrDefault(post.Id) ?? [];
            var galleryOfPost = galleryByPostId.GetValueOrDefault(post.Id) ?? [];

            input = new AdminPostEditorInputModel
            {
                Id = post.Id,
                Title = post.Title?.Trim() ?? string.Empty,
                Slug = post.Slug?.Trim() ?? string.Empty,
                Excerpt = post.Excerpt?.Trim() ?? string.Empty,
                Content = post.Content?.Trim() ?? string.Empty,
                CoverImage = post.CoverImage?.Trim() ?? string.Empty,
                AuthorId = post.AuthorId,
                Status = NormalizeBlogStatus(post.Status),
                PublishedAt = post.PublishedAt,
                PrimaryCategoryId = categoriesOfPost.Select(item => (int?)item.CategoryId).FirstOrDefault(),
                SelectedCategoryIds = categoriesOfPost.Select(item => item.CategoryId).Distinct().ToList(),
                SelectedProductIds = productsOfPost.Select(item => item.ProductId).Distinct().ToList(),
                GalleryInput = BuildGalleryInput(galleryOfPost),
                SearchTerm = normalizedSearchTerm,
                ReturnStatus = normalizedStatusFilter,
                ReturnCategoryId = selectedCategoryId,
                ReturnPage = currentPage
            };
        }
        else
        {
            input = new AdminPostEditorInputModel
            {
                Status = "draft",
                SearchTerm = normalizedSearchTerm,
                ReturnStatus = normalizedStatusFilter,
                ReturnCategoryId = selectedCategoryId,
                ReturnPage = currentPage
            };
        }

        var previewGallery = input.Id.HasValue && galleryByPostId.TryGetValue(input.Id.Value, out var existingGallery)
            ? existingGallery.Select(item => new AdminPostGalleryItemViewModel
            {
                ImageUrl = item.ImageUrl?.Trim() ?? string.Empty,
                Caption = item.Caption?.Trim() ?? string.Empty,
                OrderIndex = item.OrderIndex
            }).ToList()
            : ParseGalleryInput(input.GalleryInput)
                .Select(item => new AdminPostGalleryItemViewModel
                {
                    ImageUrl = item.ImageUrl,
                    Caption = item.Caption,
                    OrderIndex = item.OrderIndex
                })
                .ToList();

        return new AdminPostEditorViewModel
        {
            IsEditing = input.Id.HasValue,
            FormTitle = input.Id.HasValue ? $"Chỉnh sửa bài viết #{input.Id.Value}" : "Tạo bài viết mới",
            SubmitLabel = input.Id.HasValue ? "Lưu cập nhật" : "Tạo bài viết",
            CancelUrl = cancelUrl,
            Input = input,
            AuthorOptions = authors
                .Select(author => new AdminSelectableItemViewModel
                {
                    Id = author.Id,
                    Label = !string.IsNullOrWhiteSpace(author.FullName) ? author.FullName!.Trim() : author.Username?.Trim() ?? $"Admin #{author.Id}",
                    Description = !string.IsNullOrWhiteSpace(author.Status) ? author.Status! : "active",
                    Selected = input.AuthorId == author.Id
                })
                .ToList(),
            CategoryOptions = categories
                .Select(category => new AdminSelectableItemViewModel
                {
                    Id = category.Id,
                    Label = category.Name?.Trim() ?? $"Chuyên mục #{category.Id}",
                    Description = category.Slug?.Trim() ?? string.Empty,
                    MetaValue = $"/assets/images/blogs/{BuildBlogCategoryFolderName(category)}",
                    Selected = input.SelectedCategoryIds.Contains(category.Id)
                })
                .ToList(),
            ProductOptions = products
                .Select(product => new AdminSelectableItemViewModel
                {
                    Id = product.Id,
                    Label = product.Name?.Trim() ?? $"Sản phẩm #{product.Id}",
                    Description = product.PriceLabel?.Trim() ?? product.Unit?.Trim() ?? string.Empty,
                    Selected = input.SelectedProductIds.Contains(product.Id)
                })
                .ToList(),
            GalleryItems = previewGallery
        };
    }

    private async Task ReplacePostCategoriesAsync(int postId, IReadOnlyCollection<int> selectedCategoryIds)
    {
        var existingMappings = await _context.BlogPostCategories
            .Where(mapping => mapping.PostId == postId)
            .ToListAsync();

        _context.BlogPostCategories.RemoveRange(existingMappings);

        if (selectedCategoryIds.Count == 0)
        {
            return;
        }

        var newMappings = selectedCategoryIds
            .Distinct()
            .Select(categoryId => new BlogPostCategory
            {
                PostId = postId,
                CategoryId = categoryId
            });

        await _context.BlogPostCategories.AddRangeAsync(newMappings);
    }

    private async Task ReplacePostProductsAsync(int postId, IReadOnlyCollection<int> selectedProductIds)
    {
        var existingMappings = await _context.BlogPostProducts
            .Where(mapping => mapping.PostId == postId)
            .ToListAsync();

        _context.BlogPostProducts.RemoveRange(existingMappings);

        if (selectedProductIds.Count == 0)
        {
            return;
        }

        var newMappings = selectedProductIds
            .Distinct()
            .Select(productId => new BlogPostProduct
            {
                PostId = postId,
                ProductId = productId
            });

        await _context.BlogPostProducts.AddRangeAsync(newMappings);
    }

    private async Task ReplacePostGalleryAsync(int postId, IReadOnlyCollection<AdminGalleryInputItem> galleryItems)
    {
        var existingImages = await _context.BlogImages
            .Where(image => image.BlogId == postId)
            .ToListAsync();

        _context.BlogImages.RemoveRange(existingImages);

        if (galleryItems.Count == 0)
        {
            return;
        }

        var newImages = galleryItems
            .Select(item => new BlogImage
            {
                BlogId = postId,
                ImageUrl = item.ImageUrl,
                Caption = string.IsNullOrWhiteSpace(item.Caption) ? null : item.Caption,
                OrderIndex = item.OrderIndex
            });

        await _context.BlogImages.AddRangeAsync(newImages);
    }

    private static async Task<List<string>> SaveUploadedPostImagesAsync(IReadOnlyList<IFormFile> imageFiles, string uniqueSlug, string categoryFolderName, ICollection<string> savedUploadedFilePaths)
    {
        if (imageFiles.Count == 0)
        {
            return [];
        }

        var normalizedCategoryFolderName = ResolveExistingBlogFolderName(categoryFolderName)
            ?? throw new InvalidOperationException("Không tìm thấy thư mục ảnh có sẵn trong wwwroot/assets/images/blogs cho chuyên mục đã chọn.");
        var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "blogs", normalizedCategoryFolderName);

        if (!Directory.Exists(uploadsDirectory))
        {
            throw new InvalidOperationException($"Thư mục ảnh blog '/assets/images/blogs/{normalizedCategoryFolderName}' không tồn tại.");
        }

        var fileBaseName = BuildBlogImageFileBaseName(uniqueSlug);
        var nextSequence = GetNextAvailableImageSequence(uploadsDirectory, fileBaseName);
        var uploadedImageUrls = new List<string>();

        foreach (var imageFile in imageFiles)
        {
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            var fileName = $"{fileBaseName}{nextSequence}{fileExtension}";
            var filePath = Path.Combine(uploadsDirectory, fileName);

            await using (var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await imageFile.CopyToAsync(stream);
            }

            uploadedImageUrls.Add($"/assets/images/blogs/{normalizedCategoryFolderName}/{fileName}");
            savedUploadedFilePaths.Add(filePath);
            nextSequence++;
        }

        return uploadedImageUrls;
    }

    private static int GetNextAvailableImageSequence(string uploadsDirectory, string fileBaseName)
    {
        var sequence = 1;

        while (Directory.EnumerateFiles(uploadsDirectory, $"{fileBaseName}{sequence}.*").Any())
        {
            sequence++;
        }

        return sequence;
    }

    private static string ResolveBlogImageCategoryFolder(IReadOnlyList<BlogCategory> selectedCategories, IReadOnlyList<int> selectedCategoryIds, int? primaryCategoryId)
    {
        if (selectedCategories.Count == 0 || selectedCategoryIds.Count == 0)
        {
            throw new InvalidOperationException("Bài viết chưa có chuyên mục để xác định thư mục ảnh trong /assets/images/blogs.");
        }

        if (primaryCategoryId.HasValue)
        {
            var primaryCategory = selectedCategories.FirstOrDefault(category => category.Id == primaryCategoryId.Value);

            if (primaryCategory is not null)
            {
                var primaryFolderName = ResolveExistingBlogCategoryFolderName(primaryCategory);
                if (!string.IsNullOrWhiteSpace(primaryFolderName))
                {
                    return primaryFolderName;
                }
            }
        }

        var categoryById = selectedCategories.ToDictionary(category => category.Id);

        foreach (var categoryId in selectedCategoryIds)
        {
            if (categoryById.TryGetValue(categoryId, out var category))
            {
                var folderName = ResolveExistingBlogCategoryFolderName(category);
                if (!string.IsNullOrWhiteSpace(folderName))
                {
                    return folderName;
                }
            }
        }

        throw new InvalidOperationException("Không tìm thấy thư mục ảnh blog có sẵn tương ứng với chuyên mục đã chọn trong wwwroot/assets/images/blogs.");
    }

    private static string BuildBlogCategoryFolderName(BlogCategory category)
    {
        var normalizedSlug = BuildSeoFriendlySlug(category.Slug);
        var normalizedName = BuildSeoFriendlySlug(category.Name);

        if (normalizedSlug is "dich-vu" || normalizedName is "dich-vu")
        {
            return "service";
        }

        if (normalizedSlug is "kinh-nghiem" || normalizedName is "kinh-nghiem")
        {
            return "experience";
        }

        if (normalizedSlug is "canh-bao" || normalizedName is "canh-bao")
        {
            return "warning";
        }

        if (normalizedSlug is "thi-truong" || normalizedName is "thi-truong" || normalizedSlug is "market" || normalizedName is "market")
        {
            return "Market";
        }

        if (normalizedSlug is "tin-tuc" || normalizedName is "tin-tuc")
        {
            return "tin-tuc";
        }

        var folderName = !string.IsNullOrWhiteSpace(normalizedSlug)
            ? normalizedSlug
            : normalizedName;

        return string.IsNullOrWhiteSpace(folderName)
            ? $"category-{category.Id}"
            : folderName;
    }

    private static string? ResolveExistingBlogCategoryFolderName(BlogCategory category)
    {
        return ResolveExistingBlogFolderName(BuildBlogCategoryFolderName(category));
    }

    private static string? ResolveExistingBlogFolderName(string? folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName))
        {
            return null;
        }

        var blogsRootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "blogs");

        if (!Directory.Exists(blogsRootDirectory))
        {
            return null;
        }

        return Directory.EnumerateDirectories(blogsRootDirectory)
            .Select(Path.GetFileName)
            .FirstOrDefault(existingFolderName =>
                !string.IsNullOrWhiteSpace(existingFolderName)
                && string.Equals(existingFolderName, folderName.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    private static string BuildBlogImageFileBaseName(string? uniqueSlug)
    {
        var baseName = BuildSeoFriendlySlug(uniqueSlug)
            .Replace("-", string.Empty)
            .Trim();

        if (string.IsNullOrWhiteSpace(baseName))
        {
            return "hinhanhblog";
        }

        return baseName.Length <= 48 ? baseName : baseName[..48];
    }

    private static string? NormalizeStoredImagePath(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim().Replace("\\", "/");

        if (normalized.StartsWith("~/", StringComparison.Ordinal))
        {
            return $"/{normalized[2..]}";
        }

        if (normalized.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || normalized.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return normalized;
        }

        return normalized.StartsWith("/", StringComparison.Ordinal)
            ? normalized
            : $"/{normalized.TrimStart('/')}";
    }

    private async Task<HashSet<string>> GetPostImageUrlsAsync(int postId)
    {
        var coverImage = await _context.BlogPosts
            .AsNoTracking()
            .Where(post => post.Id == postId)
            .Select(post => post.CoverImage)
            .FirstOrDefaultAsync();

        var galleryImages = await _context.BlogImages
            .AsNoTracking()
            .Where(image => image.BlogId == postId)
            .Select(image => image.ImageUrl)
            .ToListAsync();

        return galleryImages
            .Append(coverImage)
            .Select(NormalizeStoredImagePath)
            .Where(imageUrl => !string.IsNullOrWhiteSpace(imageUrl))
            .Cast<string>()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private async Task DeleteUnreferencedBlogImageFilesAsync(IEnumerable<string> imageUrls)
    {
        var normalizedImageUrls = imageUrls
            .Select(NormalizeStoredImagePath)
            .Where(imageUrl => !string.IsNullOrWhiteSpace(imageUrl))
            .Cast<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedImageUrls.Count == 0)
        {
            return;
        }

        var referencedCoverImages = await _context.BlogPosts
            .AsNoTracking()
            .Where(post => post.CoverImage != null && normalizedImageUrls.Contains(post.CoverImage))
            .Select(post => post.CoverImage!)
            .ToListAsync();

        var referencedGalleryImages = await _context.BlogImages
            .AsNoTracking()
            .Where(image => image.ImageUrl != null && normalizedImageUrls.Contains(image.ImageUrl))
            .Select(image => image.ImageUrl!)
            .ToListAsync();

        var referencedImageUrls = referencedCoverImages
            .Concat(referencedGalleryImages)
            .Select(NormalizeStoredImagePath)
            .Where(imageUrl => !string.IsNullOrWhiteSpace(imageUrl))
            .Cast<string>()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var imageUrl in normalizedImageUrls)
        {
            if (referencedImageUrls.Contains(imageUrl))
            {
                continue;
            }

            var filePath = MapBlogImageUrlToFilePath(imageUrl);
            if (string.IsNullOrWhiteSpace(filePath) || !System.IO.File.Exists(filePath))
            {
                continue;
            }

            System.IO.File.Delete(filePath);
        }
    }

    private static string? MapBlogImageUrlToFilePath(string? imageUrl)
    {
        var normalizedImageUrl = NormalizeStoredImagePath(imageUrl);
        if (string.IsNullOrWhiteSpace(normalizedImageUrl)
            || !normalizedImageUrl.StartsWith("/assets/images/blogs/", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var relativePath = normalizedImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
    }

    private IActionResult RenderPostsView(AdminPostsPageViewModel model)
    {
        ViewData["Title"] = "Bài viết";
        ViewData["AdminSection"] = "Posts";
        return View("Posts", model);
    }

    private IActionResult RedirectToPosts(string? searchTerm, string? status, int? categoryId, int? editId, int? page = null)
    {
        return RedirectToAction(nameof(Posts), new
        {
            searchTerm = searchTerm?.Trim(),
            status = string.IsNullOrWhiteSpace(status) ? null : NormalizeStatusFilter(status),
            categoryId = categoryId > 0 ? categoryId : null,
            editId = editId > 0 ? editId : null,
            page = page > 1 ? page : null
        });
    }

    private IActionResult RedirectToPostsSection(string? searchTerm, string? status, int? categoryId, int? editId, string section, int? page = null)
    {
        var url = Url.Action(nameof(Posts), "Admin", new
        {
            searchTerm = searchTerm?.Trim(),
            status = string.IsNullOrWhiteSpace(status) ? null : NormalizeStatusFilter(status),
            categoryId = categoryId > 0 ? categoryId : null,
            editId = editId > 0 ? editId : null,
            page = page > 1 ? page : null
        }) ?? "/admin/posts";

        var normalizedSection = string.IsNullOrWhiteSpace(section)
            ? string.Empty
            : section.Trim().TrimStart('#').Replace("-panel", string.Empty, StringComparison.OrdinalIgnoreCase);

        return string.IsNullOrWhiteSpace(normalizedSection)
            ? Redirect(url)
            : Redirect($"{url}#{normalizedSection}-panel");
    }

    private static List<AdminFilterOptionViewModel> BuildStatusFilters(int totalPosts, int publishedCount, int draftCount, int reviewCount, int hiddenCount, int missingCoverCount, string selectedStatus)
    {
        return new List<AdminFilterOptionViewModel>
        {
            new() { Value = string.Empty, Label = $"Tất cả ({totalPosts})", Selected = string.IsNullOrWhiteSpace(selectedStatus) },
            new() { Value = "published", Label = $"Published ({publishedCount})", Selected = selectedStatus == "published" },
            new() { Value = "draft", Label = $"Draft ({draftCount})", Selected = selectedStatus == "draft" },
            new() { Value = "review", Label = $"Chờ duyệt ({reviewCount})", Selected = selectedStatus == "review" },
            new() { Value = "hidden", Label = $"Đang ẩn ({hiddenCount})", Selected = selectedStatus == "hidden" },
            new() { Value = "missing-cover", Label = $"Thiếu cover ({missingCoverCount})", Selected = selectedStatus == "missing-cover" }
        };
    }

    private static List<AdminFilterOptionViewModel> BuildCategoryFilters(IEnumerable<BlogCategory> categories, int? selectedCategoryId)
    {
        var filters = new List<AdminFilterOptionViewModel>
        {
            new() { Value = string.Empty, Label = "Tất cả chuyên mục", Selected = !selectedCategoryId.HasValue }
        };

        filters.AddRange(categories.Select(category => new AdminFilterOptionViewModel
        {
            Value = category.Id.ToString(CultureInfo.InvariantCulture),
            Label = category.Name?.Trim() ?? $"Chuyên mục #{category.Id}",
            Selected = selectedCategoryId == category.Id
        }));

        return filters;
    }

    private static bool MatchesSearch(AdminPostRowViewModel post, string normalizedSearchTerm)
    {
        if (string.IsNullOrWhiteSpace(normalizedSearchTerm))
        {
            return true;
        }

        return ContainsInvariant(post.Title, normalizedSearchTerm)
            || ContainsInvariant(post.Slug, normalizedSearchTerm)
            || ContainsInvariant(post.Category, normalizedSearchTerm)
            || ContainsInvariant(post.Author, normalizedSearchTerm)
            || ContainsInvariant(post.Excerpt, normalizedSearchTerm)
            || ContainsInvariant(post.RelatedProductSummary, normalizedSearchTerm);
    }

    private static bool MatchesStatusFilter(AdminPostRowViewModel post, string selectedStatus)
    {
        if (string.IsNullOrWhiteSpace(selectedStatus))
        {
            return true;
        }

        return selectedStatus == "missing-cover"
            ? !post.HasCover
            : string.Equals(post.Status, selectedStatus, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ContainsInvariant(string source, string keyword)
    {
        return !string.IsNullOrWhiteSpace(source)
            && source.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<string> BuildUniqueBlogSlugAsync(string? rawSlug, string? title, int? excludingPostId)
    {
        var baseSlug = BuildSeoFriendlySlug(!string.IsNullOrWhiteSpace(rawSlug) ? rawSlug : title);

        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "bai-viet";
        }

        var existingSlugs = await _context.BlogPosts
            .AsNoTracking()
            .Where(post => !excludingPostId.HasValue || post.Id != excludingPostId.Value)
            .Select(post => post.Slug ?? string.Empty)
            .ToListAsync();

        if (!existingSlugs.Any(slug => string.Equals(slug, baseSlug, StringComparison.OrdinalIgnoreCase)))
        {
            return baseSlug;
        }

        for (var suffix = 2; suffix < 10000; suffix++)
        {
            var candidate = $"{baseSlug}-{suffix}";

            if (!existingSlugs.Any(slug => string.Equals(slug, candidate, StringComparison.OrdinalIgnoreCase)))
            {
                return candidate;
            }
        }

        return $"{baseSlug}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
    }

    private static string BuildSeoFriendlySlug(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().Replace('Đ', 'D').Replace('đ', 'd').Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        var previousWasHyphen = false;

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(character))
            {
                builder.Append(char.ToLowerInvariant(character));
                previousWasHyphen = false;
                continue;
            }

            if (previousWasHyphen || builder.Length == 0)
            {
                continue;
            }

            builder.Append('-');
            previousWasHyphen = true;
        }

        return builder.ToString().Trim('-');
    }

    private static DateTime? ResolvePublishedAt(DateTime? requestedPublishedAt, string normalizedStatus, DateTime? existingPublishedAt, DateTime now)
    {
        if (normalizedStatus == "published")
        {
            return requestedPublishedAt ?? existingPublishedAt ?? now;
        }

        return requestedPublishedAt ?? existingPublishedAt;
    }

    private static string? NormalizeNullableText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static List<AdminGalleryInputItem> ParseGalleryInput(string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return [];
        }

        var lines = rawValue
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        var items = new List<AdminGalleryInputItem>();

        for (var index = 0; index < lines.Count; index++)
        {
            var line = lines[index];
            var separatorIndex = line.IndexOf('|');
            var imageUrl = separatorIndex >= 0 ? line[..separatorIndex].Trim() : line;
            var caption = separatorIndex >= 0 ? line[(separatorIndex + 1)..].Trim() : string.Empty;

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                continue;
            }

            items.Add(new AdminGalleryInputItem
            {
                ImageUrl = imageUrl,
                Caption = caption,
                OrderIndex = index + 1
            });
        }

        return items;
    }

    private static List<AdminGalleryInputItem> ReplaceGalleryUploadTokens(IReadOnlyList<AdminGalleryInputItem> galleryItems, IReadOnlyList<string> uploadedGalleryUrls)
    {
        if (galleryItems.Count == 0)
        {
            return [];
        }

        var uploadedUrlQueue = new Queue<string>(uploadedGalleryUrls);
        var resolvedItems = new List<AdminGalleryInputItem>();

        foreach (var item in galleryItems)
        {
            var resolvedUrl = item.ImageUrl?.Trim() ?? string.Empty;

            if (IsGalleryUploadToken(resolvedUrl))
            {
                if (uploadedUrlQueue.Count == 0)
                {
                    throw new InvalidOperationException("Không đủ ảnh gallery được tải lên để khớp với dữ liệu caption.");
                }

                resolvedUrl = uploadedUrlQueue.Dequeue();
            }

            if (string.IsNullOrWhiteSpace(resolvedUrl))
            {
                continue;
            }

            resolvedItems.Add(new AdminGalleryInputItem
            {
                ImageUrl = resolvedUrl,
                Caption = item.Caption,
                OrderIndex = resolvedItems.Count + 1
            });
        }

        if (uploadedUrlQueue.Count > 0)
        {
            throw new InvalidOperationException("Có ảnh gallery tải lên chưa được gắn caption tương ứng trong dữ liệu form.");
        }

        return resolvedItems;
    }

    private static bool IsGalleryUploadToken(string? value)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Trim().StartsWith("__UPLOAD__", StringComparison.OrdinalIgnoreCase);
    }

    private static string SerializeGalleryInput(IEnumerable<AdminGalleryInputItem> galleryItems)
    {
        return string.Join(Environment.NewLine, galleryItems.Select(item =>
            string.IsNullOrWhiteSpace(item.Caption)
                ? item.ImageUrl?.Trim() ?? string.Empty
                : $"{item.ImageUrl?.Trim()} | {item.Caption?.Trim()}"));
    }

    private static string BuildGalleryInput(IEnumerable<AdminBlogImageRecord> galleryItems)
    {
        return string.Join(Environment.NewLine, galleryItems.Select(item =>
            string.IsNullOrWhiteSpace(item.Caption)
                ? item.ImageUrl?.Trim() ?? string.Empty
                : $"{item.ImageUrl?.Trim()} | {item.Caption?.Trim()}"));
    }

    private static string NormalizeStatusFilter(string? status)
    {
        var normalized = status?.Trim().ToLowerInvariant() ?? string.Empty;

        return normalized switch
        {
            "published" => "published",
            "review" => "review",
            "hidden" => "hidden",
            "draft" => "draft",
            "missing-cover" => "missing-cover",
            _ => string.Empty
        };
    }

    private static bool IsValidBlogStatus(string? status)
    {
        return ValidBlogStatuses.Contains(status?.Trim().ToLowerInvariant() ?? string.Empty);
    }

    private static string NormalizeBlogStatus(string? status)
    {
        return status?.Trim().ToLowerInvariant() switch
        {
            "published" => "published",
            "review" => "review",
            "hidden" => "hidden",
            _ => "draft"
        };
    }

    private static string BuildBlogStatusLabel(string status)
    {
        return status switch
        {
            "published" => "Đã xuất bản",
            "review" => "Chờ duyệt",
            "hidden" => "Đang ẩn",
            _ => "Bản nháp"
        };
    }

    private static string BuildBlogStatusClass(string status)
    {
        return status switch
        {
            "published" => "published",
            "review" => "review",
            "hidden" => "hidden",
            _ => "draft"
        };
    }

    private static string BuildPublishText(string status, DateTime? publishedAt, CultureInfo culture)
    {
        if (status == "published" && publishedAt.HasValue)
        {
            return publishedAt.Value.ToString("dd/MM/yyyy HH:mm", culture);
        }

        if (publishedAt.HasValue)
        {
            return $"Lịch: {publishedAt.Value.ToString("dd/MM/yyyy HH:mm", culture)}";
        }

        return "Chưa lên lịch";
    }

    private static string BuildPublishHint(string status, DateTime? publishedAt)
    {
        return status switch
        {
            "published" => "Đang hiển thị ngoài site",
            "review" => publishedAt.HasValue ? "Đã có lịch, đang chờ duyệt trước khi publish" : "Đang chờ editor/phê duyệt",
            "hidden" => "Tạm ẩn khỏi ngoài site",
            _ => publishedAt.HasValue ? "Đã lên lịch nhưng vẫn là bản nháp" : "Có thể lên lịch hoặc giữ nháp"
        };
    }

    private static string BuildBlogSlug(string? slug, int id)
    {
        return !string.IsNullOrWhiteSpace(slug)
            ? slug.Trim()
            : id.ToString(CultureInfo.InvariantCulture);
    }

    private static string BuildAdminExcerpt(string? excerpt, string? content)
    {
        var source = !string.IsNullOrWhiteSpace(excerpt)
            ? excerpt
            : content;

        if (string.IsNullOrWhiteSpace(source))
        {
            return "Bài viết chưa có mô tả ngắn.";
        }

        var plainText = Regex.Replace(source, "<.*?>", string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(plainText))
        {
            return "Bài viết chưa có mô tả ngắn.";
        }

        return plainText.Length > 160 ? $"{plainText[..157]}..." : plainText;
    }

    private static int CalculateSeoScore(AdminBlogPostRecord post, bool hasMedia, bool hasCategory, int relatedProducts, bool isPublished)
    {
        var score = 35;
        var title = post.Title?.Trim() ?? string.Empty;
        var slug = post.Slug?.Trim() ?? string.Empty;
        var excerpt = post.Excerpt?.Trim() ?? string.Empty;
        var plainContentLength = Regex.Replace(post.Content ?? string.Empty, "<.*?>", string.Empty).Trim().Length;

        if (!string.IsNullOrWhiteSpace(title))
        {
            score += 14;

            if (title.Length is >= 35 and <= 80)
            {
                score += 4;
            }
        }

        if (!string.IsNullOrWhiteSpace(slug))
        {
            score += 10;
        }

        if (!string.IsNullOrWhiteSpace(excerpt))
        {
            score += 12;
        }

        if (plainContentLength >= 300)
        {
            score += 10;
        }

        if (hasMedia)
        {
            score += 10;
        }

        if (hasCategory)
        {
            score += 6;
        }

        if (relatedProducts > 0)
        {
            score += 5;
        }

        if (isPublished)
        {
            score += 4;
        }

        return Math.Clamp(score, 0, 100);
    }

    private static string BuildSeoClass(int seoScore)
    {
        return seoScore switch
        {
            >= 85 => "high",
            >= 70 => "medium",
            _ => "low"
        };
    }

    private static (string PrimaryLabel, string PrimaryValue, string SecondaryLabel, string SecondaryValue) BuildStatusActions(string status)
    {
        return status switch
        {
            "draft" => ("Chờ duyệt", "review", "Xuất bản", "published"),
            "review" => ("Xuất bản", "published", "Trả nháp", "draft"),
            "hidden" => ("Xuất bản lại", "published", "Về nháp", "draft"),
            _ => ("Ẩn bài", "hidden", "Về nháp", "draft")
        };
    }

    private static string BuildProductSummary(IReadOnlyCollection<AdminBlogProductMapping> products)
    {
        if (products.Count == 0)
        {
            return "Chưa gắn sản phẩm";
        }

        var names = products
            .Select(product => product.ProductName?.Trim())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Cast<string>()
            .ToList();

        if (names.Count == 0)
        {
            return $"{products.Count} sản phẩm liên quan";
        }

        return names.Count <= 2
            ? string.Join(", ", names)
            : $"{string.Join(", ", names.Take(2))} +{names.Count - 2}";
    }

    private sealed class AdminBlogPostRecord
    {
        public int Id { get; init; }

        public string? Title { get; init; }

        public string? Slug { get; init; }

        public string? Excerpt { get; init; }

        public string? Content { get; init; }

        public string? CoverImage { get; init; }

        public int? AuthorId { get; init; }

        public string? AuthorName { get; init; }

        public string? Status { get; init; }

        public DateTime? PublishedAt { get; init; }

        public DateTime? CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }

        public int LikeCount { get; init; }
    }

    private sealed class AdminBlogCategoryMapping
    {
        public int PostId { get; init; }

        public int CategoryId { get; init; }

        public string? CategoryName { get; init; }

        public string? CategorySlug { get; init; }
    }

    private sealed class AdminBlogProductMapping
    {
        public int PostId { get; init; }

        public int ProductId { get; init; }

        public string? ProductName { get; init; }

        public string? ProductSlug { get; init; }
    }

    private sealed class AdminBlogImageRecord
    {
        public int PostId { get; init; }

        public string? ImageUrl { get; init; }

        public string? Caption { get; init; }

        public int OrderIndex { get; init; }
    }

    private sealed class AdminAuthorRecord
    {
        public int Id { get; init; }

        public string? FullName { get; init; }

        public string? Username { get; init; }

        public string? Status { get; init; }
    }

    private sealed class AdminGalleryInputItem
    {
        public string ImageUrl { get; init; } = string.Empty;

        public string Caption { get; init; } = string.Empty;

        public int OrderIndex { get; init; }
    }
}
