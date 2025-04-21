using Microsoft.AspNetCore.Mvc;
using BookstoreEcommerce.ViewModels;
using BookstoreEcommerce.Data;
namespace BookstoreEcommerce.ViewComponents
{
    public class MenuLoaiViewComponent: ViewComponent
    {
        private readonly BookEcommerceContext db;
        public MenuLoaiViewComponent(BookEcommerceContext context)
        {
            db = context;
        }
        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(loai => new MenuLoaiViewModel()
            {
                MaLoai = loai.MaLoai,
                TenLoai = loai.TenLoai,
                SoLuong = db.Saches.Count(s => s.MaLoai == loai.MaLoai)
            }).OrderBy(p => p.TenLoai).ToList();
            return View(data);
        }
    }
}
