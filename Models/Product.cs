using System;
using System.Collections.Generic;

namespace WebThuMuaPheLieu.Models;

public partial class Product
{
    public int Id { get; set; }

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

    public bool? IsFeatured { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ProductCategory? Category { get; set; }

    public virtual ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
