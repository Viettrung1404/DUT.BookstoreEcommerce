using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookstoreEcommerce.Data;
using BookstoreEcommerce.Models;
using Microsoft.AspNetCore.Authorization;
using BookstoreEcommerce.Areas.Admin.ViewModels;
using BookstoreEcommerce.Helpers;

namespace BookstoreEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class KhachHangController : Controller
    {
        private readonly BookEcommerceContext _context;

        public KhachHangController(BookEcommerceContext context)
        {
            _context = context;
        }

        // GET: Admin/KhachHang
        public async Task<IActionResult> Index()
        {
            var db = await _context.KhachHangs.ToListAsync();
            List<KhachHangViewModel> model = new List<KhachHangViewModel>();
            foreach (var item in db)
            {
                model.Add(new KhachHangViewModel
                {
                    MaKh = item.MaKh,
                    MatKhau = item.MatKhau,
                    HoTen = item.HoTen,
                    GioiTinh = item.GioiTinh,
                    NgaySinh = item.NgaySinh,
                    DiaChi = item.DiaChi,
                    DienThoai = item.DienThoai,
                    Email = item.Email,
                    Hinh = item.Hinh,
                    HieuLuc = item.HieuLuc,
                    VaiTro = item.VaiTro,
                    RandomKey = item.RandomKey
                });
            }
            return View(model);
        }

        // GET: Admin/KhachHang/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(new KhachHangViewModel
            {
                MaKh = khachHang.MaKh,
                MatKhau = khachHang.MatKhau,
                HoTen = khachHang.HoTen,
                GioiTinh = khachHang.GioiTinh,
                NgaySinh = khachHang.NgaySinh,
                DiaChi = khachHang.DiaChi,
                DienThoai = khachHang.DienThoai,
                Email = khachHang.Email,
                Hinh = khachHang.Hinh,
                HieuLuc = khachHang.HieuLuc,
                VaiTro = khachHang.VaiTro,
                RandomKey = khachHang.RandomKey
            });
        }

        // GET: Admin/KhachHang/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/KhachHang/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHangViewModel khachHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(new KhachHang
                {
                    MaKh = khachHang.MaKh,
                    MatKhau = khachHang.MatKhau,
                    HoTen = khachHang.HoTen,
                    GioiTinh = khachHang.GioiTinh,
                    NgaySinh = khachHang.NgaySinh,
                    DiaChi = khachHang.DiaChi,
                    DienThoai = khachHang.DienThoai,
                    Email = khachHang.Email,
                    Hinh = khachHang.Hinh,
                    HieuLuc = khachHang.HieuLuc,
                    VaiTro = khachHang.VaiTro,
                    RandomKey = khachHang.RandomKey
                });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(khachHang);
        }

        // GET: Admin/KhachHang/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return View(new KhachHangViewModel
            {
                MaKh = khachHang.MaKh,
                MatKhau = khachHang.MatKhau,
                HoTen = khachHang.HoTen,
                GioiTinh = khachHang.GioiTinh,
                NgaySinh = khachHang.NgaySinh,
                DiaChi = khachHang.DiaChi,
                DienThoai = khachHang.DienThoai,
                Email = khachHang.Email,
                Hinh = khachHang.Hinh,
                HieuLuc = khachHang.HieuLuc,
                VaiTro = khachHang.VaiTro,
                RandomKey = khachHang.RandomKey
            });
        }

        // POST: Admin/KhachHang/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaKh,MatKhau,HoTen,GioiTinh,NgaySinh,DiaChi,DienThoai,Email,Hinh,HieuLuc,VaiTro,RandomKey")] KhachHang khachHang)
        {
            if (id != khachHang.MaKh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingKhachHang = await _context.KhachHangs.FindAsync(id);
                if (existingKhachHang == null)
                {
                    return NotFound();
                }
                existingKhachHang.MatKhau = DataEncryptionExtensions.ToMd5Hash(khachHang.MatKhau, khachHang.RandomKey);
                existingKhachHang.HoTen = khachHang.HoTen;
                existingKhachHang.GioiTinh = khachHang.GioiTinh;
                existingKhachHang.NgaySinh = khachHang.NgaySinh;
                existingKhachHang.DiaChi = khachHang.DiaChi;
                existingKhachHang.DienThoai = khachHang.DienThoai;
                existingKhachHang.Email = khachHang.Email;
                existingKhachHang.Hinh = khachHang.Hinh;
                existingKhachHang.HieuLuc = khachHang.HieuLuc;
                existingKhachHang.VaiTro = khachHang.VaiTro;
                existingKhachHang.RandomKey = khachHang.RandomKey;
                existingKhachHang.MaKh = khachHang.MaKh;

                try
                {
                    _context.Update(existingKhachHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangExists(khachHang.MaKh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(khachHang);
        }

        // GET: Admin/KhachHang/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(new KhachHangViewModel
            {
                MaKh = khachHang.MaKh,
                MatKhau = khachHang.MatKhau,
                HoTen = khachHang.HoTen,
                GioiTinh = khachHang.GioiTinh,
                NgaySinh = khachHang.NgaySinh,
                DiaChi = khachHang.DiaChi,
                DienThoai = khachHang.DienThoai,
                Email = khachHang.Email,
                Hinh = khachHang.Hinh,
                HieuLuc = khachHang.HieuLuc,
                VaiTro = khachHang.VaiTro,
                RandomKey = khachHang.RandomKey
            });
        }

        // POST: Admin/KhachHang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang != null)
            {
                _context.KhachHangs.Remove(khachHang);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KhachHangExists(string id)
        {
            return _context.KhachHangs.Any(e => e.MaKh == id);
        }
    }
}
