using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using WebThuMuaPheLieu.Services;
using WebThuMuaPheLieu.helpper;
using WebThuMuaPheLieu.Models;

var builder = WebApplication.CreateBuilder(args);

var dataProtectionKeysPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "DataProtection-Keys");
Directory.CreateDirectory(dataProtectionKeysPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));

builder.Services.AddMemoryCache();
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("BlogList", policy =>
    {
        policy.Expire(TimeSpan.FromSeconds(45));
        policy.SetVaryByQuery(["page", "searchTerm", "categoryName"]);
    });

    options.AddPolicy("BlogDetail", policy =>
    {
        policy.Expire(TimeSpan.FromMinutes(2));
        policy.SetVaryByQuery(["slug"]);
    });

    options.AddPolicy("BlogPagedApi", policy =>
    {
        policy.Expire(TimeSpan.FromSeconds(30));
        policy.SetVaryByQuery(["page", "searchTerm", "categoryName"]);
    });
});
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

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
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IContactInfoHelper, ContactInfoHelper>();
builder.Services.AddScoped<IBannerInjectHelper, BannerInjectHelper>();
builder.Services.AddScoped<ISeoSettingHelper, SeoSettingHelper>();
builder.Services.AddScoped<IStructuredDataService, StructuredDataService>();
builder.Services.AddSingleton<IBlogImageProcessor, BlogImageProcessor>();
builder.Services.AddSingleton<IProductImageProcessor, ProductImageProcessor>();
builder.Services.AddScoped<IBlogImageMigrationService, BlogImageMigrationService>();
builder.Services.AddHostedService<BlogImageMigrationHostedService>();

var app = builder.Build();

if (args.Any(arg => string.Equals(arg, "--migrate-blog-images", StringComparison.OrdinalIgnoreCase)))
{
    using var scope = app.Services.CreateScope();
    var migrator = scope.ServiceProvider.GetRequiredService<IBlogImageMigrationService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("BlogImageMigrationCli");
    var result = await migrator.RunAsync();
    logger.LogInformation("Manual blog image migration completed: scanned={Scanned}, processed={Processed}, skipped={Skipped}", result.Scanned, result.Processed, result.Skipped);
    return;
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseOutputCache();
app.UseResponseCompression();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var ext = Path.GetExtension(ctx.File.Name);
        var headers = ctx.Context.Response.Headers;

        if (string.Equals(ext, ".css", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ext, ".js", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ext, ".webp", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ext, ".jpg", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ext, ".jpeg", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ext, ".png", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ext, ".svg", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ext, ".woff", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ext, ".woff2", StringComparison.OrdinalIgnoreCase))
        {
            headers.CacheControl = "public,max-age=2592000";
        }
    }
});
app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

var staticAssetsManifestPath = Path.Combine(app.Environment.ContentRootPath, "bin", "Debug", "net10.0", "WebThuMuaPheLieu.staticwebassets.endpoints.json");
var hasStaticAssetsManifest = File.Exists(staticAssetsManifestPath);

if (hasStaticAssetsManifest)
{
    app.MapStaticAssets();
}

app.MapControllerRoute(
    name: "product-detail",
    pattern: "Home/Detail/{slug?}/{id:int?}",
    defaults: new { controller = "Home", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
