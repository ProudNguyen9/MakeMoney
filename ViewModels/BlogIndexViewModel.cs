namespace WebThuMuaPheLieu.Models;

public class BlogCardViewModel
{
    public int Id { get; set; }

    public string? Slug { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string CoverImage { get; set; } = string.Empty;

    public string CategoryName { get; set; } = "Tin tức";

    public DateTime? PublishedAt { get; set; }

    public int LikeCount { get; set; }
}

public class BlogCategoryFilterViewModel
{
    public string Name { get; set; } = string.Empty;

    public int PostCount { get; set; }
}

public class BlogIndexViewModel
{
    public List<BlogCardViewModel> Posts { get; set; } = [];

    public List<BlogCardViewModel> FeaturedPosts { get; set; } = [];

    public List<BlogCategoryFilterViewModel> Categories { get; set; } = [];

    public string SearchTerm { get; set; } = string.Empty;

    public string SelectedCategoryName { get; set; } = string.Empty;

    public int TotalPublishedPosts { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }
}

public class BlogDetailViewModel
{
    public BlogPost Post { get; set; } = new();

    public string CoverImage { get; set; } = string.Empty;

    public string PrimaryCategoryName { get; set; } = "Tin tức";

    public string AuthorName { get; set; } = string.Empty;

    public string CurrentUrl { get; set; } = string.Empty;

    public List<BlogCardViewModel> RelatedPosts { get; set; } = [];

    public List<BlogCategoryFilterViewModel> Categories { get; set; } = [];
}
