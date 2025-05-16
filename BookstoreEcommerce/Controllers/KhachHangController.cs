using AutoMapper;
using BookstoreEcommerce.Data;
using Microsoft.AspNetCore.Mvc;
using BookstoreEcommerce.ViewModels;
using BookstoreEcommerce.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using BookstoreEcommerce.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BookstoreEcommerce.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly BookEcommerceContext db;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        public KhachHangController(BookEcommerceContext context, IMapper mapper, IEmailSender emailSender)
        {
            db = context;
            _mapper = mapper;
            _emailSender = emailSender;
        }

        #region Register

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DangKy(RegisterViewModel model, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.RandomKey = MyUtil.GenerateRamdomKey();
                    khachHang.MatKhau = model.MatKhau.ToMd5Hash(khachHang.RandomKey);
                    khachHang.HieuLuc = true;
                    khachHang.VaiTro = 0;
                    if (Hinh != null)
                    {
                        var fileName = MyUtil.UploadHinh(Hinh, "KhachHang");
                        if (fileName != string.Empty)
                        {
                            khachHang.Hinh = fileName;
                        }
                    }
                    else
                    {
                    }
                    db.KhachHangs.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("DangNHap", "KhachHang");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                }
            }
            return View(model);
        }

        #endregion

        #region Login

        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginViewModel model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.FirstOrDefault(x => x.MaKh == model.UserName);
                if (khachHang == null)
                {
                    ModelState.AddModelError("loi", "Tên đăng nhập hoặc mật khẩu không đúng");
                }
                else
                {
                    if (khachHang.HieuLuc == false)
                    {
                        ModelState.AddModelError("loi", "Tài khoản đã bị khóa");
                    }
                    else if (khachHang.MatKhau != model.Password.ToMd5Hash(khachHang.RandomKey))
                    {
                        ModelState.AddModelError("loi", "Tên đăng nhập hoặc mật khẩu không đúng");
                    }
                    else
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, khachHang.MaKh),
                            new Claim(ClaimTypes.Email, khachHang.Email),
                            new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.MaKh),
                            new Claim(ClaimTypes.Role, khachHang.VaiTro == 0? "Customer" : "Admin"),
                            new Claim(ClaimTypes.Surname, khachHang.HoTen),
                            new Claim("Hinh", khachHang.Hinh ?? "default.png"),
                            new Claim("GioiTinh", khachHang.GioiTinh == true ? "Nam" : "Nữ"),
                            new Claim("DienThoai", khachHang.DienThoai ?? ""),
                            new Claim("DiaChi", khachHang.DiaChi ?? ""),
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, "login");
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(claimsPrincipal);

                        if (Url.IsLocalUrl(ReturnUrl))
                            return Redirect(ReturnUrl);
                        if (khachHang.VaiTro == 1)
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }
        #endregion

        public IActionResult DangXuat()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult ThongTin()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DoiMatKhau(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var maKh = User.FindFirst(ClaimTypes.Name)?.Value;
            var kh = db.KhachHangs.FirstOrDefault(x => x.MaKh == maKh);

            if (kh == null)
            {
                ModelState.AddModelError("", "Không tìm thấy tài khoản.");
                return View("ThongTin");
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                return View("ThongTin");
            }

            if (kh.MatKhau != CurrentPassword.ToMd5Hash(kh.RandomKey))
            {
                ModelState.AddModelError("", "Mật khẩu hiện tại không đúng.");
                return View("ThongTin");
            }

            // Tạo và lưu thông tin vào Session
            var maXacThuc = MyUtil.GenerateRamdomKey(6);
            var xacThucHetHan = DateTime.Now.AddMinutes(10);

            HttpContext.Session.SetString("VerificationCode", maXacThuc);
            HttpContext.Session.SetString("NewPasswordHash", NewPassword.ToMd5Hash(kh.RandomKey));
            HttpContext.Session.SetString("Expiration", xacThucHetHan.ToString("o")); // Lưu dưới dạng ISO 8601

            // Gửi email
            var emailSender = new EmailSender();
            await emailSender.SendEmailAsync(kh.Email, "Xác thực đổi mật khẩu",
                $"Mã xác thực đổi mật khẩu của bạn là: <b>{maXacThuc}</b>");

            TempData["ThongBao"] = "Đã gửi mã xác thực về email. Vui lòng nhập mã xác thực để hoàn tất đổi mật khẩu.";
            return RedirectToAction("NhapMaXacThuc");
        }

        [Authorize]
        public IActionResult NhapMaXacThuc()
        {
            return View();
        }


        [Authorize]
        [HttpPost]
        public IActionResult XacThucDoiMatKhau(string MaXacThuc)
        {
            var maKh = User.FindFirst(ClaimTypes.Name)?.Value;
            var kh = db.KhachHangs.FirstOrDefault(x => x.MaKh == maKh);

            if (kh == null)
            {
                TempData["Loi"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("NhapMaXacThuc");
            }

            // Lấy thông tin từ Session
            var maXacThucSession = HttpContext.Session.GetString("VerificationCode");
            var hashedNewPassword = HttpContext.Session.GetString("NewPasswordHash");
            var expirationStr = HttpContext.Session.GetString("Expiration");

            if (string.IsNullOrEmpty(maXacThucSession) ||
                string.IsNullOrEmpty(hashedNewPassword) ||
                string.IsNullOrEmpty(expirationStr))
            {
                TempData["Loi"] = "Phiên xác thực không hợp lệ.";
                return RedirectToAction("NhapMaXacThuc");
            }

            if (!DateTime.TryParse(expirationStr, out var expiration) ||
                MaXacThuc != maXacThucSession ||
                expiration < DateTime.Now)
            {
                TempData["Loi"] = "Mã xác thực không đúng hoặc đã hết hạn.";
                return RedirectToAction("NhapMaXacThuc");
            }

            // Cập nhật mật khẩu mới
            kh.MatKhau = hashedNewPassword;
            db.SaveChanges();

            // Xóa dữ liệu session sau khi xác thực thành công
            HttpContext.Session.Remove("VerificationCode");
            HttpContext.Session.Remove("NewPasswordHash");
            HttpContext.Session.Remove("Expiration");

            TempData["ThongBao"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("ThongTin");
        }
    }
}
