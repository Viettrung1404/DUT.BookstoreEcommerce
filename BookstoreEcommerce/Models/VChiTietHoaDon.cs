using System;
using System.Collections.Generic;

namespace BookstoreEcommerce.Models;

public partial class VChiTietHoaDon
{
    public int MaCt { get; set; }

    public int MaHd { get; set; }

    public int MaSach { get; set; }

    public double DonGia { get; set; }

    public int SoLuong { get; set; }

    public double GiamGia { get; set; }

    public string TenSach { get; set; } = null!;
}
