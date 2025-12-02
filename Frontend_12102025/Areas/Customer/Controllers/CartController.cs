using Frontend_12102025.Models;
using Frontend_12102025.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
namespace Frontend_12102025.Areas.Customer.Controllers
{
    public class CartController : Controller
    {
        // GET: Customer/Cart
        private dbprojectltwEntities db = new dbprojectltwEntities();
        //private CartService cartService;

        // Hiển thị giỏ hàng
        public ActionResult Index()
        {
            var cartService = new CartService(this.Session);
            var cart = cartService.GetCart();
            return View(cart);
        }

        // Thêm sách vào giỏ
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var bookEdition = db.BookEditions
                .Include("BookTitle")
                .Include("BookTitle.Author")
                .FirstOrDefault(b => b.BookEditionId == id);

            if (bookEdition == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra tồn kho
            if (bookEdition.Stock < quantity)
            {
                TempData["ErrorMessage"] = "Số lượng sách trong kho không đủ.";
                return RedirectToAction("Details", "Book", new { id = id });
            }
            var cartService = new CartService(this.Session);
            var cart = cartService.GetCart();
            cart.AddItem(
                bookEdition.BookEditionId,
                bookEdition.CoverImage,
                bookEdition.BookTitle.Title,
                bookEdition.BookTitle.Author.AuthorName,
                bookEdition.ISBN,
                bookEdition.Price,
                quantity,
                bookEdition.Stock
            );
            //Luu vao session
            cartService.SaveCart(cart);
            TempData["SuccessMessage"] = "Đã thêm sách vào giỏ hàng.";
            return RedirectToAction("Index");
        }

        //Mua ngay -> Checkout
        [Authorize]
        public ActionResult BuyNow(int id, int quantity = 1)
        {
            if (id <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var book = db.BookEditions
                .Include("BookTitle")
                .Include("BookTitle.Author")
                .FirstOrDefault(b => b.BookEditionId == id);

            if (book == null)
            {
                return HttpNotFound();
            }

            if (book.Stock < quantity)
            {
                TempData["ErrorMessage"] = "Số lượng sách trong kho không đủ.";
                return RedirectToAction("ProductDetail", "Home", new { id = id });
            }

            var cartService = new CartService(this.Session);
            var cart = cartService.GetCart();

            cart.AddItem(
                book.BookEditionId,
                book.CoverImage,
                book.BookTitle.Title,
                book.BookTitle.Author.AuthorName,
                book.ISBN,
                book.Price,
                quantity,
                book.Stock
            );

            cartService.SaveCart(cart);

            return RedirectToAction("Checkout", "Order");
        }
        // Cập nhật số lượng
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            if (quantity < 1)
            {
                return RedirectToAction("Index");
            }
            var cartService = new CartService(this.Session);
            var cart = cartService.GetCart();
            cart.UpdateQuantity(id, quantity);
            cartService.SaveCart(cart);

            return RedirectToAction("Index");
        }

        // Xóa sách khỏi giỏ
        public ActionResult RemoveFromCart(int id)
        {
            var cartService = new CartService(this.Session);
            var cart = cartService.GetCart();
            cart.RemoveItem(id);
            cartService.SaveCart(cart);

            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ hàng
        public ActionResult ClearCart()
        {
            var cartService = new CartService(this.Session);
            cartService.ClearCart();
            return RedirectToAction("Index");
        }

        // API lấy số lượng sách trong giỏ (dùng cho hiển thị badge)
        [HttpGet]
        //Tra ve JSON thay vi HTML view
        //Vi chi can JSON so luog thoi, hien thi tren icon gio hang sau khi ket hop js
        public JsonResult GetCartCount()
        {
            var cartService = new CartService(this.Session);
            int count = cartService.GetCartItemCount();
            return Json(new { count = count }, JsonRequestBehavior.AllowGet);
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