using Microsoft.AspNetCore.Mvc;

namespace WebThuMuaPheLieu.Controllers;

[Route("admin")]
public class AdminController : Controller
{
    [HttpGet("")]
    [HttpGet("overview")]
    public IActionResult Overview()
    {
        ViewData["Title"] = "Tổng quan";
        ViewData["AdminSection"] = "Overview";
        return View();
    }

    [HttpGet("products")]
    public IActionResult Products()
    {
        ViewData["Title"] = "Sản phẩm";
        ViewData["AdminSection"] = "Products";
        return View();
    }

    [HttpGet("prices")]
    public IActionResult Prices()
    {
        ViewData["Title"] = "Bảng giá";
        ViewData["AdminSection"] = "Prices";
        return View();
    }

    [HttpGet("posts")]
    public IActionResult Posts()
    {
        ViewData["Title"] = "Bài viết";
        ViewData["AdminSection"] = "Posts";
        return View();
    }

    [HttpGet("marketing")]
    public IActionResult Marketing()
    {
        ViewData["Title"] = "Banner & SEO";
        ViewData["AdminSection"] = "Marketing";
        return View();
    }
}
