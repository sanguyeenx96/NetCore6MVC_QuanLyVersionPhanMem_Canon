using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Quanlyversion.Models;
using System.Collections.Generic;

namespace Quanlyversion.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _context;
        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.parentPage = "Trang chủ";
            ViewBag.currentPage = "Tìm kiếm phần mềm";

            var tenModel = await _context.Models.Select(x => x.Name).Distinct().ToListAsync();
            var selectlist = new SelectList(tenModel, tenModel);
            ViewBag.sl = selectlist;

            var tenStation = await _context.Softwares.Select(x => x.CongDoan).Distinct().ToListAsync();
            var selectlistStation = new SelectList(tenStation, tenStation);
            ViewBag.slstation = selectlistStation;

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> SearchSoftware(string searchTerm, string models, string stations, string trangThai)
        {
            try
            {
                var modelArray = models.Split(',');
                var stationArray = stations.Split(',');
                var trangThaiBool = trangThai == "Đang áp dụng" ? true : trangThai == "Không áp dụng" ? false : (bool?)null;

                //1-1-1
                if (modelArray.Contains("Tất cả") && stationArray.Contains("Tất cả") && trangThaiBool == null)
                {              
                    var results = await _context.Softwares.Include(x=>x.Model)
                        .Where(s => (string.IsNullOrEmpty(searchTerm) || s.Name.Contains(searchTerm)))
                        .ToListAsync();
                    return PartialView("_ketquatimkiem", results);
                }
                //1-1-0
                if (modelArray.Contains("Tất cả") && stationArray.Contains("Tất cả") && trangThaiBool != null)
                {
                    var results = await _context.Softwares.Include(x => x.Model)
                        .Where(s => (string.IsNullOrEmpty(searchTerm) || s.Name.Contains(searchTerm)) &&
                                      s.TrangThaiApDung == trangThaiBool)
                        .ToListAsync();
                    return PartialView("_ketquatimkiem", results);
                }
                //1-0-0
                if (modelArray.Contains("Tất cả") && !stationArray.Contains("Tất cả") && trangThaiBool != null)
                {
                    var results = await _context.Softwares.Include(x => x.Model)
                        .Where(s => (string.IsNullOrEmpty(searchTerm) || s.Name.Contains(searchTerm)) &&
                                     (stationArray.Length == 0 || stationArray.Contains(s.CongDoan)) &&
                                      (s.TrangThaiApDung == trangThaiBool)
                                         ).ToListAsync();
                    return PartialView("_ketquatimkiem", results);
                }
                //1-0-1
                if (modelArray.Contains("Tất cả") && !stationArray.Contains("Tất cả") && trangThaiBool == null)
                {
                    var results = await _context.Softwares.Include(x => x.Model)
                        .Where(s => (string.IsNullOrEmpty(searchTerm) || s.Name.Contains(searchTerm)) &&
                                     (stationArray.Length == 0 || stationArray.Contains(s.CongDoan))
                                         ).ToListAsync();
                    return PartialView("_ketquatimkiem", results);
                }
                //0-0-1
                if (!modelArray.Contains("Tất cả") && !stationArray.Contains("Tất cả") && trangThaiBool == null)
                {
                    var results = await _context.Softwares.Include(x => x.Model)
                        .Where(s => (string.IsNullOrEmpty(searchTerm) || s.Name.Contains(searchTerm)) &&
                                     (modelArray.Length == 0 || modelArray.Contains(s.Model.Name)) &&
                                     (stationArray.Length == 0 || stationArray.Contains(s.CongDoan))
                                         ).ToListAsync();
                    return PartialView("_ketquatimkiem", results);
                }
                //0-0-0
                if (!modelArray.Contains("Tất cả") && !stationArray.Contains("Tất cả") && trangThaiBool != null)
                {
                    var results = await _context.Softwares.Include(x => x.Model)
                        .Where(s => (string.IsNullOrEmpty(searchTerm) || s.Name.Contains(searchTerm)) &&
                                     (modelArray.Length == 0 || modelArray.Contains(s.Model.Name)) &&
                                     (stationArray.Length == 0 || stationArray.Contains(s.CongDoan)) &&
                                     (s.TrangThaiApDung == trangThaiBool)
                                         ).ToListAsync();
                    return PartialView("_ketquatimkiem", results);
                }
                //0-1-0
                if (!modelArray.Contains("Tất cả") && stationArray.Contains("Tất cả") && trangThaiBool != null)
                {
                    var results = await _context.Softwares.Include(x => x.Model)
                        .Where(s => (string.IsNullOrEmpty(searchTerm) || s.Name.Contains(searchTerm)) &&
                                     (modelArray.Length == 0 || modelArray.Contains(s.Model.Name)) &&
                                     (s.TrangThaiApDung == trangThaiBool)
                                         ).ToListAsync();
                    return PartialView("_ketquatimkiem", results);
                }
                //0-1-1
                if (!modelArray.Contains("Tất cả") && stationArray.Contains("Tất cả") && trangThaiBool == null)
                {
                    var results = await _context.Softwares.Include(x => x.Model)
                        .Where(s => (string.IsNullOrEmpty(searchTerm) || s.Name.Contains(searchTerm)) &&
                                     (modelArray.Length == 0 || modelArray.Contains(s.Model.Name))
                                         ).ToListAsync();
                    return PartialView("_ketquatimkiem", results);
                }
                else
                {
                    var results = await _context.Softwares.Include(x => x.Model)
                   .Where(s => (string.IsNullOrEmpty(searchTerm) || s.Name.Contains(searchTerm)) &&
                               (modelArray.Length == 0 || modelArray.Contains(s.Model.Name)) &&
                               (stationArray.Length == 0 || stationArray.Contains(s.CongDoan)) &&
                              (trangThaiBool == null || s.TrangThaiApDung == trangThaiBool))
                   .ToListAsync();
                    return PartialView("_ketquatimkiem", results);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

    }
}
