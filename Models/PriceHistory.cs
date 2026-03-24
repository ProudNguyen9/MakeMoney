using System;
using System.Collections.Generic;

namespace WebThuMuaPheLieu.Models;

public partial class PriceHistory
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public decimal? PriceValue { get; set; }

    public string? PriceUnit { get; set; }

    public string? PriceType { get; set; }

    public string? Note { get; set; }

    public DateOnly? EffectiveDate { get; set; }

    public DateTime? RecordedAt { get; set; }

    public virtual Product? Product { get; set; }
}
