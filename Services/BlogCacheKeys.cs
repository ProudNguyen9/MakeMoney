namespace WebThuMuaPheLieu.Services;

public static class BlogCacheKeys
{
    public const string CategoryFilters = "blog:category-filters:v1";
    public const string FeaturedPosts = "blog:featured-posts:v1";
    public const string TotalPublishedPosts = "blog:total-published:v1";

    public static string RelatedPosts(int postId)
    {
        return $"blog:related:{postId}:v1";
    }
}

