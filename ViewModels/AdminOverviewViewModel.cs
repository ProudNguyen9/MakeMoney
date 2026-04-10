namespace WebThuMuaPheLieu.Models;

public class AdminOverviewViewModel
{
    public int TotalProducts { get; set; }

    public int TotalPublishedPosts { get; set; }

    public int NewContactRequests { get; set; }

    public DateOnly? LatestPriceUpdateDate { get; set; }

    public int ProductsWithoutImages { get; set; }

    public int BlogsWithoutImages { get; set; }

    public List<AdminContactRequestOverviewItemViewModel> NewContactRequestItems { get; set; } = [];
}

public class AdminContactRequestOverviewItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string RequestType { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string SourcePage { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }

    public string RequestTypeDisplayName => RequestType switch
    {
        "khao-sat-kho-xuong" => "Khảo sát kho xưởng",
        "don-kho-thanh-ly" => "Dọn kho thanh lý",
        "thu-gom-dinh-ky" => "Thu gom định kỳ",
        "thu-mua-phe-lieu-tan-noi" => "Thu mua phế liệu tận nơi",
        _ => string.IsNullOrWhiteSpace(RequestType) ? string.Empty : RequestType
    };
}
