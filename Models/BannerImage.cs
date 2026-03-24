using System;
using System.Collections.Generic;

namespace WebThuMuaPheLieu.Models;

public partial class BannerImage
{
    public int Id { get; set; }

    public int? BannerId { get; set; }

    public string? ImageUrl { get; set; }

    public string? Caption { get; set; }

    public int? OrderIndex { get; set; }

    public virtual Banner? Banner { get; set; }
}
