using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;
using WebThuMuaPheLieu.ViewModels;

namespace WebThuMuaPheLieu.Components;

public class FeaturedProductsSectionViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public FeaturedProductsSectionViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync(int? count = null, string viewName = "Pricing")
    {
        var normalizedViewName = string.IsNullOrWhiteSpace(viewName)
            ? "Pricing"
            : viewName.Trim();

        var query = _context.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Include(product => product.ProductImages)
            .Where(product => product.Status == "active" && product.IsFeatured == true)
            .OrderByDescending(product => product.UpdatedAt)
            .ThenByDescending(product => product.CreatedAt)
            .AsQueryable();

        if (count.HasValue && count.Value > 0)
        {
            query = query.Take(count.Value);
        }

        var products = await query.ToListAsync();

        var viewModel = new FeaturedProductsSectionViewModel
        {
            Count = count,
            ViewName = normalizedViewName,
            Products = products
        };

        return View(normalizedViewName, viewModel);
    }
}
