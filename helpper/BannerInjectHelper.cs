using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.helpper;

public class BannerInjectHelper : IBannerInjectHelper
{
    private const string TitleSeparator = "|||";

    private readonly AppDbContext _context;

    public BannerInjectHelper(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<BannerInjectSettings>> GetActiveBannersAsync()
    {
        var banners = await _context.Banners
            .AsNoTracking()
            .Include(item => item.BannerImages.OrderBy(image => image.OrderIndex).ThenBy(image => image.Id))
            .Where(item => item.Status != null && item.Status.ToLower() == "active")
            .OrderBy(item => item.OrderIndex)
            .ThenBy(item => item.Id)
            .ToListAsync();

        return banners.Select(MapBanner).ToList();
    }

    private static BannerInjectSettings MapBanner(Banner banner)
    {
        var title = banner.Title ?? string.Empty;
        var titleLines = title.Split(TitleSeparator, StringSplitOptions.None)
            .Select(line => line.Trim())
            .ToArray();

        return new BannerInjectSettings
        {
            Id = banner.Id,
            Title = title,
            TitleLine1 = titleLines.Length > 0 ? titleLines[0] : string.Empty,
            TitleLine2 = titleLines.Length > 1 ? titleLines[1] : string.Empty,
            TitleLine3 = titleLines.Length > 2 ? titleLines[2] : string.Empty,
            TitleLine4 = titleLines.Length > 3 ? titleLines[3] : string.Empty,
            Subtitle = banner.Subtitle ?? string.Empty,
            SellText = banner.SellText ?? string.Empty,
            ButtonPrimaryText = banner.ButtonPrimaryText ?? string.Empty,
            ButtonPrimaryLink = banner.ButtonPrimaryLink ?? string.Empty,
            ButtonSecondaryText = banner.ButtonSecondaryText ?? string.Empty,
            ButtonSecondaryLink = banner.ButtonSecondaryLink ?? string.Empty,
            OrderIndex = banner.OrderIndex ?? 0,
            Status = banner.Status ?? string.Empty,
            Images = banner.BannerImages
                .OrderBy(image => image.OrderIndex)
                .ThenBy(image => image.Id)
                .Select(image => new BannerInjectImageSettings
                {
                    Id = image.Id,
                    ImageUrl = image.ImageUrl ?? string.Empty,
                    Caption = image.Caption ?? string.Empty,
                    OrderIndex = image.OrderIndex ?? 0
                })
                .ToList()
        };
    }
}
