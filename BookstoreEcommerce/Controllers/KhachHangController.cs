using AutoMapper;
using BookstoreEcommerce.Data;
using Microsoft.AspNetCore.Mvc;
using BookstoreEcommerce.ViewModels;
using BookstoreEcommerce.Helpers;

namespace BookstoreEcommerce.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly BookEcommerceContext db;
        private readonly IMapper _mapper;
        public KhachHangController(BookEcommerceContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

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


                    db.KhachHangs.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                }
            }
            return View(model);
        }
    }
}
