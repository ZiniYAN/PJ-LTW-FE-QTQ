using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Frontend_12102025.Models.ViewModels
{
    public class CategoriesVM
    {
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục không thể trống")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên danh mục phải từ 2-100 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        // Helper property
        public bool IsEdit => CategoryId.HasValue;

        [Display(Name = "Số lượng sách")]
        public int BookCount { get; set; }
    }
}