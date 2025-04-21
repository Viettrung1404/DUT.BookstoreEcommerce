namespace BookstoreEcommerce.ViewModels
{
    public class CartViewModel
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public double TongTien => DonGia * SoLuong;

    }
}
