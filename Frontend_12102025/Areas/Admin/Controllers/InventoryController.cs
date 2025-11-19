using Frontend_12102025.Models;
using Frontend_12102025.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend_12102025.Areas.Admin.Controllers
{
    public class InventoryController : Controller
    {
        dbprojectltwEntities db = new dbprojectltwEntities();
        // GET: Admin/Inventory
        public ActionResult Index()
        {
            var inventory = new InventoryVM 
            { 
                NeedImportBooks = db.BookEditions.Where(b => b.Stock == 0).Include(b => b.BookTitle).Include(b => b.BookTitle.Category).ToList(),
                InStockBooks = db.BookEditions.Where(b => b.Stock > 0).Include(b => b.BookTitle).Include(b => b.BookTitle.Category).ToList(),
            };
            return View(inventory);
        }
    }
}