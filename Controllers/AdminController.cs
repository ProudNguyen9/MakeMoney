using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;
using WebThuMuaPheLieu.ViewModels;

namespace WebThuMuaPheLieu.Controllers;

[Authorize]
[Route("admin")]
public class AdminController : Controller
{
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const string ProductImageFolder = "assets/images/products";

    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AdminController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
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
                (product.PrimaryImage == null || product.PrimaryImage == "") && !product.ProductImages.Any()),
            BlogsWithoutImages = await _context.BlogPosts.CountAsync(post =>
                (post.CoverImage == null || post.CoverImage == "") && !post.BlogImages.Any())
        };

        ViewData["Title"] = "Tổng quan";
        ViewData["AdminSection"] = "Overview";
        return View(model);
    }

    [HttpGet("products")]
    public async Task<IActionResult> Products(
        int? id = null,
        int? categoryId = null,
        int? categoryEditId = null,
        bool create = false,
        bool manageCategories = false)
    {
        var model = await BuildAdminProductsViewModelAsync(id, categoryId, categoryEditId, create, manageCategories);
        ViewData["Title"] = "Sản phẩm";
        ViewData["AdminSection"] = "Products";
        return View(model);
    }

    [HttpPost("products/save")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(50_000_000)]
    public async Task<IActionResult> SaveProduct(AdminProductEditorViewModel editor)
    {
        if (string.IsNullOrWhiteSpace(editor.Name))
        {
            TempData["AdminProductsError"] = "Tên sản phẩm không được để trống.";
            return RedirectToAction(nameof(Products), new { id = editor.Id });
        }

        var normalizedStatus = NormalizeStatus(editor.Status);
        if (normalizedStatus is null)
        {
            TempData["AdminProductsError"] = "Trạng thái sản phẩm không hợp lệ.";
            return RedirectToAction(nameof(Products), new { id = editor.Id });
        }

        if (editor.CategoryId.HasValue)
        {
            var categoryExists = await _context.ProductCategories.AnyAsync(c => c.Id == editor.CategoryId.Value);
            if (!categoryExists)
            {
                TempData["AdminProductsError"] = "Danh mục đã chọn không tồn tại.";
                return RedirectToAction(nameof(Products), new { id = editor.Id });
            }
        }

        var now = DateTime.Now;
        var isCreate = !editor.Id.HasValue;

        Product product;
        decimal? oldPrice = null;
        string? oldUnit = null;

        if (isCreate)
        {
            product = new Product
            {
                CreatedAt = now
            };
            _context.Products.Add(product);
        }
        else
        {
            product = await _context.Products
                .Include(p => p.ProductImages.OrderBy(i => i.OrderIndex))
                .Include(p => p.PriceHistories.OrderByDescending(h => h.RecordedAt))
                .FirstOrDefaultAsync(p => p.Id == editor.Id.Value)
                ?? throw new InvalidOperationException($"Không tìm thấy sản phẩm ID {editor.Id.Value}.");

            oldPrice = product.PriceValue;
            oldUnit = product.Unit;
        }

        var cleanedName = editor.Name.Trim();
        product.Name = cleanedName;
        product.Slug = string.IsNullOrWhiteSpace(editor.Slug)
            ? GenerateSlug(cleanedName)
            : GenerateSlug(editor.Slug);
        product.ShortDescription = editor.ShortDescription?.Trim();
        product.Description = editor.Description?.Trim();
        product.CategoryId = editor.CategoryId;
        product.PriceValue = editor.PriceValue;
        product.Unit = editor.Unit?.Trim();
        product.PriceLabel = editor.PriceLabel?.Trim();
        product.Status = normalizedStatus;
        product.IsFeatured = editor.IsFeatured;
        product.UpdatedAt = now;

        await _context.SaveChangesAsync();

        var uploadedFiles = editor.UploadedImages?
            .Where(file => file is { Length: > 0 })
            .ToList() ?? [];

        if (uploadedFiles.Count > 0)
        {
            await SaveProductImagesAsync(product, uploadedFiles);
        }

        if (isCreate && product.PriceValue.HasValue)
        {
            await CreateInitialPriceHistoryAsync(product);
        }
        else if (!isCreate && oldPrice != product.PriceValue)
        {
            await CreatePreviousPriceHistoryAsync(product, oldPrice, oldUnit);
        }

        await _context.SaveChangesAsync();

        TempData["AdminProductsSuccess"] = isCreate
            ? "Đã thêm sản phẩm mới thành công."
            : "Đã cập nhật sản phẩm thành công.";

        return RedirectToAction(nameof(Products), new { id = product.Id });
    }

    [HttpPost("products/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.ProductImages)
            .Include(p => p.PriceHistories)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            TempData["AdminProductsError"] = "Không tìm thấy sản phẩm cần xóa.";
            return RedirectToAction(nameof(Products));
        }

        foreach (var image in product.ProductImages)
        {
            DeletePhysicalFile(image.ImageUrl);
        }

        DeletePhysicalFile(product.PrimaryImage);

        _context.ProductImages.RemoveRange(product.ProductImages);
        _context.PriceHistories.RemoveRange(product.PriceHistories);
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        TempData["AdminProductsSuccess"] = "Đã xóa sản phẩm thành công.";
        return RedirectToAction(nameof(Products));
    }

    [HttpPost("product-categories/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(AdminCategoryEditorViewModel editor)
    {
        if (string.IsNullOrWhiteSpace(editor.Name))
        {
            TempData["AdminProductsError"] = "Tên loại sản phẩm không được để trống.";
            return RedirectToAction(nameof(Products), new { manageCategories = true });
        }

        var category = await CreateCategoryIfNeededAsync(editor.Name, editor.Description);
        TempData["AdminProductsSuccess"] = $"Đã thêm loại sản phẩm \"{category.Name}\".";
        return RedirectToAction(nameof(Products), new { categoryId = category.Id, categoryEditId = category.Id, manageCategories = true });
    }

    [HttpPost("product-categories/save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCategory(AdminCategoryEditorViewModel editor)
    {
        if (string.IsNullOrWhiteSpace(editor.Name))
        {
            TempData["AdminProductsError"] = "Tên loại sản phẩm không được để trống.";
            return RedirectToAction(nameof(Products), new { manageCategories = true, categoryEditId = editor.Id });
        }

        var normalizedName = editor.Name.Trim();
        var duplicatedCategory = await _context.ProductCategories
            .FirstOrDefaultAsync(c => c.Id != editor.Id && c.Name != null && c.Name.ToLower() == normalizedName.ToLower());

        if (duplicatedCategory is not null)
        {
            TempData["AdminProductsError"] = "Tên loại sản phẩm đã tồn tại.";
            return RedirectToAction(nameof(Products), new { manageCategories = true, categoryEditId = editor.Id });
        }

        ProductCategory category;
        if (editor.Id.HasValue)
        {
            category = await _context.ProductCategories.FirstOrDefaultAsync(c => c.Id == editor.Id.Value)
                ?? throw new InvalidOperationException($"Không tìm thấy loại sản phẩm ID {editor.Id.Value}.");

            category.Name = normalizedName;
            category.Slug = GenerateSlug(normalizedName);
            category.Description = editor.Description?.Trim();
            category.UpdatedAt = DateTime.Now;

            TempData["AdminProductsSuccess"] = $"Đã cập nhật loại sản phẩm \"{category.Name}\".";
        }
        else
        {
            category = new ProductCategory
            {
                Name = normalizedName,
                Slug = GenerateSlug(normalizedName),
                Description = editor.Description?.Trim(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.ProductCategories.Add(category);
            TempData["AdminProductsSuccess"] = $"Đã thêm loại sản phẩm \"{category.Name}\".";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Products), new { manageCategories = true, categoryEditId = category.Id, categoryId = category.Id });
    }

    [HttpPost("product-categories/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.ProductCategories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            TempData["AdminProductsError"] = "Không tìm thấy loại sản phẩm cần xóa.";
            return RedirectToAction(nameof(Products), new { manageCategories = true });
        }

        if (category.Products.Any())
        {
            TempData["AdminProductsError"] = $"Không thể xóa loại sản phẩm \"{category.Name}\" vì vẫn còn sản phẩm đang dùng.";
            return RedirectToAction(nameof(Products), new { manageCategories = true, categoryEditId = id });
        }

        _context.ProductCategories.Remove(category);
        await _context.SaveChangesAsync();

        TempData["AdminProductsSuccess"] = $"Đã xóa loại sản phẩm \"{category.Name}\".";
        return RedirectToAction(nameof(Products), new { manageCategories = true });
    }

    [HttpGet("prices")]
    public async Task<IActionResult> Prices(int? productId = null, int? historyId = null, bool createHistory = false, bool editCurrent = false)
    {
        var model = await BuildAdminPricesViewModelAsync(productId, historyId, createHistory, editCurrent);
        ViewData["Title"] = "Bảng giá";
        ViewData["AdminSection"] = "Prices";
        return View(model);
    }

    [HttpPost("prices/current/save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCurrentPrice(AdminCurrentPriceEditorViewModel editor)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == editor.ProductId);

        if (product is null)
        {
            TempData["AdminPricesError"] = "Không tìm thấy sản phẩm cần cập nhật giá.";
            return RedirectToAction(nameof(Prices));
        }

        var oldPrice = product.PriceValue;
        var oldUnit = product.Unit;

        product.PriceValue = editor.PriceValue;
        product.Unit = editor.Unit?.Trim();
        product.PriceLabel = editor.PriceLabel?.Trim();
        product.UpdatedAt = DateTime.Now;

        if (oldPrice != product.PriceValue || !string.Equals(oldUnit, product.Unit, StringComparison.OrdinalIgnoreCase))
        {
            await CreatePreviousPriceHistoryAsync(product, oldPrice, oldUnit);
        }

        await _context.SaveChangesAsync();
        TempData["AdminPricesSuccess"] = $"Đã cập nhật giá hiện tại cho sản phẩm \"{product.Name}\".";
        return RedirectToAction(nameof(Prices), new { productId = product.Id });
    }

    [HttpPost("prices/history/save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePriceHistory(AdminPriceHistoryEditorViewModel editor)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == editor.ProductId);

        if (product is null)
        {
            TempData["AdminPricesError"] = "Sản phẩm áp dụng lịch sử giá không tồn tại.";
            return RedirectToAction(nameof(Prices));
        }

        if (!editor.PriceValue.HasValue)
        {
            TempData["AdminPricesError"] = "Giá lịch sử không được để trống.";
            return RedirectToAction(nameof(Prices), new { productId = editor.ProductId, historyId = editor.Id, createHistory = !editor.Id.HasValue });
        }

        PriceHistory history;
        var isCreate = !editor.Id.HasValue;

        if (isCreate)
        {
            history = new PriceHistory
            {
                RecordedAt = editor.RecordedAt ?? DateTime.Now
            };
            _context.PriceHistories.Add(history);
        }
        else
        {
            history = await _context.PriceHistories.FirstOrDefaultAsync(h => h.Id == editor.Id.Value)
                ?? throw new InvalidOperationException($"Không tìm thấy lịch sử giá ID {editor.Id.Value}.");
        }

        history.ProductId = editor.ProductId;
        history.PriceValue = editor.PriceValue;
        history.PriceUnit = editor.PriceUnit?.Trim();
        history.PriceType = editor.PriceType?.Trim();
        history.Note = editor.Note?.Trim();
        history.EffectiveDate = editor.EffectiveDate;
        history.RecordedAt = editor.RecordedAt ?? history.RecordedAt ?? DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["AdminPricesSuccess"] = isCreate
            ? "Đã thêm lịch sử giá mới."
            : "Đã cập nhật lịch sử giá thành công.";

        return RedirectToAction(nameof(Prices), new { productId = editor.ProductId, historyId = history.Id });
    }

    [HttpPost("prices/history/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePriceHistory(int id, int? productId = null)
    {
        var history = await _context.PriceHistories.FirstOrDefaultAsync(h => h.Id == id);
        if (history is null)
        {
            TempData["AdminPricesError"] = "Không tìm thấy lịch sử giá cần xóa.";
            return RedirectToAction(nameof(Prices), new { productId });
        }

        var redirectProductId = productId ?? history.ProductId;
        _context.PriceHistories.Remove(history);
        await _context.SaveChangesAsync();

        TempData["AdminPricesSuccess"] = "Đã xóa lịch sử giá thành công.";
        return RedirectToAction(nameof(Prices), new { productId = redirectProductId });
    }

    [HttpGet("posts")]
    public IActionResult Posts()
    {
        ViewData["Title"] = "Bài viết";
        ViewData["AdminSection"] = "Posts";
        return View();
    }

    [HttpGet("marketing")]
    public IActionResult Marketing()
    {
        ViewData["Title"] = "Banner & SEO";
        ViewData["AdminSection"] = "Marketing";
        return View();
    }

    private async Task<AdminProductsViewModel> BuildAdminProductsViewModelAsync(
        int? selectedProductId,
        int? preselectedCategoryId,
        int? categoryEditId,
        bool createMode,
        bool manageCategories)
    {
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt)
            .ThenBy(p => p.Name)
            .ToListAsync();

        Product? selectedProduct = null;
        if (selectedProductId.HasValue)
        {
            selectedProduct = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.ProductImages.OrderBy(i => i.OrderIndex))
                .Include(p => p.PriceHistories.OrderByDescending(h => h.RecordedAt))
                .FirstOrDefaultAsync(p => p.Id == selectedProductId.Value);
        }

        var categories = await _context.ProductCategories
            .AsNoTracking()
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();

        var editor = selectedProduct is null
            ? new AdminProductEditorViewModel
            {
                Status = "draft",
                CategoryId = preselectedCategoryId
            }
            : new AdminProductEditorViewModel
            {
                Id = selectedProduct.Id,
                Name = selectedProduct.Name,
                Slug = selectedProduct.Slug,
                ShortDescription = selectedProduct.ShortDescription,
                Description = selectedProduct.Description,
                CategoryId = selectedProduct.CategoryId,
                PriceValue = selectedProduct.PriceValue,
                Unit = selectedProduct.Unit,
                PriceLabel = selectedProduct.PriceLabel,
                PrimaryImage = NormalizeImagePath(selectedProduct.PrimaryImage),
                Status = NormalizeStatus(selectedProduct.Status) ?? "draft",
                IsFeatured = selectedProduct.IsFeatured ?? false,
                ExistingImages = BuildExistingImages(selectedProduct),
                PriceHistories = selectedProduct.PriceHistories
                    .OrderByDescending(h => h.RecordedAt ?? DateTime.MinValue)
                    .Select(h => new AdminPriceHistoryItemViewModel
                    {
                        Id = h.Id,
                        PriceValue = h.PriceValue,
                        PriceUnit = h.PriceUnit,
                        PriceType = h.PriceType,
                        Note = h.Note,
                        EffectiveDate = h.EffectiveDate,
                        RecordedAt = h.RecordedAt
                    })
                    .ToList()
            };

        var selectedCategory = categoryEditId.HasValue
            ? categories.FirstOrDefault(category => category.Id == categoryEditId.Value)
            : null;

        var categoryEditor = selectedCategory is null
            ? new AdminCategoryEditorViewModel()
            : new AdminCategoryEditorViewModel
            {
                Id = selectedCategory.Id,
                Name = selectedCategory.Name,
                Description = selectedCategory.Description
            };

        return new AdminProductsViewModel
        {
            Products = products.Select(product => new AdminProductListItemViewModel
            {
                Id = product.Id,
                Name = product.Name ?? string.Empty,
                Slug = product.Slug ?? string.Empty,
                CategoryName = product.Category?.Name,
                PriceValue = product.PriceValue,
                Unit = product.Unit,
                PriceLabel = product.PriceLabel,
                PrimaryImage = NormalizeImagePath(product.PrimaryImage),
                Status = NormalizeStatus(product.Status),
                IsFeatured = product.IsFeatured ?? false,
                UpdatedAt = product.UpdatedAt,
                ImageCount = CountImages(product)
            }).ToList(),
            Categories = categories.Select(category => new AdminCategoryListItemViewModel
            {
                Id = category.Id,
                Name = category.Name ?? string.Empty,
                Slug = category.Slug ?? string.Empty,
                Description = category.Description,
                ProductCount = category.Products.Count,
                UpdatedAt = category.UpdatedAt
            }).ToList(),
            Editor = editor,
            CategoryEditor = categoryEditor,
            CategoryOptions = await BuildCategoryOptionsAsync(editor.CategoryId),
            ShowEditor = createMode || selectedProduct is not null,
            ShowCategoryManager = manageCategories || categoryEditId.HasValue
        };
    }

    private async Task<AdminPricesViewModel> BuildAdminPricesViewModelAsync(int? selectedProductId, int? historyId, bool createHistory, bool editCurrent)
    {
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();

        var selectedProduct = selectedProductId.HasValue
            ? products.FirstOrDefault(p => p.Id == selectedProductId.Value)
            : products.FirstOrDefault();

        var historiesQuery = _context.PriceHistories
            .AsNoTracking()
            .Include(h => h.Product)
            .AsQueryable();

        if (selectedProduct is not null)
        {
            historiesQuery = historiesQuery.Where(h => h.ProductId == selectedProduct.Id);
        }

        var histories = await historiesQuery
            .OrderByDescending(h => h.EffectiveDate ?? DateOnly.MinValue)
            .ThenByDescending(h => h.RecordedAt ?? DateTime.MinValue)
            .ToListAsync();

        var selectedHistory = historyId.HasValue
            ? histories.FirstOrDefault(h => h.Id == historyId.Value)
            : null;

        var currentPriceEditor = selectedProduct is null
            ? new AdminCurrentPriceEditorViewModel()
            : new AdminCurrentPriceEditorViewModel
            {
                ProductId = selectedProduct.Id,
                ProductName = selectedProduct.Name ?? string.Empty,
                CategoryName = selectedProduct.Category?.Name ?? "Chưa phân loại",
                PriceValue = selectedProduct.PriceValue,
                Unit = selectedProduct.Unit,
                PriceLabel = selectedProduct.PriceLabel,
                Status = NormalizeStatus(selectedProduct.Status),
                UpdatedAt = selectedProduct.UpdatedAt
            };

        var historyEditor = createHistory || selectedHistory is null
            ? new AdminPriceHistoryEditorViewModel
            {
                ProductId = selectedProduct?.Id ?? 0,
                PriceUnit = selectedProduct?.Unit,
                EffectiveDate = DateOnly.FromDateTime(DateTime.Today),
                RecordedAt = DateTime.Now
            }
            : new AdminPriceHistoryEditorViewModel
            {
                Id = selectedHistory.Id,
                ProductId = selectedHistory.ProductId ?? selectedProduct?.Id ?? 0,
                PriceValue = selectedHistory.PriceValue,
                PriceUnit = selectedHistory.PriceUnit,
                PriceType = selectedHistory.PriceType,
                Note = selectedHistory.Note,
                EffectiveDate = selectedHistory.EffectiveDate,
                RecordedAt = selectedHistory.RecordedAt
            };

        return new AdminPricesViewModel
        {
            Products = products.Select(product => new AdminPriceProductItemViewModel
            {
                Id = product.Id,
                Name = product.Name ?? string.Empty,
                CategoryName = product.Category?.Name ?? "Chưa phân loại",
                PriceValue = product.PriceValue,
                Unit = product.Unit,
                PriceLabel = product.PriceLabel,
                Status = NormalizeStatus(product.Status),
                IsSelected = selectedProduct?.Id == product.Id,
                UpdatedAt = product.UpdatedAt
            }).ToList(),
            ProductOptions = products.Select(product => new SelectListItem
            {
                Value = product.Id.ToString(CultureInfo.InvariantCulture),
                Text = product.Name ?? $"Sản phẩm #{product.Id}",
                Selected = (selectedHistory?.ProductId ?? selectedProduct?.Id) == product.Id
            }).ToList(),
            CurrentPriceEditor = currentPriceEditor,
            ShowCurrentPriceEditor = editCurrent && selectedProduct is not null,
            PriceHistories = histories.Select(history => new AdminPriceHistoryListItemViewModel
            {
                Id = history.Id,
                ProductId = history.ProductId,
                ProductName = history.Product?.Name ?? $"Sản phẩm #{history.ProductId}",
                PriceValue = history.PriceValue,
                PriceUnit = history.PriceUnit,
                PriceType = history.PriceType,
                Note = history.Note,
                EffectiveDate = history.EffectiveDate,
                RecordedAt = history.RecordedAt,
                IsSelected = selectedHistory?.Id == history.Id
            }).ToList(),
            HistoryEditor = historyEditor,
            ShowHistoryEditor = createHistory || selectedHistory is not null
        };
    }

    private async Task<List<SelectListItem>> BuildCategoryOptionsAsync(int? selectedCategoryId)
    {
        var categories = await _context.ProductCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        var options = new List<SelectListItem>
        {
            new() { Value = string.Empty, Text = "-- Chọn danh mục --", Selected = !selectedCategoryId.HasValue }
        };

        options.AddRange(categories.Select(category => new SelectListItem
        {
            Value = category.Id.ToString(CultureInfo.InvariantCulture),
            Text = category.Name ?? $"Danh mục #{category.Id}",
            Selected = selectedCategoryId == category.Id
        }));

        return options;
    }

    private async Task<ProductCategory> CreateCategoryIfNeededAsync(string name, string? description)
    {
        var normalizedName = name.Trim();

        var existingCategory = await _context.ProductCategories
            .FirstOrDefaultAsync(c => c.Name != null && c.Name.ToLower() == normalizedName.ToLower());

        if (existingCategory is not null)
        {
            if (!string.IsNullOrWhiteSpace(description) && string.IsNullOrWhiteSpace(existingCategory.Description))
            {
                existingCategory.Description = description.Trim();
                existingCategory.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return existingCategory;
        }

        var category = new ProductCategory
        {
            Name = normalizedName,
            Slug = GenerateSlug(normalizedName),
            Description = description?.Trim(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.ProductCategories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    private async Task SaveProductImagesAsync(Product product, List<IFormFile> uploadedFiles)
    {
        var webRootPath = _environment.WebRootPath;
        var targetFolderPath = Path.Combine(webRootPath, ProductImageFolder.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(targetFolderPath);

        var currentImages = await _context.ProductImages
            .Where(image => image.ProductId == product.Id)
            .OrderBy(image => image.OrderIndex)
            .ToListAsync();

        var baseName = BuildSafeFileName(product.Name, $"product{product.Id}");
        var nextIndex = GetNextImageIndex(product.PrimaryImage, currentImages, baseName);
        var nextOrderIndex = currentImages.Count == 0 ? 1 : currentImages.Max(image => image.OrderIndex ?? 0) + 1;

        foreach (var file in uploadedFiles)
        {
            var extension = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(extension) || !AllowedImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var safeExtension = extension.ToLowerInvariant();
            var fileName = $"{baseName}{nextIndex}{safeExtension}";
            while (System.IO.File.Exists(Path.Combine(targetFolderPath, fileName)))
            {
                nextIndex++;
                fileName = $"{baseName}{nextIndex}{safeExtension}";
            }

            var physicalPath = Path.Combine(targetFolderPath, fileName);
            await using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"~/{ProductImageFolder}/{fileName}";

            if (string.IsNullOrWhiteSpace(product.PrimaryImage))
            {
                product.PrimaryImage = relativePath;
            }
            else
            {
                _context.ProductImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = relativePath,
                    Caption = product.Name,
                    OrderIndex = nextOrderIndex
                });
                nextOrderIndex++;
            }

            nextIndex++;
        }

        product.UpdatedAt = DateTime.Now;
    }

    private Task CreateInitialPriceHistoryAsync(Product product)
    {
        if (!product.PriceValue.HasValue)
        {
            return Task.CompletedTask;
        }

        _context.PriceHistories.Add(new PriceHistory
        {
            ProductId = product.Id,
            PriceValue = product.PriceValue,
            PriceUnit = product.Unit,
            PriceType = "initial",
            Note = "Giá khởi tạo sản phẩm",
            EffectiveDate = DateOnly.FromDateTime(DateTime.Today),
            RecordedAt = DateTime.Now
        });

        return Task.CompletedTask;
    }

    private Task CreatePreviousPriceHistoryAsync(Product product, decimal? oldPrice, string? oldUnit)
    {
        if (!oldPrice.HasValue)
        {
            return Task.CompletedTask;
        }

        _context.PriceHistories.Add(new PriceHistory
        {
            ProductId = product.Id,
            PriceValue = oldPrice,
            PriceUnit = oldUnit,
            PriceType = "previous",
            Note = "Giá trước khi cập nhật",
            EffectiveDate = DateOnly.FromDateTime(DateTime.Today),
            RecordedAt = DateTime.Now
        });

        return Task.CompletedTask;
    }

    private static List<AdminProductImageItemViewModel> BuildExistingImages(Product product)
    {
        var images = new List<AdminProductImageItemViewModel>();

        if (!string.IsNullOrWhiteSpace(product.PrimaryImage))
        {
            images.Add(new AdminProductImageItemViewModel
            {
                ImageUrl = NormalizeImagePath(product.PrimaryImage) ?? string.Empty,
                Caption = product.Name,
                OrderIndex = 0,
                IsPrimary = true
            });
        }

        images.AddRange(product.ProductImages
            .OrderBy(image => image.OrderIndex ?? int.MaxValue)
            .ThenBy(image => image.Id)
            .Select(image => new AdminProductImageItemViewModel
            {
                Id = image.Id,
                ImageUrl = NormalizeImagePath(image.ImageUrl) ?? string.Empty,
                Caption = image.Caption,
                OrderIndex = image.OrderIndex ?? 0,
                IsPrimary = false
            }));

        return images;
    }

    private static int CountImages(Product product)
    {
        var count = product.ProductImages.Count;
        if (!string.IsNullOrWhiteSpace(product.PrimaryImage))
        {
            count++;
        }

        return count;
    }

    private static string? NormalizeImagePath(string? imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            return null;
        }

        var normalized = imagePath.Replace('\\', '/').Trim();
        if (normalized.StartsWith("~/", StringComparison.Ordinal))
        {
            return normalized;
        }

        if (normalized.StartsWith("/", StringComparison.Ordinal))
        {
            return $"~{normalized}";
        }

        return $"~/{normalized.TrimStart('~', '/')}";
    }

    private void DeletePhysicalFile(string? imagePath)
    {
        var normalized = NormalizeImagePath(imagePath);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return;
        }

        var relativePath = normalized[2..].Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(_environment.WebRootPath, relativePath);
        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }
    }

    private static int GetNextImageIndex(string? primaryImage, IEnumerable<ProductImage> images, string baseName)
    {
        var maxIndex = 0;
        var allPaths = new List<string?> { primaryImage };
        allPaths.AddRange(images.Select(image => image.ImageUrl));

        foreach (var path in allPaths.Where(path => !string.IsNullOrWhiteSpace(path)))
        {
            var fileName = Path.GetFileName(path!.Replace('\\', '/'));
            if (!fileName.StartsWith(baseName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var numberPart = new string(fileName
                .Skip(baseName.Length)
                .TakeWhile(char.IsDigit)
                .ToArray());

            if (int.TryParse(numberPart, out var parsedIndex) && parsedIndex > maxIndex)
            {
                maxIndex = parsedIndex;
            }
        }

        return maxIndex + 1;
    }

    private static string BuildSafeFileName(string? value, string fallback)
    {
        var source = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim().ToLowerInvariant();
        var normalized = source.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var character in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
            if (unicodeCategory == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
        }

        var result = builder
            .ToString()
            .Replace('đ', 'd')
            .Replace('Đ', 'D');

        return string.IsNullOrWhiteSpace(result) ? fallback.ToLowerInvariant() : result;
    }

    private static string GenerateSlug(string? value)
    {
        var source = string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString("N") : value.Trim().ToLowerInvariant();
        var normalized = source.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        var pendingDash = false;

        foreach (var character in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
            if (unicodeCategory == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            var current = character switch
            {
                'đ' => 'd',
                'Đ' => 'd',
                _ => char.ToLowerInvariant(character)
            };

            if (char.IsLetterOrDigit(current))
            {
                if (pendingDash && builder.Length > 0)
                {
                    builder.Append('-');
                }

                builder.Append(current);
                pendingDash = false;
            }
            else
            {
                pendingDash = builder.Length > 0;
            }
        }

        return builder.Length == 0 ? Guid.NewGuid().ToString("N") : builder.ToString();
    }

    private static string? NormalizeStatus(string? value)
    {
        return value?.Trim().ToLowerInvariant() switch
        {
            null or "" => "draft",
            "draft" => "draft",
            "active" => "active",
            "inactive" => "inactive",
            _ => null
        };
    }
}
