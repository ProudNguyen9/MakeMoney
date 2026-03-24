using System;
using System.Collections.Generic;

namespace WebThuMuaPheLieu.Models;

public partial class BlogPostCategory
{
    public int? PostId { get; set; }

    public int? CategoryId { get; set; }

    public virtual BlogCategory? Category { get; set; }

    public virtual BlogPost? Post { get; set; }
}
