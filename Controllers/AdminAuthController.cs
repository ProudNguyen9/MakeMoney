using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.Models;
using WebThuMuaPheLieu.ViewModels;

namespace WebThuMuaPheLieu.Controllers;

[Route("admin/login")]
public class AdminAuthController : Controller
{
    private readonly AppDbContext _context;

    public AdminAuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        ViewData["Title"] = "Đăng nhập quản trị";
        return View(new AdminLoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AdminLoginViewModel model)
    {
        ViewData["Title"] = "Đăng nhập quản trị";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedUsername = model.Username.Trim().ToLower();
        var admin = await _context.Admins
            .FirstOrDefaultAsync(x => x.Username != null && x.Username.ToLower() == normalizedUsername);

        if (admin is null || !IsActive(admin.Status) || !VerifySha256(model.Password, admin.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new(ClaimTypes.Name, admin.Username ?? string.Empty),
            new(ClaimTypes.GivenName, admin.FullName ?? admin.Username ?? "Admin"),
            new(ClaimTypes.Role, admin.Role ?? "admin")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        return RedirectToLocal(model.ReturnUrl);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Overview", "Admin");
    }

    private static bool IsActive(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return true;
        }

        var normalized = status.Trim().ToLowerInvariant();
        return normalized is "active" or "enabled" or "1" or "hoatdong" or "hoạt động";
    }

    private static bool VerifySha256(string password, string? passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        var computedHash = Convert.ToHexString(bytes);
        var normalizedStoredHash = NormalizeSqlServerHash(passwordHash);
        return string.Equals(computedHash, normalizedStoredHash, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeSqlServerHash(string passwordHash)
    {
        var normalized = passwordHash.Trim();

        if (normalized.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized[2..];
        }

        return normalized.Replace("-", string.Empty).Replace(" ", string.Empty);
    }
}
