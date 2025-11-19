using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;
namespace Frontend_12102025.Models.ViewModel
{
    public class ProductDetailVM
    {
        public BookEdition product { get; set; }
        public int quantity { get; set; } = 1;
        public decimal estimatedValue { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 3;
        public List<BookEdition> RelatedProduct { get; set; }
        public PagedList.IPagedList<BookEdition> TopProducts { get; set; }
    }
}