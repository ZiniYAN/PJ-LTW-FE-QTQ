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
            var Books = db.BookEditions.Include(e => e.BookTitle.Author).Include(e => e.BookTitle.Category).Include(e => e.BookTitle.Publisher);
            return View(Books.ToList());
        }

        // GET: Admin/Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookTitle Book = db.BookTitles.Find(id);
            if (Book == null)
            {
                return HttpNotFound();
            }
            return View(Book);
        }

        // GET: Admin/Books/Create
        public ActionResult Create()
        {
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "AuthorName");
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "PublisherName");
            return View();
        }

        // POST: Admin/Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookID,Title,AuthorID,PublisherID,CategoryID,Price,Stock,ISBN,Description,ImageURL,PublishDate")] BookTitle Book)
        {
            if (ModelState.IsValid)
            {
                db.BookTitles.Add(Book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "AuthorName", Book.AuthorId);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", Book.CategoryId);
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "PublisherName", Book.PublisherId);
            return View(Book);
        }

        // GET: Admin/Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookTitle Book = db.BookTitles.Find(id);
            if (Book == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "AuthorName", Book.AuthorId);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", Book.CategoryId);
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "PublisherName", Book.PublisherId);
            return View(Book);
        }

        // POST: Admin/Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookID,Title,AuthorID,PublisherID,CategoryID,Price,Stock,ISBN,Description,ImageURL,PublishDate")] BookTitle Book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorID", "AuthorName", Book.AuthorId);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", Book.CategoryId);
            ViewBag.PublisherID = new SelectList(db.Publishers, "PublisherID", "PublisherName", Book.PublisherId);
            return View(Book);
        }

        // GET: Admin/Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookTitle Book = db.BookTitles.Find(id);
            if (Book == null)
            {
                return HttpNotFound();
            }
            return View(Book);
        }

        // POST: Admin/Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BookTitle Book = db.BookTitles.Find(id);
            db.BookTitles.Remove(Book);
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
