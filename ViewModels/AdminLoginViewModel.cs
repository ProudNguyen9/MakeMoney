using System.ComponentModel.DataAnnotations;

namespace WebThuMuaPheLieu.ViewModels;

public class AdminLoginViewModel
{
    [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
