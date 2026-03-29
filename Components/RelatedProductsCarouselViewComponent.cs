using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.Components
{
    public class RelatedProductsCarouselViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public RelatedProductsCarouselViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 8, IEnumerable<int>? excludeIds = null)
        {
            var excludedIdSet = (excludeIds ?? Enumerable.Empty<int>())
                .Where(id => id > 0)
                .Distinct()
                .ToHashSet();

            var products = await _context.Products
                .AsNoTracking()
                .Include(product => product.Category)
                .Include(product => product.ProductImages)
                .Where(product => product.Status == "active" && !excludedIdSet.Contains(product.Id))
                .OrderByDescending(product => product.IsFeatured)
                .ThenByDescending(product => product.CreatedAt)
                .Take(count > 0 ? count : 8)
                .ToListAsync();

            return View(products);
        }
    }
}
