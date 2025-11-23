using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Frontend_12102025.Models;

namespace Frontend_12102025.Areas.Admin
{
    public class BooksController : Controller
    {
        private dbprojectltwEntities db = new dbprojectltwEntities();

        // GET: Admin/Books
        public ActionResult Index()
        {
            var Books = db.BookEditions
                .Include(e => e.BookTitle.Author)
                .Include(e => e.BookTitle.Category)
                .Include(e => e.BookTitle.Publisher);
            return View(Books.ToList());
        }

        // GET: Admin/Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookEdition book = db.BookEditions
                .Include(e => e.BookTitle.Author)
                .Include(e => e.BookTitle.Category)
                .Include(e => e.BookTitle.Publisher)
                .FirstOrDefault(e => e.BookTitleId == id);
            
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Admin/Books/Create
        public ActionResult Create()
        {
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorId", "AuthorName");
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryId", "CategoryName");
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherId", "PublisherName");
            return View();
        }

        // POST: Admin/Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookEdition bookEdition, string ImageUrl)
        {
            if (ModelState.IsValid)
            {
                // 1. Tạo BookTitle trước
                var bookTitle = new BookTitle
                {
                    Title = bookEdition.BookTitle.Title,
                    Description = bookEdition.BookTitle.Description,
                    AuthorId = int.Parse(Request.Form["AuthorId"]),
                    PublisherId = int.Parse(Request.Form["PublisherId"]),
                    CategoryId = int.Parse(Request.Form["CategoryId"])
                };

                db.BookTitles.Add(bookTitle);
                db.SaveChanges(); // Lấy BookTitleId

                // 2. Tạo BookEdition
                var newBookEdition = new BookEdition
                {
                    BookTitleId = bookTitle.BookTitleId,
                    Price = bookEdition.Price,
                    Stock = bookEdition.Stock,
                    ISBN = bookEdition.ISBN,
                    PublishDate = bookEdition.PublishDate
                };

                db.BookEditions.Add(newBookEdition);
                db.SaveChanges(); // Lấy BookEditionId

                // 3. Lưu Image URL vào BookImages
                if (!string.IsNullOrEmpty(ImageUrl))
                {
                    var bookImage = new BookImage
                    {
                        BookEditionId = newBookEdition.BookEditionId,
                        ImageUrl = ImageUrl
                    };

                    db.BookImages.Add(bookImage);
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            // Nếu lỗi, load lại dropdown
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorId", "AuthorName", bookEdition.BookTitle.AuthorId);
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherId", "PublisherName", bookEdition.BookTitle.PublisherId);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryId", "CategoryName", bookEdition.BookTitle.CategoryId);

            return View(bookEdition);
        }

        // GET: Admin/Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            BookEdition book = db.BookEditions
                .Include(e => e.BookTitle)
                .FirstOrDefault(e => e.BookTitleId == id);
            
            if (book == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "AuthorName", book.BookTitle.AuthorId);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", book.BookTitle.CategoryId);
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "PublisherName", book.BookTitle.PublisherId);
            
            return View(book);
        }

        // POST: Admin/Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BookEdition bookEdition, int AuthorID, int PublisherID, int CategoryID)
        {
            if (ModelState.IsValid)
            {
                // Update BookTitle
                var bookTitle = db.BookTitles.Find(bookEdition.BookTitleId);
                if (bookTitle != null)
                {
                    bookTitle.Title = bookEdition.BookTitle.Title;
                    bookTitle.AuthorId = AuthorID;
                    bookTitle.PublisherId = PublisherID;
                    bookTitle.CategoryId = CategoryID;
                    bookTitle.Description = bookEdition.BookTitle.Description;
                    
                    db.Entry(bookTitle).State = EntityState.Modified;
                }
                
                // Update BookEdition
                var existingEdition = db.BookEditions.Find(bookEdition.BookEditionId);
                if (existingEdition != null)
                {
                    existingEdition.ISBN = bookEdition.ISBN;
                    existingEdition.Price = bookEdition.Price;
                    existingEdition.Stock = bookEdition.Stock;
                    existingEdition.PublishDate = bookEdition.PublishDate;
                    existingEdition.CoverImage = bookEdition.CoverImage;
                    
                    db.Entry(existingEdition).State = EntityState.Modified;
                }
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "AuthorName", AuthorID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", CategoryID);
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "PublisherName", PublisherID);
            
            return View(bookEdition);
        }

        // GET: Admin/Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            BookEdition book = db.BookEditions
                .Include(e => e.BookTitle.Author)
                .Include(e => e.BookTitle.Category)
                .Include(e => e.BookTitle.Publisher)
                .FirstOrDefault(e => e.BookTitleId == id);
            
            if (book == null)
            {
                return HttpNotFound();
            }
            
            return View(book);
        }

        // POST: Admin/Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Tìm BookEdition theo BookTitleId
            var bookEditions = db.BookEditions.Where(e => e.BookTitleId == id).ToList();
            
            // Xóa tất cả BookEditions liên quan
            foreach (var edition in bookEditions)
            {
                db.BookEditions.Remove(edition);
            }
            
            // Xóa BookTitle
            BookTitle bookTitle = db.BookTitles.Find(id);
            if (bookTitle != null)
            {
                db.BookTitles.Remove(bookTitle);
            }
            
            db.SaveChanges();
            return RedirectToAction("Index");
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