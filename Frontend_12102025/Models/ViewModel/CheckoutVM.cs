using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Frontend_12102025.Models.ViewModel
{
    public class CheckoutVM
    {
        // Giỏ hàng
        public List<CartItem> CartItems { get; set; }

        [Display(Name = "Tổng tiền hàng")]
        public decimal SubTotal { get; set; }

        [Display(Name = "Phí vận chuyển")]
        public decimal ShippingFee { get; set; }

        [Display(Name = "Giảm giá")]
        public decimal DiscountAmount { get; set; }

        [Display(Name = "Tổng thanh toán")]
        public decimal TotalAmount { get; set; }

        // Thông tin user
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Địa chỉ giao hàng
        public int? SelectedAddressId { get; set; }
        public List<ShippingAddressVM> SavedAddresses { get; set; }
        public ShippingAddressVM NewAddress { get; set; }
        public bool UseNewAddress { get; set; } // Checkbox để chọn dùng địa chỉ mới

        // Thanh toán
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PaymentMethod { get; set; }

        // Mã giảm giá
        [Display(Name = "Mã giảm giá")]
        public string CouponCode { get; set; }
        public int? CouponId { get; set; }

        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được quá 500 ký tự")]
        public string OrderNotes { get; set; }
    }
}