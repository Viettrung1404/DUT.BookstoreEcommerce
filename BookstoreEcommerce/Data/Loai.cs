using System;
using System.Collections.Generic;

namespace BookstoreEcommerce.Data;

public partial class Loai
{
    public int MaLoai { get; set; }

    public string TenLoai { get; set; } = null!;

    public string? TenLoaiAlias { get; set; }

    public string? MoTa { get; set; }

    public string? Hinh { get; set; }

    public virtual ICollection<Sach> Saches { get; set; } = new List<Sach>();
}
