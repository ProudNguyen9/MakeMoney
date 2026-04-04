using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.helpper;

public class ContactInfoHelper : IContactInfoHelper
{
    private readonly AppDbContext _context;

    public ContactInfoHelper(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ContactInfoSettings> GetContactInfoAsync()
    {
        var keys = new[]
        {
            "contact.phone",
            "contact.zalo",
            "contact.messenger",
            "contact.facebook",
            "contact.email",
            "contact.address",
            "contact.purchase_areas"
        };

        var settings = await _context.SiteSettings
            .Where(item => item.SettingKey != null && keys.Contains(item.SettingKey))
            .ToDictionaryAsync(item => item.SettingKey!, item => item.SettingValue ?? string.Empty);

        return new ContactInfoSettings
        {
            Phone = GetValue(settings, "contact.phone", "0909 123 45699"),
            Zalo = GetValue(settings, "contact.zalo", "zalo.me/0909123456"),
            Messenger = GetValue(settings, "contact.messenger", "m.me/phelieupro"),
            Facebook = GetValue(settings, "contact.facebook", "facebook.com/phelieupro"),
            Gmail = GetValue(settings, "contact.email", "contact@phelieupro.vn"),
            Address = GetValue(settings, "contact.address", "Quận 12, TP. Hồ Chí Minh"),
            PurchaseAreas = GetValue(settings, "contact.purchase_areas", string.Empty)
        };
    }

    private static string GetValue(IReadOnlyDictionary<string, string> settings, string key, string fallback)
    {
        return settings.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : fallback;
    }
}
