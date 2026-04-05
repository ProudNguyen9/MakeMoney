// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.EntityFrameworkCore;
// using WebThuMuaPheLieu.helpper;
// using WebThuMuaPheLieu.Models;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddDistributedMemoryCache();
// builder.Services.AddSession(options =>
// {
//     options.IdleTimeout = TimeSpan.FromMinutes(30);
//     options.Cookie.HttpOnly = true;
//     options.Cookie.IsEssential = true;
// });
// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie(options =>
//     {
//         options.LoginPath = "/admin/login";
//         options.AccessDeniedPath = "/admin/login";
//         options.Cookie.Name = "WebThuMuaPheLieu.AdminAuth";
//         options.ExpireTimeSpan = TimeSpan.FromHours(8);
//         options.SlidingExpiration = true;
//     });
// builder.Services.AddControllersWithViews();
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
// builder.Services.AddScoped<IContactInfoHelper, ContactInfoHelper>();
// builder.Services.AddScoped<IBannerInjectHelper, BannerInjectHelper>();
// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
// app.UseRouting();
// app.UseSession();

// app.UseAuthentication();

// app.UseAuthorization();

// app.MapStaticAssets();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}")
//     .WithStaticAssets();


// app.Run();

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebThuMuaPheLieu.helpper;
using WebThuMuaPheLieu.Models;

var builder = WebApplication.CreateBuilder(args);

// Cho app lắng nghe toàn bộ mạng LAN ở port 5000
builder.WebHost.UseUrls("http://192.168.2.9:5000");

// Add services to the container.
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IContactInfoHelper, ContactInfoHelper>();
builder.Services.AddScoped<IBannerInjectHelper, BannerInjectHelper>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Tạm thời comment dòng này để test bằng điện thoại trong LAN
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
