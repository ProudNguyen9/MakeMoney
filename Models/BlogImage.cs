using System;

namespace WebThuMuaPheLieu.Models;

public partial class BlogImage
{
    public int Id { get; set; }

    public int? BlogId { get; set; }

    public string? ImageUrl { get; set; }

    public string? Caption { get; set; }

    public int? OrderIndex { get; set; }

    public virtual BlogPost? Blog { get; set; }
}
