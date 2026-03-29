using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Where(p => p.Status == "active")
            .OrderByDescending(p => p.IsFeatured)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();

        var recentBlogPosts = await _context.BlogPosts
            .Where(post => post.Status == "published"
                && !_context.BlogPostCategories.Any(category => category.PostId == post.Id && category.CategoryId == 1))
            .OrderByDescending(post => post.PublishedAt ?? post.CreatedAt)
            .ThenByDescending(post => post.Id)
            .Take(8)
            .ToListAsync();

        var serviceBlogPosts = await _context.BlogPosts
            .Where(post => post.Status == "published"
                && _context.BlogPostCategories.Any(category => category.PostId == post.Id && category.CategoryId == 1))
            .OrderByDescending(post => post.PublishedAt ?? post.CreatedAt)
            .ThenByDescending(post => post.Id)
            .Take(8)
            .ToListAsync();

        ViewBag.RecentBlogPosts = recentBlogPosts;
        ViewBag.ServiceBlogPosts = serviceBlogPosts;

        return View(products);
    }

    // Action lấy tất cả sản phẩm
    public async Task<IActionResult> Products()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Where(p => p.Status == "active")
            .OrderByDescending(p => p.IsFeatured)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(products);
    }

    // Action lấy tất cả sản phẩm nổi bật
    public async Task<IActionResult> FeaturedProducts()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Where(p => p.Status == "active" && p.IsFeatured == true)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(products);
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

