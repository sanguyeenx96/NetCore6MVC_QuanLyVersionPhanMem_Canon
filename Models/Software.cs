using Microsoft.AspNetCore.Authentication;
using Microsoft.Build.ObjectModelRemoting;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace Quanlyversion.Models
{
    public class Software
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Phải nhập tên phần mềm")]
        [StringLength(255)]
        [Display(Name = "Tên phần mềm")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phải nhập tên công đoạn")]
        [Display(Name = "Tên công đoạn")]
        public string CongDoan { get; set; }

        [Required(ErrorMessage = "Phải nhập version của phần mềm")]
        [Display(Name = "Số version")]
        public string Version { get; set; }

        [Display(Name = "Điểm thay đổi")]
        public string? DiemThayDoi { get; set; }

        [Display(Name = "Ngày áp dụng")]
        public DateTime? NgayApDung { get; set; }

        [Required(ErrorMessage = "Phải nhập tên người cài đặt")]
        [Display(Name = "Người cài đặt")]
        public string? NguoiCaiDat { get; set; }

        [Display(Name = "Ngày cài đặt")]
        public DateTime? NgayCaiDat { get; set; }

        [Display(Name = "Số lượng Jig cài đặt")]
        public int? SoLuongJigCaiDat { get; set; }

        [Required(ErrorMessage = "Phải nhập tên người xác nhận")]
        [Display(Name = "Người xác nhận")]
        public string? NguoiXacNhan { get; set; }

        [Required(ErrorMessage = "Phải nhập trạng thái")]
        [Display(Name = "Trạng thái áp dụng")]
        public bool TrangThaiApDung { get; set; }

        [Display(Name = "Đường dẫn tới File phần mềm")]
        public string Path { get; set; } // Đường dẫn đến tệp

        [Display(Name = "Tên File phần mềm")]
        public string FileName { get; set; } // Tên tệp

        [Display(Name = "Người tạo")]
        public string CreatedBy { get; set; }

        [Display(Name = "Ngày giờ tạo")]
        public DateTime TimeCreated { get; set; }

        public int ModelId { get; set; } // Khóa ngoại đến bảng cha
        public Model? Model { get; set; } // Thuộc tính navigation
    }
}