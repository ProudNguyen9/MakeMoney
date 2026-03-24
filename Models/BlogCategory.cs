using System;
using System.Collections.Generic;

namespace WebThuMuaPheLieu.Models;

public partial class BlogCategory
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Slug { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
