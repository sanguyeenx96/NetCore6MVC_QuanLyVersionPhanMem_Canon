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
            ViewBag.parentPage = "Trang chủ";
            ViewBag.currentPage = "Lịch sử hoạt động";
            var list = await _context.Lichsus.OrderBy(x=>x.Thoigian).ToListAsync();
            return View(list);
        }
    }
}
