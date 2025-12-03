using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend_12102025.Models.ViewModels
{
    public class ProductSearchVM
    {
        public string SortTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortOrder { get; set; }
        public List<BookEdition> Books { get; set; }
    }
}