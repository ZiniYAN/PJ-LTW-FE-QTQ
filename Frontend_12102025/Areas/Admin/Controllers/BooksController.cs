using Frontend_12102025.Models;
using Frontend_12102025.Models.ViewModels;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

namespace Frontend_12102025.Areas.Admin.Controllers
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
            var model = new BookEditionVM
            {
                CategoryList = new SelectList(db.Categories, "CategoryId", "CategoryName")
            };
            return View(model);
        }

        // POST: Admin/Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookEditionVM bookEdition)
        {
            // Check ISBN duplicate
            if (!string.IsNullOrWhiteSpace(bookEdition.ISBN))
            {
                bool isbnExists = db.BookEditions.Any(e => e.ISBN == bookEdition.ISBN.Trim());
                if (isbnExists)
                {
                    ModelState.AddModelError("ISBN", "ISBN đã tồn tại!");
                }
            }

            // Check Category exists
            if (!db.Categories.Any(c => c.CategoryId == bookEdition.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Danh mục không hợp lệ!");
            }

            // Check duplicate book (same title + author)
            if (!string.IsNullOrWhiteSpace(bookEdition.Title) && !string.IsNullOrWhiteSpace(bookEdition.AuthorName))
            {
                bool bookExists = db.BookTitles.Any(b =>
                    b.Title.ToLower() == bookEdition.Title.Trim().ToLower() &&
                    b.Author.AuthorName.ToLower() == bookEdition.AuthorName.Trim().ToLower());

                if (bookExists)
                {
                    ModelState.AddModelError("", "Sách đã tồn tại (trùng tên sách và tác giả)!");
                }
            }

            // 2. Return if validation fail
            if (!ModelState.IsValid)
            {
                bookEdition.CategoryList = new SelectList(db.Categories, "CategoryId", "CategoryName", bookEdition.CategoryId);
                return View(bookEdition);
            }

            try
            {
                var authorName = bookEdition.AuthorName.Trim();
                var author = db.Authors.FirstOrDefault(a => a.AuthorName.ToLower() == authorName.ToLower());
                if (author == null)
                {
                    author = new Author { AuthorName = authorName };
                    db.Authors.Add(author);
                    db.SaveChanges();
                }

                var publisherName = bookEdition.PublisherName.Trim();
                var publisher = db.Publishers.FirstOrDefault(p => p.PublisherName.ToLower() == publisherName.ToLower());
                if (publisher == null)
                {
                    publisher = new Frontend_12102025.Models.Publisher { PublisherName = publisherName };
                    db.Publishers.Add(publisher);
                    db.SaveChanges();
                }

                var bookTitle = new BookTitle
                {
                    Title = bookEdition.Title.Trim(),
                    Description = bookEdition.Description?.Trim(),
                    AuthorId = author.AuthorId,
                    PublisherId = publisher.PublisherId,
                    CategoryId = bookEdition.CategoryId
                };
                db.BookTitles.Add(bookTitle);
                db.SaveChanges();

                var book = new BookEdition
                {
                    BookTitleId = bookTitle.BookTitleId,
                    ISBN = bookEdition.ISBN.Trim(),
                    Price = bookEdition.Price,
                    Stock = bookEdition.Stock,
                    PublishDate = bookEdition.PublishDate,
                    CoverImage = bookEdition.CoverImage.Trim()
                };
                db.BookEditions.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                bookEdition.CategoryList = new SelectList(db.Categories, "CategoryId", "CategoryName", bookEdition.CategoryId);
                return View(bookEdition);
            }
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

            var model = new BookEditionVM
            {
                BookEditionId = book.BookEditionId,
                BookTitleId = book.BookTitleId,
                Title = book.BookTitle.Title,
                Description = book.BookTitle.Description,
                AuthorName = book.BookTitle.Author.AuthorName,
                PublisherName = book.BookTitle.Publisher.PublisherName,
                CategoryId = book.BookTitle.CategoryId,
                ISBN = book.ISBN,
                Price = book.Price,
                Stock = book.Stock,
                PublishDate = book.PublishDate ?? DateTime.Now,
                CoverImage = book.CoverImage,
                CategoryList = new SelectList(db.Categories, "CategoryId", "CategoryName", book.BookTitle.CategoryId)
            };


            return View(model);
        }

        // POST: Admin/Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BookEditionVM bookEdition)
        {
            System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            // Thêm đoạn này để xem lỗi
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                if (state.Errors.Count > 0)
                {
                    foreach (var error in state.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine($"Field: {key} - Error: {error.ErrorMessage}");
                    }
                }
            }
            // Validate IBSN
            if (!string.IsNullOrEmpty(bookEdition.ISBN))
            {
                bool ISBNIsExist = db.BookEditions.Any(e => e.ISBN == bookEdition.ISBN && e.BookEditionId != bookEdition.BookEditionId);
                if (ISBNIsExist)
                {
                    ModelState.AddModelError("IBSN", "IBSN đã tồn tại");
                }
            }
            // Check Category exists
            if (!db.Categories.Any(c => c.CategoryId == bookEdition.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Danh mục không hợp lệ!");
            }

            // Check duplicate book (exclude current)
            if (!string.IsNullOrWhiteSpace(bookEdition.Title) && !string.IsNullOrWhiteSpace(bookEdition.AuthorName))
            {
                bool bookExists = db.BookTitles.Any(b =>
                    b.Title.ToLower() == bookEdition.Title.Trim().ToLower() &&
                    b.Author.AuthorName.ToLower() == bookEdition.AuthorName.Trim().ToLower() &&
                    b.BookTitleId != bookEdition.BookTitleId);

                if (bookExists)
                {
                    ModelState.AddModelError("", "Sách đã tồn tại (trùng tên sách và tác giả)!");
                }
            }

            // 2. Return nếu validation fail
            if (!ModelState.IsValid)
            {
                bookEdition.CategoryList = new SelectList(db.Categories, "CategoryId", "CategoryName", bookEdition.CategoryId);
                return View(bookEdition);
            }

            // 3. Update entities
            try
            {
                // Tìm hoặc tạo Author
                var authorName = bookEdition.AuthorName.Trim();
                var author = db.Authors.FirstOrDefault(a => a.AuthorName.ToLower() == authorName.ToLower());
                if (author == null)
                {
                    author = new Author { AuthorName = authorName };
                    db.Authors.Add(author);
                    db.SaveChanges();
                }

                // Tìm hoặc tạo Publisher
                var publisherName = bookEdition.PublisherName.Trim();
                var publisher = db.Publishers.FirstOrDefault(p => p.PublisherName.ToLower() == publisherName.ToLower());
                if (publisher == null)
                {
                    publisher = new Frontend_12102025.Models.Publisher { PublisherName = publisherName };
                    db.Publishers.Add(publisher);
                    db.SaveChanges();
                }

                // Update BookTitle
                var bookTitle = db.BookTitles.Find(bookEdition.BookTitleId);
                if (bookTitle == null)
                {
                    return HttpNotFound();
                }

                bookTitle.Title = bookEdition.Title.Trim();
                bookTitle.Description = bookEdition.Description?.Trim();
                bookTitle.AuthorId = author.AuthorId;
                bookTitle.PublisherId = publisher.PublisherId;
                bookTitle.CategoryId = bookEdition.CategoryId;

                var book = db.BookEditions.Find(bookEdition.BookEditionId);
                if (book == null)
                {
                    return HttpNotFound();
                }

                book.ISBN = bookEdition.ISBN.Trim();
                book.Price = bookEdition.Price;
                book.Stock = bookEdition.Stock;
                book.PublishDate = bookEdition.PublishDate;
                book.CoverImage = bookEdition.CoverImage.Trim();

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                bookEdition .CategoryList = new SelectList(db.Categories, "CategoryId", "CategoryName", bookEdition.CategoryId);
                return View(bookEdition);
            }
           
        }

        // GET: Admin/Books/Delete/5
        public ActionResult Delete(int? id)
        {
            try
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
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.ToString()}");
                TempData["ErrorMessage"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Admin/Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var bookEdition = db.BookEditions
                    .Include(e => e.BookTitle)
                    .FirstOrDefault(e => e.BookEditionId == id);

                if (bookEdition == null)
                {
                    return HttpNotFound();
                }

                // Check xem sách có trong order không
                bool hasOrders = db.OrderDetails.Any(od => od.BookEditionId == id);
                if (hasOrders)
                {
                    TempData["ErrorMessage"] = "Không thể xóa sách này vì đã có đơn hàng!";
                    return RedirectToAction("Index");
                }

                // Xóa BookImages
                var bookImages = db.BookImages.Where(img => img.BookEditionId == id).ToList();
                db.BookImages.RemoveRange(bookImages);

                // Lấy BookTitleId trước khi xóa
                int bookTitleId = bookEdition.BookTitleId;

                // Xóa BookEdition
                db.BookEditions.Remove(bookEdition);

                // Check nếu không còn edition nào thì xóa luôn BookTitle
                bool hasOtherEditions = db.BookEditions.Any(e =>
                    e.BookTitleId == bookTitleId &&
                    e.BookEditionId != id);

                if (!hasOtherEditions)
                {
                    var bookTitle = db.BookTitles.Find(bookTitleId);
                    if (bookTitle != null)
                    {
                        db.BookTitles.Remove(bookTitle);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
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