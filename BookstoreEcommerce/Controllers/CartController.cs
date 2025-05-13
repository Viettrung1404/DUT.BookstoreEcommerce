using BookstoreEcommerce.Data;
using BookstoreEcommerce.Helpers;
using BookstoreEcommerce.Models;
using BookstoreEcommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace BookstoreEcommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly BookEcommerceContext? db;
        private readonly PaypalClient _paypalClient;
        public List<CartViewModel> Cart => HttpContext.Session.Get<List<CartViewModel>>(MySetting.CART_KEY) ?? new List<CartViewModel>();

        public CartController(BookEcommerceContext context, PaypalClient paypalClient)
        {
            db = context;
            _paypalClient = paypalClient;
        }

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            if (db == null)
            {
                TempData["Error"] = "Database context is not initialized.";
                return Redirect("/500");
            }

            var gioHang = Cart;
            var item = gioHang.FirstOrDefault(x => x.MaSach == id);
            if (item == null)
            {
                var sach = db.Saches?.FirstOrDefault(x => x.MaSach == id);
                if (sach != null)
                {
                    gioHang.Add(new CartViewModel
                    {
                        MaSach = sach.MaSach,
                        TenSach = sach.TenSach,
                        Hinh = sach.Hinh ?? string.Empty,
                        DonGia = sach.DonGia ?? 0,
                        SoLuong = quantity
                    });
                }
                else
                {
                    TempData["Error"] = $"Không thể thêm vào giỏ do sách có mã {id} không tồn tại!";
                    return Redirect("/404");
                }
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Details", "Sach", new { id = id });
        }

        public IActionResult RemoveCart(int id)
        {
            if (db == null)
            {
                TempData["Error"] = "Database context is not initialized.";
                return Redirect("/500");
            }

            var gioHang = Cart;
            var item = gioHang.FirstOrDefault(x => x.MaSach == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Cart.Count == 0)
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }
            ViewBag.PaypalClientdId = _paypalClient.ClientId;
            return View(Cart);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutViewModel model, string payment = "COD")
        {
            if (ModelState.IsValid)
            {
                var customerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
                if (customerId == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin khách hàng!";
                    return RedirectToAction("Index");
                }
                var khachHang = new KhachHang();
                if (model.GiongKhachHang)
                {
                    khachHang = db?.KhachHangs?.FirstOrDefault(x => x.MaKh == customerId);
                }
                var hoaDon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model.HoTen ?? khachHang.HoTen,
                    DiaChi = model.DiaChi ?? khachHang?.DiaChi,
                    DienThoai = model.DienThoai ?? khachHang.DienThoai,
                    GhiChu = model.GhiChu,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "COD",
                    CachVanChuyen = "GHN",
                    MaTrangThai = 0
                };

                db.Database.BeginTransaction();
                try
                {
                    db.Add(hoaDon);
                    db.SaveChanges();
                    var cthd = new List<ChiTietHd>();
                    foreach (var item in Cart)
                    {
                        cthd.Add(new ChiTietHd
                        {
                            MaHd = hoaDon.MaHd,
                            MaSach = item.MaSach,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            GiamGia = 0,
                        });
                    }
                    db.AddRange(cthd);
                    db.SaveChanges();   
                    db.Database.CommitTransaction();
                    HttpContext.Session.Set(MySetting.CART_KEY, new List<CartViewModel>());
                    TempData["Success"] = "Đặt hàng thành công!";
                    return View("Success");
                }
                catch (Exception ex)
                {
                    db.Database.RollbackTransaction();
                    TempData["Error"] = $"Đã xảy ra lỗi: {ex.Message}";
                }
            }

            return View(Cart);
        }

        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View("Success");
        }

        #region Paypal payment
        [Authorize]
        [HttpPost]
        public IActionResult SaveCheckoutInfo([FromBody] CheckoutViewModel model)
        {
            HttpContext.Session.Set("CHECKOUT_INFO", model);
            return Ok();
        }


        [Authorize]
        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            // Thông tin đơn hàng gửi qua Paypal
            var tongTien = (Cart.Sum(p => p.TongTien)/26000).ToString("F2");
            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);

                // Lưu database đơn hàng của mình
                var model = HttpContext.Session.Get<CheckoutViewModel>("CHECKOUT_INFO");

                var customerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
                var khachHang = db?.KhachHangs?.FirstOrDefault(x => x.MaKh == customerId);

                var hoaDon = new HoaDon
                {
                    MaKh = customerId,
                    HoTen = model?.HoTen ?? khachHang?.HoTen,
                    DiaChi = model?.DiaChi ?? khachHang?.DiaChi,
                    DienThoai = model?.DienThoai ?? khachHang?.DienThoai,
                    GhiChu = model?.GhiChu,
                    NgayDat = DateTime.Now,
                    CachThanhToan = "PayPal",
                    CachVanChuyen = "GHN",
                    MaTrangThai = 1
                };

                db.HoaDons.Add(hoaDon);
                db.SaveChanges();

                var cthd = new List<ChiTietHd>();
                foreach (var item in Cart)
                {
                    cthd.Add(new ChiTietHd
                    {
                        MaHd = hoaDon.MaHd,
                        MaSach = item.MaSach,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia,
                        GiamGia = 0,
                    });
                }
                db.ChiTietHds.AddRange(cthd);
                db.SaveChanges();

                // Xóa giỏ hàng và thông tin checkout khỏi Session
                HttpContext.Session.Set(MySetting.CART_KEY, new List<CartViewModel>());
                HttpContext.Session.Remove("CHECKOUT_INFO");

                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        #endregion

        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }

        [Authorize]
        [Authorize]
        public IActionResult LichSu()
        {
            var customerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
            if (customerId == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng!";
                return RedirectToAction("Index");
            }

            var lichSu = db.HoaDons
                .Include(hd => hd.MaTrangThaiNavigation)
                .Include(hd => hd.ChiTietHds)
                    .ThenInclude(ct => ct.MaSachNavigation)
                .Where(hd => hd.MaKh == customerId)
                .OrderByDescending(hd => hd.NgayDat)
                .Select(hd => new LichSuHoaDonViewModel
                {
                    MaHd = hd.MaHd,
                    NgayDat = hd.NgayDat,
                    CachThanhToan = hd.CachThanhToan,
                    CachVanChuyen = hd.CachVanChuyen,
                    TenTrangThai = hd.MaTrangThaiNavigation.TenTrangThai,
                    ChiTietHds = hd.ChiTietHds.Select(ct => new ChiTietHdViewModel
                    {
                        MaSach = ct.MaSach,
                        SoLuong = ct.SoLuong,
                        DonGia = ct.DonGia,
                        TenSach = ct.MaSachNavigation.TenSach,
                        Hinh = ct.MaSachNavigation.Hinh
                    }).ToList()
                })
                .ToList();

            return View(lichSu);
        }
        [Authorize]
        [HttpPost]
        public IActionResult HuyDon(int id)
        {
            var customerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == MySetting.CLAIM_CUSTOMERID)?.Value;
            if (customerId == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin khách hàng!";
                return RedirectToAction("LichSu");
            }

            var hoaDon = db.HoaDons.FirstOrDefault(hd => hd.MaHd == id && hd.MaKh == customerId && hd.MaTrangThai == 0);
            if (hoaDon == null)
            {
                TempData["Error"] = "Không thể hủy đơn này!";
                return RedirectToAction("LichSu");
            }

            hoaDon.MaTrangThai = -1;
            db.SaveChanges();

            TempData["Success"] = "Đã hủy đơn hàng thành công!";
            return RedirectToAction("LichSu");
        }
    }
}
