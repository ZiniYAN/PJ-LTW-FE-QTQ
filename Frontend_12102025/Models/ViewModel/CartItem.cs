using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend_12102025.Models.ViewModel
{
    [Serializable]
    public class CartItem
    {
        public int BookEditionId { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string ISBN { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string CoverImage { get; set; }
        public int Stock { get; set; } // kiểm tra số sách còn tồn trong kho hàng

        public decimal TotalPrice
        {
            get { return Quantity * UnitPrice; }
        }
    }
}