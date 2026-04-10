using System.Collections.Generic;
using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.ViewModels
{
    public class RecentNewsSectionComponentViewModel
    {
        public List<BlogCardViewModel> Posts { get; set; } = new();
    }
}
