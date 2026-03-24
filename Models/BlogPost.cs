using System;
using System.Collections.Generic;

namespace WebThuMuaPheLieu.Models;

public partial class BlogPost
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Slug { get; set; }

    public string? Excerpt { get; set; }

    public string? Content { get; set; }

    public string? CoverImage { get; set; }

    public int? AuthorId { get; set; }

    public DateTime? PublishedAt { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Admin? Author { get; set; }
}
