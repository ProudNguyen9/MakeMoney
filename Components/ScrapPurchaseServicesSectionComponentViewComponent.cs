using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;
using WebThuMuaPheLieu.ViewModels;

namespace WebThuMuaPheLieu.Components
{
    public class ScrapPurchaseServicesSectionComponentViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public ScrapPurchaseServicesSectionComponentViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = await _context.BlogPosts
                .AsNoTracking()
                .Include(post => post.BlogImages)
                .Where(post => post.Status == "published"
                    && _context.BlogPostCategories.Any(category => category.PostId == post.Id && category.CategoryId == 1))
                .OrderByDescending(post => post.PublishedAt ?? post.CreatedAt)
                .ThenByDescending(post => post.Id)
                .Take(8)
                .Select(post => new BlogCardViewModel
                {
                    Id = post.Id,
                    Slug = !string.IsNullOrWhiteSpace(post.Slug) ? post.Slug : post.Id.ToString(),
                    Title = post.Title ?? "Dịch vụ thu mua phế liệu",
                    Summary = !string.IsNullOrWhiteSpace(post.Excerpt) ? post.Excerpt : (post.Content ?? string.Empty),
                    PublishedAt = post.PublishedAt ?? post.CreatedAt,
                    CoverImage = string.IsNullOrWhiteSpace(post.CoverImage) ? "/assets/img/blog/blog-1.jpg" : post.CoverImage,
                    ImageSequence = post.BlogImages
                        .OrderBy(image => image.OrderIndex ?? int.MaxValue)
                        .ThenBy(image => image.Id)
                        .Select(image => image.ImageUrl ?? string.Empty)
                        .Where(imageUrl => imageUrl != string.Empty)
                        .ToList()
                })
                .ToListAsync();

            foreach (var post in posts)
            {
                var imageSequence = new List<string>();

                if (!string.IsNullOrWhiteSpace(post.CoverImage))
                {
                    imageSequence.Add(post.CoverImage);
                }

                imageSequence.AddRange(post.ImageSequence);
                post.ImageSequence = imageSequence
                    .Where(imageUrl => !string.IsNullOrWhiteSpace(imageUrl))
                    .Select(NormalizeImagePath)
                    .Distinct(System.StringComparer.OrdinalIgnoreCase)
                    .ToList();

                post.CoverImage = post.ImageSequence.FirstOrDefault() ?? "/assets/img/blog/blog-1.jpg";
            }

            var viewModel = new ScrapPurchaseServicesSectionComponentViewModel
            {
                Posts = posts
            };

            return View(viewModel);
        }

        private static string NormalizeImagePath(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return "/assets/img/blog/blog-1.jpg";
            }

            var normalized = imagePath.Replace("\\", "/").Trim();

            if (normalized.StartsWith("~/"))
            {
                return $"/{normalized[2..]}";
            }

            if (normalized.StartsWith("http://") || normalized.StartsWith("https://") || normalized.StartsWith("/"))
            {
                return normalized;
            }

            return $"/{normalized.TrimStart('/')}";
        }
    }
}
