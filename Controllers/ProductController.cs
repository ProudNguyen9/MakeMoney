using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.Controllers;

public class ProductController : Controller
{
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public ProductController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var viewModel = await BuildProductIndexViewModelAsync(page);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetPagedProducts(int page = 1)
    {
        var viewModel = await BuildProductIndexViewModelAsync(page);

        return Json(new
        {
            currentPage = viewModel.CurrentPage,
            totalPages = viewModel.TotalPages,
            pageSize = viewModel.PageSize,
            totalItems = viewModel.TotalItems,
            products = viewModel.Products.Select(product => new
            {
                id = product.Id,
                name = product.Name,
                categoryName = product.CategoryName,
                shortDescription = product.ShortDescription,
                description = product.Description,
                priceLabel = product.PriceLabel,
                unit = product.Unit,
                primaryImage = product.PrimaryImage,
                images = product.Images
            })
        });
    }

    private async Task<ProductIndexViewModel> BuildProductIndexViewModelAsync(int page)
    {
        const int pageSize = 8;

        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.Status == "active")
            .OrderByDescending(p => p.IsFeatured)
            .ThenBy(p => p.Name)
            .ToListAsync();

        var productCards = products
            .Select(product =>
            {
                var productImages = BuildProductImageList(product.PrimaryImage);

                return new ProductCardViewModel
                {
                    Id = product.Id,
                    Name = product.Name ?? string.Empty,
                    CategoryName = product.Category?.Name,
                    ShortDescription = product.ShortDescription,
                    Description = product.Description,
                    PriceLabel = product.PriceLabel,
                    Unit = product.Unit,
                    PrimaryImage = productImages.FirstOrDefault() ?? Url.Content("~/assets/images/no-image.png"),
                    Images = productImages
                };
            })
            .ToList();

        var totalItems = productCards.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        var currentPage = Math.Min(Math.Max(page, 1), totalPages);

        var pagedProducts = productCards
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new ProductIndexViewModel
        {
            Products = pagedProducts,
            CurrentPage = currentPage,
            TotalPages = totalPages,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    private List<string> BuildProductImageList(string? primaryImage)
    {
        if (string.IsNullOrWhiteSpace(primaryImage))
        {
            return [Url.Content("~/assets/images/no-image.png")];
        }

        var normalizedPrimaryImage = primaryImage.Replace('\\', '/').Trim();

        if (normalizedPrimaryImage.Contains("/assets/img/", StringComparison.OrdinalIgnoreCase))
        {
            normalizedPrimaryImage = Regex.Replace(
                normalizedPrimaryImage,
                @"/assets/img/",
                "/assets/images/",
                RegexOptions.IgnoreCase);
        }
        if (!normalizedPrimaryImage.StartsWith("~/", StringComparison.Ordinal))
        {
            normalizedPrimaryImage = normalizedPrimaryImage.StartsWith("/", StringComparison.Ordinal)
                ? $"~{normalizedPrimaryImage}"
                : $"~/{normalizedPrimaryImage.TrimStart('~', '/')}";
        }

        var images = new List<string>();
        var currentImage = normalizedPrimaryImage;

        while (true)
        {
            var relativePath = currentImage[2..].Replace('/', Path.DirectorySeparatorChar);
            var physicalPath = Path.Combine(_environment.WebRootPath, relativePath);

            if (!System.IO.File.Exists(physicalPath))
            {
                break;
            }

            images.Add(Url.Content(currentImage));

            var nextImage = Regex.Replace(currentImage, @"(\d+)(\.[^.]+)$", match =>
                $"{int.Parse(match.Groups[1].Value) + 1}{match.Groups[2].Value}");

            if (string.Equals(nextImage, currentImage, StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            currentImage = nextImage;
        }

        return images.Count > 0
            ? images
            : [Url.Content(normalizedPrimaryImage)];
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
