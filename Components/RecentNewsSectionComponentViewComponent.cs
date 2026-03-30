using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;
using WebThuMuaPheLieu.ViewModels;

namespace WebThuMuaPheLieu.Components
{
    public class RecentNewsSectionComponentViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public RecentNewsSectionComponentViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = await _context.BlogPosts
                .Where(post => post.Status == "published"
                    && !_context.BlogPostCategories.Any(category => category.PostId == post.Id && category.CategoryId == 1))
                .OrderByDescending(post => post.PublishedAt ?? post.CreatedAt)
                .ThenByDescending(post => post.Id)
                .Take(8)
                .ToListAsync();

            var viewModel = new RecentNewsSectionComponentViewModel
            {
                Posts = posts
            };

            return View(viewModel);
        }
    }
}
