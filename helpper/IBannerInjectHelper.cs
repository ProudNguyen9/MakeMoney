namespace WebThuMuaPheLieu.helpper;

public interface IBannerInjectHelper
{
    Task<IReadOnlyList<BannerInjectSettings>> GetActiveBannersAsync();
}
