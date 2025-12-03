using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Frontend_12102025.Models;
using Frontend_12102025.Models.ViewModels;
using Frontend_12102025.ViewModels;

namespace Frontend_12102025.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        private dbprojectltwEntities db = new dbprojectltwEntities();

        // GET: Admin/Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        // GET: Admin/Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Admin/Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoriesVM category)
        {
            if (!string.IsNullOrWhiteSpace(category.CategoryName))
            {
                var categoryNameExist = await db.Categories.AnyAsync(c => c.CategoryName.ToLower() == category.CategoryName.Trim().ToLower());

                if (categoryNameExist)
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại");
                }
            }
            if (!string.IsNullOrWhiteSpace(category.CategoryName))
            {
                var invalidChars = new[] { '<', '>', '&', '"', '\'' };
                if (category.CategoryName.Any(c => invalidChars.Contains(c)))
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục không được chứa ký tự đặc biệt");
                }
            }
            if (ModelState.IsValid)
            {
                var newCategory = new Category { 
                    CategoryName = category.CategoryName.Trim(),
                    Description = category.Description,
                };
                db.Categories.Add(newCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            var editedCategory = new CategoriesVM
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
            return View(editedCategory);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CategoriesVM category)
        {
            if (!string.IsNullOrWhiteSpace(category.CategoryName))
            {
                var categoryNameExists = await db.Categories.AnyAsync(c => c.CategoryName.ToLower() == category.CategoryName.Trim().ToLower() && c.CategoryId != category.CategoryId);
                if (categoryNameExists)
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục đã tồn tại");
                }
            }
            if (!string.IsNullOrWhiteSpace(category.CategoryName))
            {
                var invalidChars = new[] { '<', '>', '&', '"', '\'' };
                if (category.CategoryName.Any(c => invalidChars.Contains(c)))
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục không được chứa ký tự đặc biệt");
                }
            }
            if (ModelState.IsValid)
            {
                var editedCategory = await db.Categories.FindAsync(category.CategoryId);
                if (editedCategory == null)
                {
                    return HttpNotFound();
                }
                editedCategory.CategoryName = category.CategoryName.Trim();
                editedCategory.Description = category.Description;
                db.Entry(editedCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }

                var hasBooks = db.BookTitles.Any(b => b.CategoryId == id);

                if (hasBooks)
                {
                    TempData["ErrorMessage"] = "Không thể xóa danh mục đang có sách! Vui lòng xóa hoặc chuyển sách sang danh mục khác trước.";
                    return RedirectToAction("Index");
                }

                db.Categories.Remove(category);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Xóa danh mục thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                return RedirectToAction("Index");
            }
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
