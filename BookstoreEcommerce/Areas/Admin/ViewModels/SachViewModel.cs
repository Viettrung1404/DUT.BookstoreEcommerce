namespace BookstoreEcommerce.Areas.Admin.ViewModels
{
    public class SachViewModel
    {
        public int MaSach { get; set; }

        public string TenSach { get; set; } = null!;

        public string? TenAlias { get; set; }

        public string TacGia { get; set; } = null!;

        public int MaLoai { get; set; }
        public string TenLoai { get; set; }

        public string? MoTaDonVi { get; set; }

        public double? DonGia { get; set; }

        public string? Hinh { get; set; }

        public DateTime NgayXb { get; set; }

        public double GiamGia { get; set; }

        public int SoLanXem { get; set; }

        public string? MoTa { get; set; }

        public string MaNcc { get; set; } = null!;
        public string TenNcc { get; set; } = null!;
    }
}
