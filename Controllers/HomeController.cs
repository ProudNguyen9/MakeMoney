using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WebThuMuaPheLieu.Models;
using WebThuMuaPheLieu.ViewModels;

namespace WebThuMuaPheLieu.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(string? slug, int? id)
        {
            if (string.IsNullOrWhiteSpace(slug) && id == null)
            {
                return NotFound();
            }

            var query = _context.Products.AsQueryable();

            Product? product = null;

            // Ưu tiên slug
            if (!string.IsNullOrWhiteSpace(slug))
            {
                product = query.FirstOrDefault(p => p.Slug == slug && p.Status == "active");
            }

            // Nếu không tìm thấy bằng slug thì dùng id
            if (product == null && id != null)
            {
                product = query.FirstOrDefault(p => p.Id == id && p.Status == "active");
            }

            if (product == null)
            {
                return NotFound();
            }

            // Nếu truy cập bằng id hoặc slug sai thì redirect về URL chuẩn
            if (string.IsNullOrWhiteSpace(slug) || slug != product.Slug)
            {
                return RedirectToAction("Detail", new { slug = product.Slug, id = product.Id });
            }

            var category = _context.ProductCategories
                .FirstOrDefault(c => c.Id == product.CategoryId);

            var images = _context.ProductImages
                .Where(x => x.ProductId == product.Id)
                .OrderBy(x => x.OrderIndex)
                .ToList();

            var priceHistories = _context.PriceHistories
                .Where(x => x.ProductId == product.Id)
                .OrderByDescending(x => x.EffectiveDate)
                .ToList();

            var relatedProducts = _context.Products
                .Where(x => x.CategoryId == product.CategoryId
                            && x.Id != product.Id
                            && x.Status == "active")
                .OrderByDescending(x => x.IsFeatured)
                .ThenByDescending(x => x.CreatedAt)
                .Take(6)
                .ToList();

            var vm = new ProductDetailViewModel
            {
                Product = product,
                Category = category,
                Images = images,
                PriceHistories = priceHistories,
                RelatedProducts = relatedProducts
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}