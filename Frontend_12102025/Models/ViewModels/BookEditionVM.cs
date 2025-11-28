using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Frontend_12102025.Models.ViewModels
{
    public class BookEditionVM
    {
        public int BookEditionId { get; set; }

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(20, ErrorMessage = "ISBN max 20 characters")]
        public string ISBN { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be >= 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be >= 0")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(500)]
        public string Title { get; set; }
    }
}