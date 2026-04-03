namespace WebThuMuaPheLieu.ViewModels;

public class AdminMarketingViewModel
{
    public int? BannerId { get; set; }

    public string BannerLine1 { get; set; } = "Chuyên thu mua";
    public string BannerLine2 { get; set; } = "phế liệu giá cao";
    public string BannerLine3 { get; set; } = "uy tín hàng đầu";
    public string BannerLine4 { get; set; } = "cho doanh nghiệp";

    public string BannerImage1 { get; set; } = "/assets/img/satthep.png";
    public string BannerImage2 { get; set; } = "/assets/img/dongnhom.jpg";
    public string BannerImage3 { get; set; } = "/assets/img/maymoc.jpg";

    public string Phone { get; set; } = "0909 123 456";
    public string Zalo { get; set; } = "zalo.me/0909123456";
    public string Messenger { get; set; } = "m.me/phelieupro";
    public string Facebook { get; set; } = "facebook.com/phelieupro";
    public string Email { get; set; } = "contact@phelieupro.vn";
    public string Address { get; set; } = "Quận 12, TP. Hồ Chí Minh";
    public string PurchaseAreas { get; set; } = "TP.HCM, Bình Dương, Đồng Nai, Long An và các khu vực lân cận";

    public string MetaTitle { get; set; } = "Thu mua phế liệu giá cao tại TP.HCM | Phế Liệu Pro";
    public string MetaDescription { get; set; } = "Dịch vụ thu mua phế liệu tận nơi, báo giá nhanh, hỗ trợ doanh nghiệp và hộ gia đình tại TP.HCM và khu vực lân cận.";
    public string SeoKeywords { get; set; } = "thu mua phế liệu, phế liệu giá cao, thu mua tận nơi";
    public string OgTitle { get; set; } = "Phế Liệu Pro - Thu mua tận nơi";
    public string OgImage { get; set; } = "/assets/img/satthep.png";
}
