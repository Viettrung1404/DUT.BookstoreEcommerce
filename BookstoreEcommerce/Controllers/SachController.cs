using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookstoreEcommerce.Data;
using BookstoreEcommerce.ViewModels;
using BookstoreEcommerce.Models;

namespace BookstoreEcommerce.Controllers
{
    public class SachController : Controller
    {
        private readonly BookEcommerceContext _context;

        public SachController(BookEcommerceContext context)
        {
            _context = context;
        }

        // GET: Sach
        public IActionResult Index(int? loai)
        {
            var saches = _context.Saches.AsQueryable();
            if (loai.HasValue)
            {
                saches = saches.Where(s => s.MaLoai == loai.Value);
            }

            var result = saches.Select(saches => new SachViewModel
            {
                MaSach = saches.MaSach,
                TenSach = saches.TenSach,
                Hinh = saches.Hinh,
                DonGia = saches.DonGia,
                MoTa = saches.MoTa,
                TenLoai = saches.MaLoaiNavigation.TenLoai
            }).ToList();
            return View(result);
        }

        // GET: Search
        public IActionResult Search(string? query)
        {
            var saches = _context.Saches.AsQueryable();
            if (query != null)
            {
                saches = saches.Where(s => s.TenSach.Contains(query));
            }

            var result = saches.Select(saches => new SachViewModel
            {
                MaSach = saches.MaSach,
                TenSach = saches.TenSach,
                Hinh = saches.Hinh,
                DonGia = saches.DonGia,
                MoTa = saches.MoTa,
                TenLoai = saches.MaLoaiNavigation.TenLoai
            }).ToList();
            return View(result);
        }

        // GET: Sach/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "Sai đường dẫn!";
                return Redirect("/404");
            }

            var sach = await _context.Saches
                .Include(s => s.MaLoaiNavigation)
                .Include(s => s.MaNccNavigation)
                .FirstOrDefaultAsync(m => m.MaSach == id);
            if (sach == null)
            {
                TempData["Error"] = $"Không tìm thấy sách có mã {id}!";
                return Redirect("/404");
            }

            var result = new ChiTietSachViewModel
            {
                MaSach = sach.MaSach,
                TenSach = sach.TenSach,
                Hinh = sach.Hinh,
                DonGia = sach.DonGia,
                ChiTiet = sach.MoTa ?? string.Empty,
                TenLoai = sach.MaLoaiNavigation.TenLoai,
                MoTaDonVi = sach.MoTaDonVi,
                // cập nhật sau
                DiemDanhGia = 5, 
                SoLuong = 10
            };

            return View(result);
        }
    }
}
