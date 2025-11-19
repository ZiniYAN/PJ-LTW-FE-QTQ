using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend_12102025.Models.ViewModels
{
    public class InventoryVM
    {
        public List<BookEdition> NeedImportBooks { get; set; }
        public List<BookEdition> InStockBooks { get; set; }
        public InventoryVM()
        {
            NeedImportBooks = new List<BookEdition>();
            InStockBooks = new List<BookEdition>();
        }
    }
}