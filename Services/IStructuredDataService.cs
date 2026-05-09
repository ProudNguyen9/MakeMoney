using WebThuMuaPheLieu.Models;
using WebThuMuaPheLieu.ViewModels;

namespace WebThuMuaPheLieu.Services;

public interface IStructuredDataService
{
    string SerializeSchemas(IEnumerable<object> schemaNodes);
    IEnumerable<object> BuildHomeSchemas(HttpContext httpContext);
    IEnumerable<object> BuildBlogListSchemas(HttpContext httpContext);
    IEnumerable<object> BuildBlogDetailSchemas(HttpContext httpContext, WebThuMuaPheLieu.Models.BlogDetailViewModel model);
    IEnumerable<object> BuildProductListSchemas(HttpContext httpContext);
    IEnumerable<object> BuildProductDetailSchemas(HttpContext httpContext, ProductDetailViewModel model);
    IEnumerable<object> BuildAboutSchemas(HttpContext httpContext);
    IEnumerable<object> BuildContactSchemas(HttpContext httpContext, ContactPageViewModel model);
}

