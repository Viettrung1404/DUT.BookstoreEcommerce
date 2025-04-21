using BookstoreEcommerce.Data;
using BookstoreEcommerce.Helpers;
using BookstoreEcommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreEcommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly BookEcommerceContext? db;
        const string CART_KEY = "MYCART";
        public List<CartViewModel> Cart => HttpContext.Session.Get<List<CartViewModel>>(CART_KEY) ?? new List<CartViewModel>();

        public CartController(BookEcommerceContext context)
        {
            db = context;
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

            HttpContext.Session.Set(CART_KEY, gioHang);
            return RedirectToAction("Index");
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
                HttpContext.Session.Set(CART_KEY, gioHang);
            }

            return RedirectToAction("Index");
        }
    }
}
