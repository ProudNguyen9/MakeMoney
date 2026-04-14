using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.helpper;
using WebThuMuaPheLieu.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://192.168.30.1:5000");

// ================== SERVICES ==================
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/admin/login";
        options.AccessDeniedPath = "/admin/login";
        options.Cookie.Name = "WebThuMuaPheLieu.AdminAuth";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// ✅ Đăng ký DI
builder.Services.AddScoped<IContactInfoHelper, ContactInfoHelper>();
builder.Services.AddScoped<IBannerInjectHelper, BannerInjectHelper>();
builder.Services.AddScoped<ISeoSettingHelper, SeoSettingHelper>(); // ⭐ THÊM DÒNG NÀY

var app = builder.Build();

// ================== PIPELINE ==================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// QUAN TRỌNG: phải có dòng này
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();