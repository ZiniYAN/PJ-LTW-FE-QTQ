using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;
namespace Frontend_12102025.Models.ViewModel
{
    public class HomeProductVM
    {
        public string SearchTerm { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } = 10;
        public List<BookEdition> FeatureProducts { get; set; }
        //List san pham co phan trang
        public PagedList.IPagedList<BookEdition> NewProducts { get; set; }
    }
}