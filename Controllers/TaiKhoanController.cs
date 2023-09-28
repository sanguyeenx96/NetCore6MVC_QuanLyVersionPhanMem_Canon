using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlyversion.Models;

namespace Quanlyversion.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ILogger<TaiKhoanController> _logger;
        private readonly AppDbContext _context;

        public TaiKhoanController(ILogger<TaiKhoanController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> LuuLichSu(string type, string hanhdong)
        {
            ISession session = HttpContext.Session;
            Lichsu lichsu = new Lichsu();
            lichsu.Ten = session.GetString("username").ToString();
            lichsu.Hanhdong = hanhdong;
            lichsu.Thoigian = DateTime.Now;
            lichsu.Type = type;
            _context.Lichsus.Add(lichsu);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ThemTaiKhoanMoi(string usn, string pwd, string hoten)
        {
            var checktrungusn = _context.Passwords.Where(x => x.Username == usn).Count();
            if (checktrungusn > 0)
            {
                return Json(new { success = false });
            }
            try
            {
                Password user = new Password();
                user.Hoten = hoten;
                user.Username = usn;
                user.Pwd = pwd;
                user.Role = "user";
                user.Type = "login";
                _context.Add(user);
                await _context.SaveChangesAsync();
                await LuuLichSu("Create", "Tạo tài khoản mới cho : " + hoten);

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> XoaTaiKhoan(int id)
        {
            var user = await _context.Passwords.FindAsync(id);
            if (user != null)
            {
                _context.Passwords.Remove(user);
                await _context.SaveChangesAsync();
                await LuuLichSu("Delete", "Xoá tài khoản : " + user.Username + " , " + user.Hoten);

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Sua(int id, string usn, string pwd, string hoten)
        {
            var checktrungusn = _context.Passwords.Where(x => x.Username == usn).Count();
            if (checktrungusn > 1)
            {
                return Json(new { success = false });
            }
            var user = await _context.Passwords.FindAsync(id);
            if (user != null)
            {
                user.Username = usn;
                user.Pwd = pwd;
                user.Hoten = hoten;
                await _context.SaveChangesAsync();
                await LuuLichSu("Edit", "Sửa thông tin tài khoản : " + user.Username + " , " + user.Hoten);

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SuamatkhauUngdung(string pwd)
        {

            var matkhau = await _context.Passwords.Where(x => x.Type == "edit").FirstOrDefaultAsync();
            if (matkhau != null)
            {
                matkhau.Pwd = pwd;
                await _context.SaveChangesAsync();
                await LuuLichSu("Edit", "Thay đổi mật khẩu ứng dụng");

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
    }
}
