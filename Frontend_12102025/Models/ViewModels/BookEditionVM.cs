using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend_12102025.Models.ViewModels
{
    public class BookEditionVM
    {
        // ID
        public int BookEditionId { get; set; }
        public int BookTitleId { get; set; }

        // Title
        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        [StringLength(500, ErrorMessage = "Tên sách tối đa 500 ký tự")]
        [Display(Name = "Tên sách")]
        public string Title { get; set; }

        [StringLength(2000, ErrorMessage = "Mô tả tối đa 2000 ký tự")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        // Author
        [Required(ErrorMessage = "Vui lòng nhập tên tác giả")]
        [StringLength(200, ErrorMessage = "Tên tác giả tối đa 200 ký tự")]
        [Display(Name = "Tác giả")]
        public string AuthorName { get; set; }

        // Publisher
        [Required(ErrorMessage = "Vui lòng nhập nhà xuất bản")]
        [StringLength(200, ErrorMessage = "Tên NXB tối đa 200 ký tự")]
        [Display(Name = "Nhà xuất bản")]
        public string PublisherName { get; set; }

        // Category
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        // ISBN, price, stock, ngay xuat ban va url hinh sach
        [Required(ErrorMessage = "Vui lòng nhập ISBN")]
        [StringLength(13, ErrorMessage = "ISBN tối đa 13 ký tự")]
        [Display(Name = "ISBN")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không được âm")]
        [Display(Name = "Số lượng tồn kho")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày xuất bản")]
        [Display(Name = "Ngày xuất bản")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập URL ảnh bìa")]
        [StringLength(200, ErrorMessage = "URL ảnh tối đa 200 ký tự")]
        [Display(Name = "Ảnh bìa")]
        public string CoverImage { get; set; }

        public SelectList CategoryList { get; set; }
    }
}