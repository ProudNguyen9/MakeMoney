using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebThuMuaPheLieu.ViewModels;

public class AdminPricesViewModel
{
    public List<AdminPriceProductItemViewModel> Products { get; set; } = new();

    public List<SelectListItem> ProductOptions { get; set; } = new();

    public AdminCurrentPriceEditorViewModel CurrentPriceEditor { get; set; } = new();

    public bool ShowCurrentPriceEditor { get; set; }

    public List<AdminPriceHistoryListItemViewModel> PriceHistories { get; set; } = new();

    public AdminPriceHistoryEditorViewModel HistoryEditor { get; set; } = new();

    public bool ShowHistoryEditor { get; set; }

    public bool IsHistoryCreateMode => !HistoryEditor.Id.HasValue;
}

public class AdminPriceProductItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string CategoryName { get; set; } = "Chưa phân loại";

    public decimal? PriceValue { get; set; }

    public string? Unit { get; set; }

    public string? PriceLabel { get; set; }

    public string? Status { get; set; }

    public bool IsSelected { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public class AdminCurrentPriceEditorViewModel
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = "Chưa phân loại";

    public decimal? PriceValue { get; set; }

    public string? Unit { get; set; }

    public string? PriceLabel { get; set; }

    public string? Status { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public class AdminPriceHistoryListItemViewModel
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal? PriceValue { get; set; }

    public string? PriceUnit { get; set; }

    public string? PriceType { get; set; }

    public string? Note { get; set; }

    public DateOnly? EffectiveDate { get; set; }

    public DateTime? RecordedAt { get; set; }

    public bool IsSelected { get; set; }
}

public class AdminPriceHistoryEditorViewModel
{
    public int? Id { get; set; }

    public int ProductId { get; set; }

    public decimal? PriceValue { get; set; }

    public string? PriceUnit { get; set; }

    public string? PriceType { get; set; }

    public string? Note { get; set; }

    public DateOnly? EffectiveDate { get; set; }

    public DateTime? RecordedAt { get; set; }
}
