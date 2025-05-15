using System;
using System.ComponentModel.DataAnnotations;

namespace BookstoreEcommerce.Areas.Admin.ViewModels
{
    public class KhachHangViewModel
    {
        [Required(ErrorMessage = "Mã khách hàng không được để trống.")]
        public string MaKh { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự.")]
        public string? MatKhau { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
        public string HoTen { get; set; } = null!;

        [Display(Name = "Giới tính")]
        public bool GioiTinh { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ tối đa 200 ký tự.")]
        public string? DiaChi { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Display(Name = "Điện thoại")]
        public string? DienThoai { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Hình đại diện")]
        public string? Hinh { get; set; }

        [Display(Name = "Hiệu lực")]
        public bool HieuLuc { get; set; }

        [Display(Name = "Vai trò")]
        [Range(0, 1, ErrorMessage = "Vai trò chỉ nhận giá trị 0 hoặc 1.")]
        public int VaiTro { get; set; }

        public string? RandomKey { get; set; }
    }
}
