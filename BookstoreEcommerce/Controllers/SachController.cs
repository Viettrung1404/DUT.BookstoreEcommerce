using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookstoreEcommerce.Data;
using BookstoreEcommerce.ViewModels;

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

        // GET: Sach/Create
        public IActionResult Create()
        {
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai");
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc");
            return View();
        }

        // POST: Sach/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSach,TenSach,TenAlias,TacGia,MaLoai,MoTaDonVi,DonGia,Hinh,NgayXb,GiamGia,SoLanXem,MoTa,MaNcc")] Sach sach)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sach);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai", sach.MaLoai);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc", sach.MaNcc);
            return View(sach);
        }

        // GET: Sach/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sach = await _context.Saches.FindAsync(id);
            if (sach == null)
            {
                return NotFound();
            }
            ViewData["MaLoai"] = new SelectList(_context.Loais, "MaLoai", "MaLoai", sach.MaLoai);
            ViewData["MaNcc"] = new SelectList(_context.NhaCungCaps, "MaNcc", "MaNcc", sach.MaNcc);
            return View(sach);
        }

        // POST: Sach/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSach,TenSach,TenAlias,TacGia,MaLoai,MoTaDonVi,DonGia,Hinh,NgayXb,GiamGia,SoLanXem,MoTa,MaNcc")] Sach sach)
        {
            if (id != sach.MaSach)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sach);
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

        // GET: Sach/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sach = await _context.Saches
                .Include(s => s.MaLoaiNavigation)
                .Include(s => s.MaNccNavigation)
                .FirstOrDefaultAsync(m => m.MaSach == id);
            if (sach == null)
            {
                return NotFound();
            }

            return View(sach);
        }

        // POST: Sach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sach = await _context.Saches.FindAsync(id);
            if (sach != null)
            {
                _context.Saches.Remove(sach);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SachExists(int id)
        {
            return _context.Saches.Any(e => e.MaSach == id);
        }
    }
}
