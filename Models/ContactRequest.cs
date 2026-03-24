using System;
using System.Collections.Generic;

namespace WebThuMuaPheLieu.Models;

public partial class ContactRequest
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? RequestType { get; set; }

    public string? Area { get; set; }

    public string? Message { get; set; }

    public string? SourcePage { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
