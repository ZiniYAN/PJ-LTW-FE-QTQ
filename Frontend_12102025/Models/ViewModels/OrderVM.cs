using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend_12102025.Models.ViewModels
{
    public class OrderVM
    {
        [Required]
        public int OrderId { get; set; }

        // Thông tin read-only
        [Display(Name = "Khách hàng")]
        public int UserId { get; set; }

        [Display(Name = "Tên khách hàng")]
        public string Username { get; set; }

        [Display(Name = "Ngày đặt hàng")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Tổng tiền")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TotalAmount { get; set; }

        // Thông tin có thể edit
        [Display(Name = "Địa chỉ giao hàng")]
        public int? ShippingAddressId { get; set; }

        [Display(Name = "Mã giảm giá")]
        public int? CouponId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái thanh toán")]
        [Display(Name = "Trạng thái thanh toán")]
        public string PaymentStatus { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái đơn hàng")]
        [Display(Name = "Trạng thái đơn hàng")]
        public string OrderStatus { get; set; }

        // Lưu trạng thái gốc
        public string OriginalOrderStatus { get; set; }
        public string OriginalPaymentStatus { get; set; }

        // SelectLists
        public SelectList ShippingAddressesList { get; set; }
        public SelectList CouponsList { get; set; }
        public SelectList PaymentMethodsList { get; set; }
        public SelectList PaymentStatusesList { get; set; }
        public SelectList OrderStatusesList { get; set; }
    }
}