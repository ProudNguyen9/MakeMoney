using System.Collections.Generic;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.ViewModels;

public class FeaturedProductsSectionViewModel
{
    public int? Count { get; set; }

    public string ViewName { get; set; } = "Pricing";

    public IReadOnlyList<Product> Products { get; set; } = new List<Product>();
}
