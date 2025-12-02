using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Frontend_12102025.Models.ViewModel
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3-50 ký tự")]
        //Chỉ cho phép chữ cái, số và dấu gạch dưới
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
        //Mật khẩu phải chứa (?=.*[a-z]) (ít nhất một chữ thường ) (ít nháta một chữ hoa ) (ít nhất một số) ---- .+ là có chứa ít nhất 1 kí tự 
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ thường, 1 chữ hoa và 1 số")]
        //Hiển thị ra dạng ***
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        //So sánh với Password
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2-255 ký tự")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        //Gọi tự kiểm tra định dạng email: dadad@gmail.com 
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        //^(Bat dau voi 0) [chua 0 - 9 ] {phai co it nhat 9 hoac 10 so} 
        [RegularExpression(@"^(0|\+84)[0-9]{9,10}$",
            ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có 10-11 số")]
        [StringLength(11)]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        //Format ngay - thang - nam
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }
    }
}