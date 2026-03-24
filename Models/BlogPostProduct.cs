using System;
using System.Collections.Generic;

namespace WebThuMuaPheLieu.Models;

public partial class BlogPostProduct
{
    public int? PostId { get; set; }

    public int? ProductId { get; set; }

    public virtual BlogPost? Post { get; set; }

    public virtual Product? Product { get; set; }
}
