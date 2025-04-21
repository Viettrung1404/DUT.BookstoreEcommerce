using BookstoreEcommerce.Helpers;
using BookstoreEcommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreEcommerce.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartViewModel>>(MySetting.CART_KEY) ?? new List<CartViewModel>();
            return View(new CartNavViewModel
            {
                TongSach = cart.Sum(x => x.SoLuong),
                TongTien = cart.Sum(x => x.TongTien)
            });
        }
    }
}
