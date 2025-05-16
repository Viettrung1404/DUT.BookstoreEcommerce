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
using System.Runtime.Intrinsics.Arm;

namespace BookstoreEcommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SachController : Controller
    {
        private readonly BookEcommerceContext _context;

        public SachController(BookEcommerceContext context)
        {
            _context = context;
        }

        // GET: Admin/Sach
        public async Task<IActionResult> Index()
        {
            var bookEcommerceContext = _context.Saches.Include(s => s.MaLoaiNavigation).Include(s => s.MaNccNavigation).Where(s => s.Status == "Available");
            List<SachViewModel> model = new List<SachViewModel>();
            foreach (var item in await bookEcommerceContext.ToListAsync())
            {
                model.Add(new SachViewModel
                {
                    MaSach = item.MaSach,
                    TenSach = item.TenSach,
                    TenAlias = item.TenAlias,
                    TacGia = item.TacGia,
                    MaLoai = item.MaLoai,
                    TenLoai = item.MaLoaiNavigation.TenLoai,
                    MoTaDonVi = item.MoTaDonVi,
                    DonGia = item.DonGia,
                    Hinh = item.Hinh,
                    NgayXb = item.NgayXb,
                    GiamGia = item.GiamGia,
                    SoLanXem = item.SoLanXem,
                    MoTa = item.MoTa,
                    MaNcc = item.MaNcc,
                    TenNcc = item.MaNccNavigation.TenCongTy
                });
            }
            return View();
        }

        // GET: Admin/Sach/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sach = await _context.Saches
                .Include(s => s.MaLoaiNavigation)
                .Include(s => s.MaNccNavigation)
                .FirstOrDefaultAsync(m => m.MaSach == id && m.Status == "Available");
            if (sach == null)
            {
                return NotFound();
            }

            return View(new SachViewModel
            {
                MaSach = sach.MaSach,
                TenSach = sach.TenSach,
                TenAlias = sach.TenAlias,
                TacGia = sach.TacGia,
                MaLoai = sach.MaLoai,
                TenLoai = sach.MaLoaiNavigation.TenLoai,
                MoTaDonVi = sach.MoTaDonVi,
                DonGia = sach.DonGia,
                Hinh = sach.Hinh,
                NgayXb = sach.NgayXb,
                GiamGia = sach.GiamGia,
                SoLanXem = sach.SoLanXem,
                MoTa = sach.MoTa,
                MaNcc = sach.MaNcc,
                TenNcc = sach.MaNccNavigation.TenCongTy
            });
        }

        // GET: Admin/Sach/Create
        public IActionResult Create()
        {
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai");
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc");
            return View();
        }

        // POST: Admin/Sach/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SachViewModel sach, IFormFile Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Hinh != null)
                    {
                        var fileName = MyUtil.UploadHinh(Hinh, "KhachHang");
                        if (fileName != string.Empty)
                        {
                            sach.Hinh = fileName;
                        }
                    }
                    _context.Add(new SachViewModel
                    {
                        TenSach = sach.TenSach,
                        TenAlias = sach.TenAlias,
                        TacGia = sach.TacGia,
                        MaLoai = sach.MaLoai,
                        MoTaDonVi = sach.MoTaDonVi,
                        DonGia = sach.DonGia,
                        Hinh = sach.Hinh,
                        NgayXb = sach.NgayXb,
                        GiamGia = sach.GiamGia,
                        SoLanXem = sach.SoLanXem,
                        MoTa = sach.MoTa,
                        MaNcc = sach.MaNcc
                    });
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                }
            }
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai", sach.MaLoai);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc", sach.MaNcc);
            return View(sach);
        }

        // GET: Admin/Sach/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sach = await _context.Saches
                .Include(s => s.MaLoaiNavigation)
                .Include(s => s.MaNccNavigation)
                .FirstOrDefaultAsync(s => s.MaSach == id && s.Status == "Available");

            if (sach == null)
            {
                return NotFound();
            }
            
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai", sach.MaLoai);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc", sach.MaNcc);
            return View(new SachViewModel
            {
                MaSach = sach.MaSach,
                TenSach = sach.TenSach,
                TenAlias = sach.TenAlias,
                TacGia = sach.TacGia,
                MaLoai = sach.MaLoai,
                TenLoai = sach.MaLoaiNavigation.TenLoai,
                MoTaDonVi = sach.MoTaDonVi,
                DonGia = sach.DonGia,
                Hinh = sach.Hinh,
                NgayXb = sach.NgayXb,
                GiamGia = sach.GiamGia,
                SoLanXem = sach.SoLanXem,
                MoTa = sach.MoTa,
                MaNcc = sach.MaNcc,
                TenNcc = sach.MaNccNavigation.TenCongTy
            });
        }

        // POST: Admin/Sach/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SachViewModel sach, IFormFile Hinh)
        {
            if (id != sach.MaSach)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (Hinh != null)
                    {
                        var fileName = MyUtil.UploadHinh(Hinh, "KhachHang");
                        if (fileName != string.Empty)
                        {
                            sach.Hinh = fileName;
                        }
                    }
                    Sach temp = await _context.Saches
                        .Include(s => s.MaLoaiNavigation)
                        .Include(s => s.MaNccNavigation)
                        .FirstOrDefaultAsync(s => s.MaSach == id && s.Status == "Available");
                    if (temp == null)
                    {
                        return NotFound();
                    }
                    temp.TenSach = sach.TenSach;
                    temp.TenAlias = sach.TenAlias;
                    temp.TacGia = sach.TacGia;
                    temp.MaLoai = sach.MaLoai;
                    temp.MoTaDonVi = sach.MoTaDonVi;
                    temp.DonGia = sach.DonGia;
                    temp.Hinh = sach.Hinh;
                    temp.NgayXb = sach.NgayXb;
                    temp.GiamGia = sach.GiamGia;
                    temp.SoLanXem = sach.SoLanXem;
                    temp.MoTa = sach.MoTa;
                    temp.MaNcc = sach.MaNcc;
                    _context.Update(temp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SachExists(sach.MaSach))
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
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai", sach.MaLoai);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc", sach.MaNcc);
            return View(sach);
        }

        // GET: Admin/Sach/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sach = await _context.Saches
                .Include(s => s.MaLoaiNavigation)
                .Include(s => s.MaNccNavigation)
                .FirstOrDefaultAsync(m => m.MaSach == id && m.Status == "Available");
            if (sach == null)
            {
                return NotFound();
            }

            return View(new SachViewModel
            {
                MaSach = sach.MaSach,
                TenSach = sach.TenSach,
                TenAlias = sach.TenAlias,
                TacGia = sach.TacGia,
                MaLoai = sach.MaLoai,
                TenLoai = sach.MaLoaiNavigation.TenLoai,
                MoTaDonVi = sach.MoTaDonVi,
                DonGia = sach.DonGia,
                Hinh = sach.Hinh,
                NgayXb = sach.NgayXb,
                GiamGia = sach.GiamGia,
                SoLanXem = sach.SoLanXem,
                MoTa = sach.MoTa,
                MaNcc = sach.MaNcc,
                TenNcc = sach.MaNccNavigation.TenCongTy
            });
        }

        // POST: Admin/Sach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sach = await _context.Saches.FindAsync(id);
            if (sach != null)
            {
                sach.Status = "OutOfStock";
                _context.Saches.Update(sach);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SachExists(int id)
        {
            return _context.Saches.Any(e => e.MaSach == id);
        }
    }
}
