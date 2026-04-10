using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;
using WebThuMuaPheLieu.ViewModels;

namespace WebThuMuaPheLieu.Controllers;

public class ContactController : Controller
{
    private readonly AppDbContext _context;

    public ContactController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = await BuildContactPageViewModelAsync();
        ViewData["Title"] = "Liên hệ";
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactPageViewModel model)
    {
        model = await BuildContactPageViewModelAsync(model);

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            TempData["ContactError"] = "Vui lòng nhập họ và tên.";
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.PhoneInput))
        {
            TempData["ContactError"] = "Vui lòng nhập số điện thoại.";
            return View(model);
        }

        var now = DateTime.Now;
        var contactRequest = new ContactRequest
        {
            Name = model.Name.Trim(),
            Phone = model.PhoneInput.Trim(),
            Email = string.IsNullOrWhiteSpace(model.EmailInput) ? null : model.EmailInput.Trim(),
            RequestType = NormalizeRequestType(model.RequestType),
            Area = string.IsNullOrWhiteSpace(model.Area) ? null : model.Area.Trim(),
            Message = string.IsNullOrWhiteSpace(model.Message) ? null : model.Message.Trim(),
            SourcePage = HttpContext?.Request?.Path.Value ?? "/contact",
            Status = "new",
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.ContactRequests.Add(contactRequest);
        await _context.SaveChangesAsync();

        TempData["ContactSuccess"] = "Thông tin của bạn đã được gửi. Chúng tôi sẽ liên hệ lại sớm nhất.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<ContactPageViewModel> BuildContactPageViewModelAsync(ContactPageViewModel? input = null)
    {
        var model = input ?? new ContactPageViewModel();

        var settings = await _context.SiteSettings
            .AsNoTracking()
            .Where(item => item.SettingKey != null && item.SettingKey.StartsWith("contact."))
            .ToListAsync();

        string? GetSetting(string key)
        {
            return settings.FirstOrDefault(item => string.Equals(item.SettingKey, key, StringComparison.OrdinalIgnoreCase))?.SettingValue;
        }

        model.Phone = GetSetting("contact.phone") ?? model.Phone;
        model.Email = GetSetting("contact.email") ?? model.Email;
        model.Address = GetSetting("contact.address") ?? model.Address;
        model.PurchaseAreas = GetSetting("contact.purchase_areas") ?? model.PurchaseAreas;
        model.Facebook = NormalizeLink(GetSetting("contact.facebook"), "#");
        model.Zalo = NormalizeLink(GetSetting("contact.zalo"), "#");
        model.Messenger = NormalizeLink(GetSetting("contact.messenger"), "#");

        return model;
    }

    private static string NormalizeLink(string? value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        var trimmed = value.Trim();
        if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("tel:", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed;
        }

        if (trimmed.Contains('@'))
        {
            return $"mailto:{trimmed}";
        }

        return $"https://{trimmed.TrimStart('/')}";
    }

    private static string NormalizeRequestType(string? value)
    {
        var normalizedValue = value?.Trim().ToLowerInvariant();

        return normalizedValue switch
        {
            "khao-sat-kho-xuong" => "khao-sat-kho-xuong",
            "don-kho-thanh-ly" => "don-kho-thanh-ly",
            "thu-gom-dinh-ky" => "thu-gom-dinh-ky",
            "thu-mua-phe-lieu-tan-noi" => "thu-mua-phe-lieu-tan-noi",
            _ => "thu-mua-phe-lieu-tan-noi"
        };
    }
}
