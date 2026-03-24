using System;
using System.Collections.Generic;

namespace WebThuMuaPheLieu.Models;

public partial class SiteSetting
{
    public int Id { get; set; }

    public string? SettingKey { get; set; }

    public string? SettingValue { get; set; }

    public string? SettingGroup { get; set; }

    public string? Description { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
