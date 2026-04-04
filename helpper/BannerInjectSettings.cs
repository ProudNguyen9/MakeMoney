namespace WebThuMuaPheLieu.helpper;

public class BannerInjectSettings
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string TitleLine1 { get; set; } = string.Empty;

    public string TitleLine2 { get; set; } = string.Empty;

    public string TitleLine3 { get; set; } = string.Empty;

    public string TitleLine4 { get; set; } = string.Empty;

    public string Subtitle { get; set; } = string.Empty;

    public string SellText { get; set; } = string.Empty;

    public string ButtonPrimaryText { get; set; } = string.Empty;

    public string ButtonPrimaryLink { get; set; } = string.Empty;

    public string ButtonSecondaryText { get; set; } = string.Empty;

    public string ButtonSecondaryLink { get; set; } = string.Empty;

    public int OrderIndex { get; set; }

    public string Status { get; set; } = string.Empty;

    public IReadOnlyList<BannerInjectImageSettings> Images { get; set; } = Array.Empty<BannerInjectImageSettings>();
}

public class BannerInjectImageSettings
{
    public int Id { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string Caption { get; set; } = string.Empty;

    public int OrderIndex { get; set; }
}
