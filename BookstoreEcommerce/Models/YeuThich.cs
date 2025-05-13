using System;
using System.Collections.Generic;

namespace BookstoreEcommerce.Models;

public partial class YeuThich
{
    public int MaYt { get; set; }

    public int? MaSach { get; set; }

    public string? MaKh { get; set; }

    public DateTime? NgayChon { get; set; }

    public string? MoTa { get; set; }

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual Sach? MaSachNavigation { get; set; }
}
