using Frontend_12102025.Models;
using Frontend_12102025.Models.ViewModel;
using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;   
namespace Frontend_12102025.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        // GET: Customer/Home
        private dbprojectltwEntities db = new dbprojectltwEntities();
        public ActionResult Index(string SearchTerm, int? page)
        {
            var model = new HomeProductVM();

            // Feature Products
            model.FeatureProducts = db.BookEditions
                .Where(b => b.Stock >= 0)
                .OrderByDescending(b => b.OrderDetails.Count)
                .Take(10)
                .ToList();

            // New Products với phân trang
            int pageSize = 12;
            int pageNumber = (page ?? 1);

            var newProductsQuery = db.BookEditions
                .Where(b => b.Stock >= 0)
                .OrderByDescending(b => b.PublishDate);

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                newProductsQuery = (IOrderedQueryable<BookEdition>)newProductsQuery
                    .Where(b => b.BookTitle.Title.Contains(SearchTerm) ||
                               b.BookTitle.Description.Contains(SearchTerm) ||
                               b.BookTitle.Category.CategoryName.Contains(SearchTerm));
            }

            model.NewProducts = newProductsQuery.ToPagedList(pageNumber, pageSize);
            model.SearchTerm = SearchTerm;

            // Sản phẩm theo từng category
            ViewBag.KhoaHocProducts = db.BookEditions
                .Where(b => b.BookTitle.Category.CategoryName == "Khoa học" && b.Stock >= 0)
                .OrderByDescending(b => b.PublishDate)
                .Take(10)
                .ToList();

            ViewBag.TamLyHocProducts = db.BookEditions
                .Where(b => b.BookTitle.Category.CategoryName == "Tâm lý học" && b.Stock >= 0)
                .OrderByDescending(b => b.PublishDate)
                .Take(10)
                .ToList();

            ViewBag.VanHocProducts = db.BookEditions
                .Where(b => b.BookTitle.Category.CategoryName == "Văn học" && b.Stock >= 0)
                .OrderByDescending(b => b.PublishDate)
                .Take(10)
                .ToList();

            ViewBag.KinhTeProducts = db.BookEditions
                .Where(b => b.BookTitle.Category.CategoryName == "Kinh tế" && b.Stock >= 0)
                .OrderByDescending(b => b.PublishDate)
                .Take(10)
                .ToList();

            ViewBag.SelfHelpProducts = db.BookEditions
                .Where(b => b.BookTitle.Category.CategoryName == "Self-Help" && b.Stock >= 0)
                .OrderByDescending(b => b.PublishDate)
                .Take(10)
                .ToList();

            return View(model);
        }



        public ActionResult ProductDetail(int? id, int? quantity, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Eager loading: Load BookEdition cùng với các navigation properties cần thiết
            BookEdition pro = db.BookEditions
                .Include(b => b.BookTitle)
                .Include(b => b.BookTitle.Category)
                .Include(b => b.BookTitle.Author)
                .Include(b => b.BookTitle.Publisher)
                .Include(b => b.OrderDetails)
                .FirstOrDefault(b => b.BookEditionId == id);

            if (pro == null)
            {
                return HttpNotFound();
            }

            // Query sản phẩm liên quan với eager loading
            var products = db.BookEditions
                .Include(b => b.BookTitle)
                .Include(b => b.BookTitle.Category)
                .Include(b => b.OrderDetails)
                .Where(p => p.BookTitle.CategoryId == pro.BookTitle.CategoryId && p.BookEditionId != pro.BookEditionId)
                .AsQueryable();

            ProductDetailVM model = new ProductDetailVM();

            // Phân trang
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize;

            // Set quantity với giá trị mặc định là 1
            int productQuantity = quantity ?? 1;

            // Đảm bảo quantity luôn >= 1 và <= Stock
            if (productQuantity < 1)
            {
                productQuantity = 1;
            }
            if (productQuantity > pro.Stock)
            {
                productQuantity = pro.Stock;
            }

            model.product = pro;
            model.quantity = productQuantity;
            model.estimatedValue = pro.Price * productQuantity;

            // RelatedProduct: Lấy 8 sản phẩm liên quan, không phân trang
            model.RelatedProduct = products
                .OrderBy(p => p.BookEditionId)
                .Take(8)
                .ToList();

            // TopProducts: Lấy top sản phẩm bán chạy, CÓ phân trang
            model.TopProducts = products
                .OrderByDescending(p => p.OrderDetails.Count)
                .ToPagedList(pageNumber, pageSize);

            return View(model);
        }

    }
}