using System.Collections.Generic;
using WebThuMuaPheLieu.helpper;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.ViewModels
{
    public class HomeIndexViewModel
    {
        public BannerInjectSettings? Banner { get; set; }

        public ContactInfoSettings ContactInfo { get; set; } = new();

        public List<Product> Products { get; set; } = new();

        public RecentNewsSectionComponentViewModel RecentNewsSection { get; set; } = new();

        public ScrapPurchaseServicesSectionComponentViewModel ScrapPurchaseServicesSection { get; set; } = new();
    }
}
