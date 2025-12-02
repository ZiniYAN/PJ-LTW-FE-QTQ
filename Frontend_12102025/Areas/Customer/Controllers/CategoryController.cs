using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frontend_12102025.Models;

namespace Frontend_12102025.Areas.Customer.Controllers
{
    public class CategoryController : Controller
    {
        private dbprojectltwEntities db = new dbprojectltwEntities();

        public ActionResult Index(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return RedirectToAction("Index", "Home");
            }
            // Dictionary voi key la URL va value va ten trong db
            var categoryMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // URL slug |  Database name 
                { "Khoa-hoc", "Khoa học" },
                { "Tam-ly-hoc", "Tâm lý học" },
                { "Van-hoc", "Văn học" },
                { "Kinh-te", "Kinh tế" },
                { "Self-Help", "Self-Help" }
            };

            string searchName;

            // Lay key tu mapping ra 
            if (!categoryMapping.TryGetValue(categoryName, out searchName))
            {
                // Thay - bang khoang trong
                searchName = categoryName.Replace("-", " ");
            }

            // Tim trong db
            var category = db.Categories
                .AsEnumerable()
                .FirstOrDefault(c => string.Equals(c.CategoryName, searchName,
                    StringComparison.OrdinalIgnoreCase));

            if (category == null)
            {
                TempData["ErrorMessage"] = $"Không tìm thấy danh mục '{searchName}'.";
                return RedirectToAction("Index", "Home");
            }

            // Tim ra roi thi lay sach theo category
            var books = db.BookEditions
                .Where(b => b.BookTitle.CategoryId == category.CategoryId)
                .OrderByDescending(b => b.PublishDate)
                .ToList();

            ViewBag.CategoryName = category.CategoryName;
            ViewBag.CategoryDescription = category.Description ?? "";
            ViewBag.BookCount = books.Count;

            return View(books);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
