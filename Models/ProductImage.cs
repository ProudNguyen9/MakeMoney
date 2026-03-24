using System;
using System.Collections.Generic;

namespace WebThuMuaPheLieu.Models;

public partial class ProductImage
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string? ImageUrl { get; set; }

    public string? Caption { get; set; }

    public int? OrderIndex { get; set; }

    public virtual Product? Product { get; set; }
}
