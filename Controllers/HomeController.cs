using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlyversion.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using Microsoft.Data.SqlClient.Server;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static System.Collections.Specialized.BitVector32;
using System.Xml.Linq;

namespace Quanlyversion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

       
        public async Task<IActionResult> Login(string username, string pwd)
        {
            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(pwd))
            {
                Password checklogin = await _context.Passwords.Where(x => (x.Type == "login" && x.Username == username && x.Pwd == pwd)).FirstOrDefaultAsync();
                if (checklogin != null)
                {
                    ISession session = HttpContext.Session;
                    session.SetString("username", checklogin.Hoten);
                    session.SetString("userrole", checklogin.Role);

                    var hoten = session.GetString("username");
                    await LuuLichSu("Login", "Đăng nhập hệ thống");

                    return RedirectToAction("settings");

                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhsachtaikhoan()
        {
            var list = await _context.Passwords.ToListAsync();
            return PartialView("_Quanlytaikhoan", list);
        }



        [HttpPost]
        public async Task<IActionResult> LuuLichSu(string type, string hanhdong)
        {
            ISession session = HttpContext.Session;
            string role = session.GetString("userrole").ToString();

            Lichsu lichsu = new Lichsu();
            if (role == "admin")
            {
                lichsu.Ten = session.GetString("username").ToString() + " " + "(Admin)";
            }
            else
            {
                lichsu.Ten = session.GetString("username").ToString();
            }
            lichsu.Hanhdong = hanhdong;
            lichsu.Thoigian = DateTime.Now;
            lichsu.Type = type;
            _context.Lichsus.Add(lichsu);
            await _context.SaveChangesAsync();
            return Ok();
        }

        public async Task<IActionResult> Settings()
        {
            ViewBag.parentPage = "Trang chủ";
            ViewBag.currentPage = "Dữ liệu";
            ViewBag.countModel = _context.Models.Count().ToString();
            ViewBag.countSoft = _context.Softwares.Count().ToString();
            ViewBag.countUser = _context.Passwords.Count().ToString();
            var recentActivities = _context.Lichsus
                .OrderByDescending(ls => ls.Thoigian)
                .Take(5)
                .ToList();
            return View(recentActivities);
        }

        [HttpPost]
        public async Task<IActionResult> CreateModel(string name, int modelTong)
        {
            var checktrung = _context.Models.FirstOrDefault(x => x.Name == name.ToUpper());
            if (checktrung != null)
            {
                return Json(new { success = false, message = "Model đã tồn tại!" });
            }
            if (modelTong == -1)
            {
                var newParentModel = new Model { Name = name.ToUpper(), ParentModelId = null };
                _context.Add(newParentModel);
                await _context.SaveChangesAsync();
                await LuuLichSu("Create", "Tạo Model: " + name.ToUpper());
                return Json(new { success = true, data = newParentModel });
            }
            else
            {
                var newChildrenModel = new Model { Name = name.ToUpper(), ParentModelId = modelTong };
                _context.Add(newChildrenModel);
                await _context.SaveChangesAsync();
                await LuuLichSu("Create", "Tạo Model: " + name.ToUpper());
                return Json(new { success = true, data = newChildrenModel });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteModel(int id)
        {
            try
            {
                var model = await _context.Models
                            .Include(c => c.ChildrenModel)
                            .FirstOrDefaultAsync(c => c.Id == id);
                if (model != null)
                {
                    // Xoá tất cả model con
                    foreach (var childModel in model.ChildrenModel)
                    {
                        _context.Models.Remove(childModel);
                    }
                    // Xoá model cha
                    _context.Models.Remove(model);
                    await _context.SaveChangesAsync();

                    await LuuLichSu("Delete", "Xoá Model: " + model.Name);
                    return Json(new { success = true, message = "Xoá thành công." });
                }
                return Json(new { success = false, message = "Không tìm thấy dữ liệu." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetParentModels()
        {
            var getAllModel = (from c in _context.Models select c)
               .Include(c => c.ParentModel) //lấy ra cả danh mục cha của category đó
               .Include(c => c.ChildrenModel)//lấy ra cả danh mục con của category đó
               .ThenInclude(s => s.Softwares);
            var parentModels = (await getAllModel.ToListAsync())
                .Where(c => c.ParentModelId == null)
                .ToList();
            return PartialView("_ModelList", parentModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetInfoModel(int id)
        {
            var getAllModel = (from c in _context.Models select c)
               .Include(c => c.ParentModel) //lấy ra cả danh mục cha của category đó
               .Include(c => c.ChildrenModel);//lấy ra cả danh mục con của category đó
            var selectModel = (await getAllModel.ToListAsync())
                .Where(c => c.Id == id)
                .FirstOrDefault();
            return PartialView("_InfoModel", selectModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetSelectListParentModels()
        {
            List<Model> parentModels = await _context.Models.Where(x => x.ParentModelId == null).ToListAsync();
            return Json(parentModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetSelectListChildrenModels()
        {
            List<Model> childrenModels = await _context.Models.Where(x => x.ParentModelId != null).ToListAsync();
            return Json(childrenModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetPwd(string type)
        {
            var data = await _context.Passwords.Where(x => x.Type == type).FirstOrDefaultAsync();
            if (data == null)
            {
                return Json(new { success = false, message = "Không tìm thấy dữ liệu." });
            }
            string pwd = data.Pwd.ToString();
            return Json(new { success = true, data = pwd });
        }

        [HttpGet]
        public async Task<IActionResult> GetThongtinmodel(int id)
        {
            var getAll = (from c in _context.Models select c).Include(c => c.ParentModel).Include(c => c.ChildrenModel);
            var thongtin = await getAll.Where(c => c.Id == id).ToListAsync(); 
            if (thongtin == null)
            {
                return Json(new { success = false, message = "Không tìm thấy dữ liệu." });
            }
            // Tạo các tùy chọn cho việc serialize JSON
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve, // Xử lý các vòng lặp tham chiếu
                MaxDepth = 64, // Điều chỉnh độ sâu tối đa nếu cần
            };
            // Serialize đối tượng thành JSON
            var jsonResult = JsonSerializer.Serialize(new { success = true, data = thongtin }, jsonOptions);
            return Content(jsonResult, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateModelName(int id, string newName)
        {
            var model = await _context.Models.FindAsync(id);
            if (model == null)
            {
                return Json(new { success = false, message = "Model không tồn tại." });
            }
            model.Name = newName;
            await _context.SaveChangesAsync();
            await LuuLichSu("Edit", "Đổi tên Model: " + newName);

            return Json(new { success = true, message = "Cập nhật tên model thành công." });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateParentModelName(int id, int newParentModelId)
        {
            var model = await _context.Models.FindAsync(id);
            if (model == null)
            {
                return Json(new { success = false, message = "Model không tồn tại." });
            }
            model.ParentModelId = newParentModelId;
            await _context.SaveChangesAsync();
            await LuuLichSu("Edit", "Đổi vị trí Model: " + model.Name);
            return Json(new { success = true, message = "Cập nhật Model tổng thành công." });
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhsachphanmem(int modelId)
        {
            var getAll = (from c in _context.Softwares select c).Include(x => x.Model);
            var danhSachphanmem = await getAll.Where(x => x.ModelId == modelId).ToListAsync();

            return PartialView("_SoftwaresList", danhSachphanmem);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        [Route("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                //if (file == null || file.Length == 0)
                //{
                //    return BadRequest("Không có tệp tin nào được tải lên.");
                //}
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "temps");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                // Kết hợp đường dẫn thư mục và tên tệp tin gốc để có đường dẫn đầy đủ.
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);
                //string uniqueFileName = originalFileName + "_" + Guid.NewGuid().ToString() + fileExtension;
                if (System.IO.File.Exists(filePath))
                {
                    return BadRequest("Tệp tin đã tồn tại với cùng tên.");
                }
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                await LuuLichSu("Upload", "Tải lên file: " + fileName);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateFolder(string folderPath)
        {
            try
            {
                string newUploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderPath);
                if (!Directory.Exists(newUploadFolder))
                {
                    Directory.CreateDirectory(newUploadFolder);
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public IActionResult CutAndPasteFile(string modelName, string fileName, string version)
        {
            try
            {
                string newFileName = version + "_" + fileName;
                string sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "temps", fileName);
                string destinationFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", modelName, newFileName);
                if (System.IO.File.Exists(sourceFilePath))
                {
                    System.IO.File.Move(sourceFilePath, destinationFilePath);
                    return Json(new { success = true, message = "File đã được cắt và dán thành công." });
                }
                else
                {
                    return Json(new { success = false, message = "File nguồn không tồn tại." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewSoft([FromBody] ThemPhanMemMoi formData)
        {
            string format = "yyyy/MM/dd";
            ISession session = HttpContext.Session;

            var newSoft = new Software();
            newSoft.Name = formData.Name;
            newSoft.ModelId = Convert.ToInt32(formData.ModelId);
            newSoft.CongDoan = formData.CongDoan;
            newSoft.DiemThayDoi = formData.DiemThayDoi;
            if (DateTime.TryParseExact(formData.NgayApDung, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resultNgayApDung))
            {
                newSoft.NgayApDung = resultNgayApDung;
            }
            if (DateTime.TryParseExact(formData.NgayCaiDat, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resultNgayCaiDat))
            {
                newSoft.NgayCaiDat = resultNgayCaiDat;
            }
            newSoft.NguoiCaiDat = formData.NguoiCaiDat;
            newSoft.NguoiXacNhan = formData.NguoiXacNhan;
            newSoft.SoLuongJigCaiDat = Convert.ToInt32(formData.SoLuongJigCaiDat);
            newSoft.TrangThaiApDung = bool.Parse(formData.TrangThaiApDung);
            newSoft.Version = formData.Version;
            newSoft.FileName = formData.FileName;
            newSoft.Path = formData.Path;

            newSoft.CreatedBy = session.GetString("username").ToString();
            newSoft.TimeCreated = DateTime.Now;

            _context.Add(newSoft);
            await _context.SaveChangesAsync();
            await LuuLichSu("Create", "Thêm phần mềm: " + formData.Name + " , Đường dẫn: " + formData.Path);

            return Json(new { success = true, data = newSoft });
        }

        public IActionResult DownloadFile(string filename)
        {
            // Đường dẫn thực tế đến file trên máy chủ
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);

            // Kiểm tra xem file có tồn tại không
            if (System.IO.File.Exists(filePath))
            {
                // Đọc nội dung của file thành một mảng byte
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                // Xác định loại nội dung của file
                string contentType = "application/octet-stream"; // Loại nội dung tổng quát cho các tệp tin nhị phân
                // Tạo một tên file để hiển thị khi tải về
                string downloadFileName = filename; // Bạn có thể điều chỉnh tên file theo ý muốn
                LuuLichSu("Download", "Tải về phần mềm: " + filename);
                return File(fileBytes, contentType, downloadFileName);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInfoIdSoft(int softId)
        {
            var getAll = (from c in _context.Softwares select c).Include(x => x.Model);
            var soft = await getAll.Where(x => x.Id == softId).FirstOrDefaultAsync();
            return PartialView("_SuaThongTinPhanMem", soft);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInfoSoft(int id, string tenmodelhientai, string tenphanmem,
            string version, string modelid, string congdoan, string trangthaiapdung,
            string diemthaydoi, string ngayapdung, string ngaycaidat, string nguoicaidat,
            string soluongjigcaidat, string nguoixacnhan)
        {
            var Soft = await _context.Softwares.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (Soft != null)
            {
                string format = "yyyy/MM/dd";
                ISession session = HttpContext.Session;
                Soft.Name = tenphanmem;
                Soft.CongDoan = congdoan;
                if (Soft.Version != version && Soft.ModelId == Convert.ToInt32(modelid))
                {
                    Soft.Version = version;
                    string newfilename = version + "_" + Soft.FileName;
                    string sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", tenmodelhientai, Soft.FileName);
                    string destinationFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", tenmodelhientai, newfilename);
                    if (System.IO.File.Exists(sourceFilePath))
                    {
                        System.IO.File.Move(sourceFilePath, destinationFilePath);
                    }
                    else
                    {
                        return Json(new { success = false, message = "File nguồn không tồn tại." });
                    }
                    Soft.FileName = newfilename;
                    string newfilePath = "uploads" + "\\" + tenmodelhientai + "\\" + newfilename;
                    Soft.Path = newfilePath;
                    await LuuLichSu("Edit", "Đổi thông tin Version phần mềm: " + tenphanmem + "_" + version);

                }
                if (Soft.ModelId != Convert.ToInt32(modelid) && Soft.Version == version)
                {
                    Soft.ModelId = Convert.ToInt32(modelid);
                    string tenmodelmoi = _context.Models.Where(x => x.Id == Convert.ToInt32(modelid)).FirstOrDefault().Name;
                    string newUploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", tenmodelmoi);
                    if (!Directory.Exists(newUploadFolder))
                    {
                        Directory.CreateDirectory(newUploadFolder);
                    }
                    string sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", tenmodelhientai, Soft.FileName);
                    string destinationFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", tenmodelmoi, Soft.FileName);
                    if (System.IO.File.Exists(sourceFilePath))
                    {
                        System.IO.File.Move(sourceFilePath, destinationFilePath);
                    }
                    else
                    {
                        return Json(new { success = false, message = "File nguồn không tồn tại." });
                    }
                    string newfilePath = "uploads" + "\\" + tenmodelmoi + "\\" + Soft.FileName;
                    Soft.Path = newfilePath;
                    await LuuLichSu("Edit", "Đổi thông tin Model phần mềm: " + tenphanmem + "_" + tenmodelmoi);

                }

                if (Soft.ModelId != Convert.ToInt32(modelid) && Soft.Version != version)
                {
                    string sourceFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", tenmodelhientai, Soft.FileName);
                    string tenmodelmoi = _context.Models.Where(x => x.Id == Convert.ToInt32(modelid)).FirstOrDefault().Name;
                    string newUploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", tenmodelmoi);
                    if (!Directory.Exists(newUploadFolder))
                    {
                        Directory.CreateDirectory(newUploadFolder);
                    }
                    string newfilename = version + "_" + Soft.FileName;
                    string destinationFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", tenmodelmoi, newfilename);
                    if (System.IO.File.Exists(sourceFilePath))
                    {
                        System.IO.File.Move(sourceFilePath, destinationFilePath);
                    }
                    else
                    {
                        return Json(new { success = false, message = "File nguồn không tồn tại." });
                    }
                    string newfilePath = "uploads" + "\\" + tenmodelmoi + "\\" + newfilename;
                    Soft.Path = newfilePath;
                    Soft.FileName = newfilename;
                    Soft.ModelId = Convert.ToInt32(modelid);
                    await LuuLichSu("Edit", "Đổi thông tin Model và Version phần mềm: " + tenphanmem + "_" + tenmodelmoi + "_" + tenmodelmoi);

                }

                Soft.DiemThayDoi = diemthaydoi;
                if (DateTime.TryParseExact(ngayapdung, format,
                    CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime resultNgayApDung))
                {
                    Soft.NgayApDung = resultNgayApDung;
                }
                if (DateTime.TryParseExact(ngaycaidat, format,
                    CultureInfo.InvariantCulture, DateTimeStyles.None,
                    out DateTime resultNgayCaiDat))
                {
                    Soft.NgayCaiDat = resultNgayCaiDat;
                }
                Soft.NguoiCaiDat = nguoicaidat;
                Soft.SoLuongJigCaiDat = Convert.ToInt32(soluongjigcaidat);
                Soft.NguoiXacNhan = nguoixacnhan;
                Soft.TrangThaiApDung = bool.Parse(trangthaiapdung);
                Soft.CreatedBy = session.GetString("username").ToString();
                await _context.SaveChangesAsync();
                await LuuLichSu("Edit", "Thay đổi thông tin phần mềm: " + tenphanmem);
                return Json(new { success = true });

            }
            return Json(new { success = false, error = "Record not found" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSoft(int id)
        {
            try
            {
                var software = await _context.Softwares.FindAsync(id);
                if (software == null)
                {
                    return NotFound();
                }
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", software.Path);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _context.Softwares.Remove(software);
                await _context.SaveChangesAsync();
                await LuuLichSu("Delete", "Xoá phần mềm: " + software.Name);

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}