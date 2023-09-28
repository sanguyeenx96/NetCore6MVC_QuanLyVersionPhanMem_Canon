using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlyversion.Models;

namespace Quanlyversion.Controllers
{
    public class LichsuController : Controller
    {
        private readonly ILogger<LichsuController> _logger;
        private readonly AppDbContext _context;
        public LichsuController(ILogger<LichsuController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Lichsu()
        {
            ViewBag.parentPage = "Thống kê lịch sử";
            ViewBag.currentPage = "Lịch sử chi tiết";
            var list = await _context.Lichsus.OrderByDescending(x=>x.Thoigian).ToListAsync();
            return View(list);
        }
    }
}
