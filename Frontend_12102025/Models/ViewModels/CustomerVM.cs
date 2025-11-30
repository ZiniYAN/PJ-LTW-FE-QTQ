using System;
using System.ComponentModel.DataAnnotations;

namespace Frontend_12102025.ViewModels
{
    public class CustomerVM
    {
        public int CustomerId { get; set; }

        [Display(Name = "User ID")]
        public int UserId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được quá 500 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        [Display(Name = "Loại khách hàng")]
        public string CustomerType { get; set; }

        [Display(Name = "Tổng số đơn hàng")]
        public int? TotalOrders { get; set; }

        [Display(Name = "Tổng chi tiêu")]
        [DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? TotalSpent { get; set; }

        [Display(Name = "Ngày đặt hàng cuối")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? LastOrderDate { get; set; }

        [Display(Name = "Trạng thái")]
        public string CustomerStatus { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }
    }
}