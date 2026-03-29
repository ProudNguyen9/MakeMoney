namespace WebThuMuaPheLieu.Models;

public class ProductCardViewModel
{
    public int Id { get; set; }

    public string? Slug { get; set; }

    public string? CategoryIcon { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? CategoryName { get; set; }

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public string? PriceLabel { get; set; }

    public string? Unit { get; set; }

    public string PrimaryImage { get; set; } = string.Empty;

    public List<string> Images { get; set; } = [];
}

public class ProductIndexViewModel
{
    public List<ProductCardViewModel> Products { get; set; } = [];

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }
}
