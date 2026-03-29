using System.Collections.Generic;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Product> Products { get; set; } = new();

        public RecentNewsSectionComponentViewModel RecentNewsSection { get; set; } = new();

        public ScrapPurchaseServicesSectionComponentViewModel ScrapPurchaseServicesSection { get; set; } = new();
    }
}
