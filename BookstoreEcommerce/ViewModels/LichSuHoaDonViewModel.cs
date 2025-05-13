namespace BookstoreEcommerce.ViewModels
{
    public class LichSuHoaDonViewModel
    {
        public int MaHd { get; set; }
        public DateTime NgayDat { get; set; }
        public string CachThanhToan { get; set; }
        public string CachVanChuyen { get; set; }
        public string TenTrangThai { get; set; }
        public List<ChiTietHdViewModel> ChiTietHds { get; set; } = new();
    }

    public class ChiTietHdViewModel
    {
        public int MaSach { get; set; }
        public int SoLuong { get; set; }
        public double DonGia { get; set; }
        public string TenSach { get; set; }
        public string Hinh { get; set; }
    }
}
