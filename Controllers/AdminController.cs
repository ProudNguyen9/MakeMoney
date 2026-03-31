using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.Controllers;

[Route("admin")]
public class AdminController : Controller
{
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
    public IActionResult Marketing()
    {
        ViewData["Title"] = "Banner & SEO";
        ViewData["AdminSection"] = "Marketing";
        return View();
    }
}
