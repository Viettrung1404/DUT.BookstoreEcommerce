namespace BookstoreEcommerce.ViewModels
{
    public class SachViewModel
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string? Hinh { get; set; }
        public double? DonGia { get; set; }
        public string? MoTa { get; set; }
        public string TenLoai { get; set; }
    }

    public class ChiTietSachViewModel
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string? Hinh { get; set; }
        public double? DonGia { get; set; }
        public string? MoTaDonVi { get; set; }
        public string TenLoai { get; set; }
        public string ChiTiet { get; set; }
        public int DiemDanhGia { get; set; }
        public int SoLuong { get; set; }
    }
}
