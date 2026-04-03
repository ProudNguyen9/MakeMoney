using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;
using WebThuMuaPheLieu.ViewModels;

namespace WebThuMuaPheLieu.Controllers;

[Route("admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    private const string BannerActiveFolderRelative = "assets/images/bannersandseos";
    private const string BannerTitleSeparator = "|||";
    private const string MarketingSettingGroup = "marketing";

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
    public IActionResult Posts()
    {
        ViewData["Title"] = "Bài viết";
        ViewData["AdminSection"] = "Posts";
        return View();
    }

    [HttpGet("marketing")]
    public async Task<IActionResult> Marketing()
    {
        var model = await BuildAdminMarketingViewModelAsync();

        ViewData["Title"] = "Banner & SEO";
        ViewData["AdminSection"] = "Marketing";
        return View(model);
    }

    [HttpPost("marketing/upload-active-image")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> UploadMarketingActiveImage(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "Chưa có file ảnh được chọn." });
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(new { message = "Chỉ hỗ trợ file jpg, jpeg, png, webp hoặc gif." });
        }

        var folderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "bannersandseos");
        Directory.CreateDirectory(folderPath);

        var fileName = $"active-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(folderPath, fileName);

        await using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        var relativeUrl = $"~/{BannerActiveFolderRelative}/{fileName}";
        return Json(new
        {
            success = true,
            url = relativeUrl,
            fileName
        });
    }

    [HttpPost("marketing/save-banner")]
    public async Task<IActionResult> SaveBanner([FromBody] AdminMarketingViewModel model)
    {
        var banner = await _context.Banners
            .Include(item => item.BannerImages)
            .OrderBy(item => item.OrderIndex ?? int.MaxValue)
            .FirstOrDefaultAsync();

        if (banner is null)
        {
            banner = new Banner
            {
                CreatedAt = DateTime.Now,
                Status = "active",
                OrderIndex = 1
            };
            _context.Banners.Add(banner);
        }

        banner.Title = string.Join(BannerTitleSeparator, new[]
        {
            model.BannerLine1 ?? string.Empty,
            model.BannerLine2 ?? string.Empty,
            model.BannerLine3 ?? string.Empty,
            model.BannerLine4 ?? string.Empty
        });
        banner.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        var activeFolderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "bannersandseos");
        Directory.CreateDirectory(activeFolderPath);

        var imageUrls = new[] { model.BannerImage1, model.BannerImage2, model.BannerImage3 };
        var captions = new[] { "Ảnh banner chính", "Ảnh banner 2", "Ảnh banner 3" };

        for (var i = 0; i < imageUrls.Length; i++)
        {
            var finalUrl = await NormalizeBannerActiveImageAsync(imageUrls[i], i + 1);
            imageUrls[i] = finalUrl;

            var bannerImage = banner.BannerImages.FirstOrDefault(item => (item.OrderIndex ?? 0) == i + 1);
            if (bannerImage is null)
            {
                bannerImage = new BannerImage
                {
                    BannerId = banner.Id,
                    OrderIndex = i + 1
                };
                _context.BannerImages.Add(bannerImage);
            }

            bannerImage.ImageUrl = finalUrl;
            bannerImage.Caption = captions[i];
        }

        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            bannerId = banner.Id,
            bannerLine1 = model.BannerLine1,
            bannerLine2 = model.BannerLine2,
            bannerLine3 = model.BannerLine3,
            bannerLine4 = model.BannerLine4,
            bannerImage1 = imageUrls[0],
            bannerImage2 = imageUrls[1],
            bannerImage3 = imageUrls[2]
        });
    }

    [HttpGet("marketing/banner-state")]
    public async Task<IActionResult> GetBannerState()
    {
        var model = await BuildAdminMarketingViewModelAsync();
        return Json(new
        {
            success = true,
            bannerId = model.BannerId,
            bannerLine1 = model.BannerLine1,
            bannerLine2 = model.BannerLine2,
            bannerLine3 = model.BannerLine3,
            bannerLine4 = model.BannerLine4,
            bannerImage1 = model.BannerImage1,
            bannerImage2 = model.BannerImage2,
            bannerImage3 = model.BannerImage3
        });
    }

    [HttpPost("marketing/save-contact")]
    public async Task<IActionResult> SaveContact([FromBody] AdminMarketingViewModel model)
    {
        await UpsertSiteSettingAsync("contact.phone", model.Phone, "Số điện thoại liên hệ");
        await UpsertSiteSettingAsync("contact.zalo", model.Zalo, "Link Zalo");
        await UpsertSiteSettingAsync("contact.messenger", model.Messenger, "Link Messenger");
        await UpsertSiteSettingAsync("contact.facebook", model.Facebook, "Link Facebook");
        await UpsertSiteSettingAsync("contact.email", model.Email, "Email liên hệ");
        await UpsertSiteSettingAsync("contact.address", model.Address, "Địa chỉ hiển thị");
        await UpsertSiteSettingAsync("contact.purchase_areas", model.PurchaseAreas, "Khu vực thu mua");

        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            phone = model.Phone,
            zalo = model.Zalo,
            messenger = model.Messenger,
            facebook = model.Facebook,
            email = model.Email,
            address = model.Address,
            purchaseAreas = model.PurchaseAreas
        });
    }

    [HttpGet("marketing/contact-state")]
    public async Task<IActionResult> GetContactState()
    {
        var model = await BuildAdminMarketingViewModelAsync();
        return Json(new
        {
            success = true,
            phone = model.Phone,
            zalo = model.Zalo,
            messenger = model.Messenger,
            facebook = model.Facebook,
            email = model.Email,
            address = model.Address,
            purchaseAreas = model.PurchaseAreas
        });
    }

    [HttpPost("marketing/save-seo")]
    public async Task<IActionResult> SaveSeo([FromBody] AdminMarketingViewModel model)
    {
        await UpsertSiteSettingAsync("seo.meta_title", model.MetaTitle, "Meta title trang chủ");
        await UpsertSiteSettingAsync("seo.meta_description", model.MetaDescription, "Meta description trang chủ");
        await UpsertSiteSettingAsync("seo.keywords", model.SeoKeywords, "Từ khóa SEO trang chủ");
        await UpsertSiteSettingAsync("seo.og_title", model.OgTitle, "OG title trang chủ");
        await UpsertSiteSettingAsync("seo.og_image", model.OgImage, "OG image trang chủ");

        await _context.SaveChangesAsync();

        return Json(new
        {
            success = true,
            metaTitle = model.MetaTitle,
            metaDescription = model.MetaDescription,
            seoKeywords = model.SeoKeywords,
            ogTitle = model.OgTitle,
            ogImage = model.OgImage
        });
    }

    [HttpGet("marketing/seo-state")]
    public async Task<IActionResult> GetSeoState()
    {
        var model = await BuildAdminMarketingViewModelAsync();
        return Json(new
        {
            success = true,
            metaTitle = model.MetaTitle,
            metaDescription = model.MetaDescription,
            seoKeywords = model.SeoKeywords,
            ogTitle = model.OgTitle,
            ogImage = model.OgImage
        });
    }

    private async Task<AdminMarketingViewModel> BuildAdminMarketingViewModelAsync()
    {
        var model = new AdminMarketingViewModel();

        var banner = await _context.Banners
            .Include(item => item.BannerImages)
            .OrderBy(item => item.OrderIndex ?? int.MaxValue)
            .FirstOrDefaultAsync();

        if (banner is null)
        {
            return model;
        }

        model.BannerId = banner.Id;

        var titleLines = (banner.Title ?? string.Empty).Contains(BannerTitleSeparator, StringComparison.Ordinal)
            ? (banner.Title ?? string.Empty).Split(BannerTitleSeparator)
            : (banner.Title ?? string.Empty).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        if (titleLines.Length > 0) model.BannerLine1 = titleLines[0];
        if (titleLines.Length > 1) model.BannerLine2 = titleLines[1];
        if (titleLines.Length > 2) model.BannerLine3 = titleLines[2];
        if (titleLines.Length > 3) model.BannerLine4 = titleLines[3];

        var orderedImages = banner.BannerImages
            .OrderBy(item => item.OrderIndex ?? int.MaxValue)
            .ToList();

        if (orderedImages.Count > 0 && !string.IsNullOrWhiteSpace(orderedImages[0].ImageUrl)) model.BannerImage1 = orderedImages[0].ImageUrl!;
        if (orderedImages.Count > 1 && !string.IsNullOrWhiteSpace(orderedImages[1].ImageUrl)) model.BannerImage2 = orderedImages[1].ImageUrl!;
        if (orderedImages.Count > 2 && !string.IsNullOrWhiteSpace(orderedImages[2].ImageUrl)) model.BannerImage3 = orderedImages[2].ImageUrl!;

        var settings = await _context.SiteSettings
            .Where(item => item.SettingGroup == MarketingSettingGroup)
            .ToListAsync();

        string? GetSetting(string key)
            => settings.FirstOrDefault(item => item.SettingKey == key)?.SettingValue;

        model.Phone = GetSetting("contact.phone") ?? model.Phone;
        model.Zalo = GetSetting("contact.zalo") ?? model.Zalo;
        model.Messenger = GetSetting("contact.messenger") ?? model.Messenger;
        model.Facebook = GetSetting("contact.facebook") ?? model.Facebook;
        model.Email = GetSetting("contact.email") ?? model.Email;
        model.Address = GetSetting("contact.address") ?? model.Address;
        model.PurchaseAreas = GetSetting("contact.purchase_areas") ?? model.PurchaseAreas;

        model.MetaTitle = GetSetting("seo.meta_title") ?? model.MetaTitle;
        model.MetaDescription = GetSetting("seo.meta_description") ?? model.MetaDescription;
        model.SeoKeywords = GetSetting("seo.keywords") ?? model.SeoKeywords;
        model.OgTitle = GetSetting("seo.og_title") ?? model.OgTitle;
        model.OgImage = GetSetting("seo.og_image") ?? model.OgImage;

        return model;
    }

    private async Task UpsertSiteSettingAsync(string key, string? value, string description)
    {
        var setting = await _context.SiteSettings.FirstOrDefaultAsync(item => item.SettingKey == key);
        if (setting is null)
        {
            setting = new SiteSetting
            {
                SettingKey = key,
                SettingGroup = MarketingSettingGroup,
                Description = description
            };

            _context.SiteSettings.Add(setting);
        }

        setting.SettingValue = value;
        setting.SettingGroup = MarketingSettingGroup;
        setting.Description = description;
        setting.UpdatedAt = DateTime.Now;
    }

    private async Task<string> NormalizeBannerActiveImageAsync(string? imageUrl, int orderIndex)
    {
        if (string.IsNullOrWhiteSpace(imageUrl) || !imageUrl.Contains(BannerActiveFolderRelative, StringComparison.OrdinalIgnoreCase))
        {
            return imageUrl ?? string.Empty;
        }

        var extension = Path.GetExtension(imageUrl);
        var activeFileName = $"banner-{orderIndex}{extension}";
        var sourcePath = Path.Combine(_environment.WebRootPath, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        var activeFolderPath = Path.Combine(_environment.WebRootPath, "assets", "images", "bannersandseos");
        Directory.CreateDirectory(activeFolderPath);

        var destinationPath = Path.Combine(activeFolderPath, activeFileName);

        if (System.IO.File.Exists(sourcePath))
        {
            if (System.IO.File.Exists(destinationPath))
            {
                System.IO.File.Delete(destinationPath);
            }

            await using var sourceStream = System.IO.File.OpenRead(sourcePath);
            await using var destinationStream = System.IO.File.Create(destinationPath);
            await sourceStream.CopyToAsync(destinationStream);
        }

        return $"~/{BannerActiveFolderRelative}/{activeFileName}";
    }
}
