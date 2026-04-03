using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.ViewModels;

public class AdminProductsViewModel
{
    public List<AdminProductListItemViewModel> Products { get; set; } = new();

    public string? SearchTerm { get; set; }

    public int CurrentPage { get; set; } = 1;

    public int PageSize { get; set; }

    public int TotalProducts { get; set; }

    public int TotalPages { get; set; }

    public int StartItemIndex => TotalProducts == 0 ? 0 : ((CurrentPage - 1) * PageSize) + 1;

    public int EndItemIndex => TotalProducts == 0 ? 0 : Math.Min(CurrentPage * PageSize, TotalProducts);

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;

    public List<AdminCategoryListItemViewModel> Categories { get; set; } = new();

    public AdminProductEditorViewModel Editor { get; set; } = new();

    public AdminCategoryEditorViewModel CategoryEditor { get; set; } = new();

    public List<SelectListItem> CategoryOptions { get; set; } = new();

    public List<SelectListItem> StatusOptions { get; set; } =
    [
        new() { Value = "draft", Text = "Nháp" },
        new() { Value = "active", Text = "Đang hiển thị" },
        new() { Value = "inactive", Text = "Tạm ẩn" }
    ];

    public bool ShowEditor { get; set; }

    public bool ShowCategoryManager { get; set; }

    public bool IsCreateMode => !Editor.Id.HasValue;

    public bool IsCategoryCreateMode => !CategoryEditor.Id.HasValue;
}

public class AdminProductListItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? CategoryName { get; set; }

    public decimal? PriceValue { get; set; }

    public string? Unit { get; set; }

    public string? PriceLabel { get; set; }

    public string? PrimaryImage { get; set; }

    public string? Status { get; set; }

    public bool IsFeatured { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int ImageCount { get; set; }
}

public class AdminCategoryListItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int ProductCount { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public class AdminProductEditorViewModel
{
    public int? Id { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public decimal? PriceValue { get; set; }

    public string? Unit { get; set; }

    public string? PriceLabel { get; set; }

    public string? PrimaryImage { get; set; }

    public string? Status { get; set; }

    public bool IsFeatured { get; set; }

    public List<AdminProductImageItemViewModel> ExistingImages { get; set; } = new();

    public List<int> RemovedImageIds { get; set; } = new();

    public bool RemovePrimaryImage { get; set; }

    public List<AdminPriceHistoryItemViewModel> PriceHistories { get; set; } = new();

    public List<IFormFile> UploadedImages { get; set; } = new();
}

public class AdminCategoryEditorViewModel
{
    public int? Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }
}

public class AdminProductImageItemViewModel
{
    public int? Id { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? Caption { get; set; }

    public int OrderIndex { get; set; }

    public bool IsPrimary { get; set; }
}

public class AdminPriceHistoryItemViewModel
{
    public int Id { get; set; }

    public decimal? PriceValue { get; set; }

    public string? PriceUnit { get; set; }

    public string? PriceType { get; set; }

    public string? Note { get; set; }

    public DateOnly? EffectiveDate { get; set; }

    public DateTime? RecordedAt { get; set; }
}
