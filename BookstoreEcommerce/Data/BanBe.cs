using System;
using System.Collections.Generic;

namespace BookstoreEcommerce.Data;

public partial class BanBe
{
    public int MaBb { get; set; }

    public string? MaKh { get; set; }

    public int MaSach { get; set; }

    public string? HoTen { get; set; }

    public string Email { get; set; } = null!;

    public DateTime NgayGui { get; set; }

    public string? GhiChu { get; set; }

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual Sach MaSachNavigation { get; set; } = null!;
}
