namespace WebThuMuaPheLieu.Models;

public class AdminOverviewViewModel
{
    public int TotalProducts { get; set; }

    public int TotalPublishedPosts { get; set; }

    public int NewContactRequests { get; set; }

    public DateOnly? LatestPriceUpdateDate { get; set; }

    public int ProductsWithoutImages { get; set; }

    public int BlogsWithoutImages { get; set; }
}
