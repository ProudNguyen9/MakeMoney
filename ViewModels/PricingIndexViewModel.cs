using System;
using System.Collections.Generic;

namespace WebThuMuaPheLieu.ViewModels;

public class PricingRowViewModel
{
    public int Id { get; set; }

    public int OrderNumber { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = "Chưa phân loại";

    public decimal? PriceValue { get; set; }

    public string PriceText { get; set; } = "Liên hệ";

    public string UnitText { get; set; } = "VNĐ / kg";

    public string StatusText { get; set; } = "Đang thu";

    public string StatusCssClass { get; set; } = "badge-info-light";

    public DateTime? UpdatedAt { get; set; }

    public string UpdatedText { get; set; } = "Hôm nay";
}

public class PricingIndexViewModel
{
    public List<PricingRowViewModel> Prices { get; set; } = [];
}
