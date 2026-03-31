using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;
using WebThuMuaPheLieu.ViewModels;

namespace WebThuMuaPheLieu.Controllers;

public class PricingController : Controller
{
    private readonly AppDbContext _context;

    public PricingController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Where(product => product.Status == "active")
            .OrderBy(product => product.Category!.Name)
            .ThenBy(product => product.Name)
            .ToListAsync();

        var pricingRows = products
            .Select((product, index) => new PricingRowViewModel
            {
                Id = product.Id,
                OrderNumber = index + 1,
                ProductName = product.Name ?? "Chưa có tên",
                CategoryName = product.Category?.Name ?? "Chưa phân loại",
                PriceValue = product.PriceValue,
                PriceText = FormatPrice(product.PriceValue, product.PriceLabel),
                UnitText = BuildUnitText(product.Unit),
                StatusText = "Đang thu",
                StatusCssClass = ResolveStatusCssClass(product.Status, product.PriceLabel),
                UpdatedAt = product.UpdatedAt,
                UpdatedText = FormatUpdatedText(product.UpdatedAt, null)
            })
            .ToList();

        var viewModel = new PricingIndexViewModel
        {
            Prices = pricingRows
        };

        return View(viewModel);
    }

    private static string FormatPrice(decimal? priceValue, string? priceLabel)
    {
        var normalizedLabel = priceLabel?.Trim();

        if (priceValue.GetValueOrDefault() == 0
            && string.Equals(normalizedLabel, "Liên hệ báo giá", StringComparison.OrdinalIgnoreCase))
        {
            return normalizedLabel;
        }

        return priceValue.HasValue
            ? string.Format(CultureInfo.GetCultureInfo("vi-VN"), "{0:N0}", priceValue.Value)
            : "Liên hệ";
    }

    private static string BuildUnitText(string? unit)
    {
        if (string.IsNullOrWhiteSpace(unit))
        {
            return "VNĐ/kg";
        }

        var normalizedUnit = unit.Trim();

        if (normalizedUnit.Contains("VNĐ", StringComparison.OrdinalIgnoreCase))
        {
            return normalizedUnit;
        }

        return $"VNĐ/{normalizedUnit}";
    }

    private static string ResolveStatusCssClass(string? priceType, string? note)
    {
        var normalized = $"{priceType} {note}".Trim().ToLowerInvariant();

        if (normalized.Contains("ngừng thu")
            || normalized.Contains("tam ngung")
            || normalized.Contains("tạm ngừng")
            || normalized.Contains("dung thu")
            || normalized.Contains("dừng thu")
            || normalized.Contains("ngung mua")
            || normalized.Contains("ngừng mua"))
        {
            return "badge-warning-light";
        }

        return "badge-info-light";
    }

    private static string FormatUpdatedText(DateTime? recordedAt, DateOnly? effectiveDate)
    {
        var updatedAt = recordedAt ?? ConvertToDateTime(effectiveDate);
        if (!updatedAt.HasValue)
        {
            return "Hôm nay";
        }

        var duration = DateTime.Now - updatedAt.Value;

        if (duration.TotalMinutes < 60)
        {
            var minutes = Math.Max(1, (int)Math.Floor(duration.TotalMinutes));
            return $"{minutes} phút trước";
        }

        if (duration.TotalHours < 24)
        {
            var hours = Math.Max(1, (int)Math.Floor(duration.TotalHours));
            return $"{hours} giờ trước";
        }

        if (updatedAt.Value.Date == DateTime.Now.Date)
        {
            return "Hôm nay";
        }

        return updatedAt.Value.ToString("dd/MM/yyyy");
    }

    private static DateTime? ConvertToDateTime(DateOnly? date)
    {
        return date?.ToDateTime(TimeOnly.MinValue);
    }
}
