using System.Collections.Generic;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.ViewModels
{
    public class ProductDetailViewModel
    {
        // Sản phẩm chính
        public Product Product { get; set; }

        // Danh mục
        public ProductCategory? Category { get; set; }

        // Danh sách ảnh
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();

        // Lịch sử giá
        public List<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();

        // Sản phẩm liên quan
        public List<Product> RelatedProducts { get; set; } = new List<Product>();
    }
}