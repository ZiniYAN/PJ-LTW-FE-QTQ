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
        //Query san pham tu string theo URL
        public ActionResult Index(string SearchTerm, int? page)
        {
            var model = new HomeProductVM();
            model.SearchTerm = SearchTerm;
            // Feature Products
            model.FeatureProducts = db.BookEditions
                .Where(b => b.Stock >= 0) // Loc san pham 
                .OrderByDescending(b => b.OrderDetails.Count) //Sap xep giam dan theo sl da ban
                .Take(10) //Lay 10 sp dau -> list
                .ToList(); //Tra ve List<BookEdition>

            // New Products với phân trang
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            
            //Dung Include() : Eager Loading. Load cac vat the co Include trong 1 query
            var newProductsQuery = db.BookEditions
                .Include(b => b.BookTitle)
                .Include(b => b.BookTitle.Category)
                .Include(b => b.BookTitle.Author)
                .Where(b => b.Stock >= 0);

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                newProductsQuery = newProductsQuery
                    .Where(b => b.BookTitle.Title.Contains(SearchTerm) ||
                               b.BookTitle.Description.Contains(SearchTerm) ||
                               b.BookTitle.Category.CategoryName.Contains(SearchTerm) ||
                               b.BookTitle.Author.AuthorName.Contains(SearchTerm));
            }

            //Dung lib PagedList phan trang
            model.NewProducts = newProductsQuery
                .OrderByDescending(b => b.PublishDate)
                .ToPagedList(pageNumber, pageSize);


            // San pham theo Category. Goi trong controller de truyen data vao hien thi trong View
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

            // Eager loading: Load BookEdition voi dieu kien FirstOrDefault Id = id
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

            // Lay san pham lien quan ( lay theo category )
            
            var products = db.BookEditions
                .Include(b => b.BookTitle)
                .Include(b => b.BookTitle.Category)
                .Include(b => b.OrderDetails)
                //Loai tru trung lap
                .Where(p => p.BookTitle.CategoryId == pro.BookTitle.CategoryId && p.BookEditionId != pro.BookEditionId)
                .AsQueryable();

            ProductDetailVM model = new ProductDetailVM();

            // Phân trang
            int pageNumber = page ?? 1;
            int pageSize = model.PageSize;

            // Set quantity 1
            int productQuantity = quantity ?? 1;

            // Validate quantity
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
            //Da loai bo
            // TopProducts: Lấy top sản phẩm bán chạy, CÓ phân trang
            model.TopProducts = products
                .OrderByDescending(p => p.OrderDetails.Count)
                .ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        public ActionResult ContactUs()
        {
            return View();
        }
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
        public ActionResult ReturnPolicy()
        {
            return View();
        }
        public ActionResult Shipping()
        {
            return View();
        }

    }
}