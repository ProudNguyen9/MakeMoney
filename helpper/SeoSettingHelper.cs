using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.helpper;

public class SeoSettingHelper : ISeoSettingHelper
{
    private const string SeoPrefix = "seo.";
    private static readonly IReadOnlyDictionary<string, string> SeoKeyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["seo.meta_title"] = "MetaTitle",
        ["seo.meta_description"] = "MetaDescription",
        ["seo.keywords"] = "SeoKeywords",
        ["seo.og_title"] = "OgTitle",
        ["seo.og_image"] = "OgImage"
    };

    private readonly AppDbContext _context;

    public SeoSettingHelper(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyDictionary<string, string>> GetSeoSettingsAsync()
    {
        var settings = await _context.SiteSettings
            .AsNoTracking()
            .Where(item => item.SettingKey != null && item.SettingKey.ToLower().StartsWith(SeoPrefix))
            .ToListAsync();

        return settings
            .Select(item => new
            {
                Key = NormalizeKey(item.SettingKey),
                Value = item.SettingValue ?? string.Empty
            })
            .Where(item => !string.IsNullOrWhiteSpace(item.Key))
            .GroupBy(item => item.Key)
            .ToDictionary(
                group => group.Key,
                group => group.Last().Value,
                StringComparer.OrdinalIgnoreCase);
    }

    public async Task<string> GetSeoSettingValueAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return string.Empty;
        }

        var normalizedKey = NormalizeKey(key);

        var setting = await _context.SiteSettings
            .AsNoTracking()
            .Where(item => item.SettingKey != null)
            .FirstOrDefaultAsync(item => NormalizeKey(item.SettingKey) == normalizedKey);

        return setting?.SettingValue ?? string.Empty;
    }

    public static string NormalizeKey(string? settingKey)
    {
        if (string.IsNullOrWhiteSpace(settingKey))
        {
            return string.Empty;
        }

        var normalized = settingKey.Trim();

        if (SeoKeyMap.TryGetValue(normalized, out var mappedKey))
        {
            return mappedKey;
        }

        if (normalized.StartsWith(SeoPrefix, StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized.Substring(SeoPrefix.Length);
        }

        return normalized.Replace('.', ' ').Replace('_', ' ').Trim();
    }
}
