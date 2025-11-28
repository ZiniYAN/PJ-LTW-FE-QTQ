using Frontend_12102025.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

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
            // Validate null
            if (bookEdition.BookTitle?.Author?.AuthorName == null)
            {
                ModelState.AddModelError("", "Author name is required!");
            }

            if (bookEdition.BookTitle?.Publisher?.PublisherName == null)
            {
                ModelState.AddModelError("", "Publisher name is required!");
            }

            // Validate ISBN 
            if (!string.IsNullOrEmpty(bookEdition.ISBN))
            {
                bool isbnExists = db.BookEditions.Any(e => e.ISBN == bookEdition.ISBN);
                if (isbnExists)
                {
                    ModelState.AddModelError("ISBN", "ISBN already exists!");
                }
            }

            // Validate Price va Stock
            if (bookEdition.Price < 0)
            {
                ModelState.AddModelError("Price", "Price must be greater than or equal to 0!");
            }

            if (bookEdition.Stock < 0)
            {
                ModelState.AddModelError("Stock", "Stock must be greater than or equal to 0!");
            }

            // Validate CategoryId
            int categoryId;
            if (!int.TryParse(Request.Form["CategoryId"], out categoryId) ||
                !db.Categories.Any(c => c.CategoryId == categoryId))
            {
                ModelState.AddModelError("", "Invalid category!");
            }

            // Validate Title 
            if (string.IsNullOrWhiteSpace(bookEdition.BookTitle?.Title))
            {
                ModelState.AddModelError("", "Book title is required!");
            }
            bool titleExist = db.BookEditions.Any(b => b.BookTitle.Title.ToLower() == bookEdition.BookTitle.Title.Trim().ToLower());
            bool authorNameExist = db.BookEditions.Any(b => b.BookTitle.Author.AuthorName.ToLower() == bookEdition.BookTitle.Author.AuthorName.Trim().ToLower());
            if(titleExist && authorNameExist)
            {
                ModelState.AddModelError("", "Book is exist");
            }
            var authorName = bookEdition.BookTitle.Author.AuthorName.Trim();
            var author = db.Authors.FirstOrDefault(a => a.AuthorName.ToLower() == authorName.ToLower());
            if (author == null)
            {
                author = new Author { AuthorName = authorName };
                db.Authors.Add(author);
                db.SaveChanges();
            }
            var publisherName = bookEdition.BookTitle.Publisher.PublisherName.Trim();
            var publisher = db.Publishers.FirstOrDefault(p => p.PublisherName.ToLower() == publisherName.ToLower());

            if (publisher == null)
            {
                publisher = new Frontend_12102025.Models.Publisher
                {
                    PublisherName = publisherName
                };
                db.Publishers.Add(publisher);
                db.SaveChanges();
            }
            if (ModelState.IsValid)
            {
                // 1. Tạo BookTitle trước
                var bookTitle = new BookTitle
                {
                    Title = bookEdition.BookTitle.Title,
                    Description = bookEdition.BookTitle.Description,
                    AuthorId = author.AuthorId,
                    PublisherId = publisher.PublisherId,
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
                    PublishDate = bookEdition.PublishDate,
                    CoverImage = ImageUrl
                };

                db.BookEditions.Add(newBookEdition);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu lỗi, load lại dropdown
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
            // Validate IBSN
            if (!string.IsNullOrEmpty(bookEdition.ISBN))
            {
                bool ISBNIsExist = db.BookEditions.Any(e => e.ISBN == bookEdition.ISBN && e.BookEditionId != bookEdition.BookEditionId);
                if (ISBNIsExist)
                {
                    ModelState.AddModelError("IBSN", "IBSN đã tồn tại");
                }
            }
            // Validate FK
            if (!db.Authors.Any(a => a.AuthorId == AuthorID))
            {
                ModelState.AddModelError("", "Author không hợp lệ ( chưa được lưu vào database)");
            }
            if (!db.Publishers.Any(p => p.PublisherId == PublisherID))
            {
                ModelState.AddModelError("", "Publisher không hợp lệ ( chưa được lưu vào database)");
            }

            if (!db.Categories.Any(c => c.CategoryId == CategoryID))
            {
                ModelState.AddModelError("", "Category không hợp lệ ( chưa được lưu vào database)");
            }
            // Validate Price va Stock
            if (bookEdition.Price < 0)
            {
                ModelState.AddModelError("", "Giá phải lớn hơn không");
            }
            if (bookEdition.Stock < 0)
            {
                ModelState.AddModelError("", "Số hàng tồn phải lớn hơn không");
            }
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
                BookTitle bookTitle = db.BookTitles.Find(id);
                if (bookTitle == null)
                {
                    return HttpNotFound();
                }
                var bookEditions = db.BookEditions
                    .Where(e => e.BookTitleId == id)
                    .ToList();
                var editionIds = bookEditions.Select(e => e.BookEditionId).ToList();
                bool hasOrders = db.OrderDetails
                                .Any(od => editionIds.Contains(od.BookEditionId));
                // Ktra xem sach co trong order ko
                if (hasOrders)
                {
                    TempData["ErrorMessage"] = "Cannot delete this book because it has been ordered by customers!";
                    return RedirectToAction("Index");
                }

                foreach (var edition in bookEditions)
                {
                    var bookImages = db.BookImages
                        .Where(img => img.BookEditionId == edition.BookEditionId)
                        .ToList();
                    db.BookImages.RemoveRange(bookImages);
                }

                db.BookEditions.RemoveRange(bookEditions);
                db.BookTitles.Remove(bookTitle);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Book deleted successfully!";
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