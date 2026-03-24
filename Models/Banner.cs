using System;
using System.Collections.Generic;

namespace WebThuMuaPheLieu.Models;

public partial class Banner
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Subtitle { get; set; }

    public string? SellText { get; set; }

    public string? ButtonPrimaryText { get; set; }

    public string? ButtonPrimaryLink { get; set; }

    public string? ButtonSecondaryText { get; set; }

    public string? ButtonSecondaryLink { get; set; }

    public int? OrderIndex { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BannerImage> BannerImages { get; set; } = new List<BannerImage>();
}
