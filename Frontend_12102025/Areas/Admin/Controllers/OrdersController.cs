using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Frontend_12102025.Models;

namespace Frontend_12102025.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private dbprojectltwEntities db = new dbprojectltwEntities();

        // GET: Admin/Orders
        public async Task<ActionResult> Index()
        {
            var orders = db.Orders.Include(o => o.Coupon).Include(o => o.ShippingAddress).Include(o => o.User);
            return View(await orders.ToListAsync());
        }

        // GET: Admin/Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var order = await db.Orders
                    .Include(o => o.OrderDetails)
                    .Include(o => o.OrderDetails.Select(oi => oi.BookEdition))
                    .Include(o => o.User)
                    .Include(o => o.ShippingAddress)
                    .Include(o => o.Coupon)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        // GET: Admin/Orders/Create
        public ActionResult Create()
        {
            ViewBag.CouponId = new SelectList(db.Coupons, "CouponId", "Code");
            ViewBag.ShippingAddressId = new SelectList(db.ShippingAddresses, "AddressId", "RecipientName");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username");
            return View();
        }

        // POST: Admin/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OrderId,UserId,OrderDate,TotalAmount,PaymentMethod,PaymentStatus,ShippingAddressId,CouponId,OrderStatus")] Order order)
        {
            // Validate user
            var userExists = await db.Users.AnyAsync(u => u.UserId == order.UserId);
            if (!userExists)
            {
                ModelState.AddModelError("UserId", "User does not exist!");
            }
            // Validate payment va order status
            var validOrderStatuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
            if (!string.IsNullOrEmpty(order.OrderStatus) && !validOrderStatuses.Contains(order.OrderStatus))
            {
                ModelState.AddModelError("OrderStatus", "Invalid order status!");
            }

            var validPaymentStatuses = new[] { "Pending", "Paid", "Failed", "Refunded" };
            if (!string.IsNullOrEmpty(order.PaymentStatus) && !validPaymentStatuses.Contains(order.PaymentStatus))
            {
                ModelState.AddModelError("PaymentStatus", "Invalid payment status!");
            }
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CouponId = new SelectList(db.Coupons, "CouponId", "Code", order.CouponId);
            ViewBag.ShippingAddressId = new SelectList(db.ShippingAddresses, "AddressId", "RecipientName", order.ShippingAddressId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", order.UserId);
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders
                                .Include(o => o.User)
                                .Include(o => o.ShippingAddress)
                                .Include(o => o.Coupon)
                                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CouponId = new SelectList(db.Coupons, "CouponId", "Code", order.CouponId);
            ViewBag.ShippingAddressId = new SelectList(db.ShippingAddresses, "AddressId", "RecipientName", order.ShippingAddressId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", order.UserId);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OrderId,UserId,OrderDate,TotalAmount,PaymentMethod,PaymentStatus,ShippingAddressId,CouponId,OrderStatus")] Order order)
        {
            System.IO.File.AppendAllText(@"C:\temp\debug.txt", $"POST Edit called - OrderId: {order.OrderId}\n");
            var orderExist = await db.Orders.FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
            if (orderExist == null)
            {
                return HttpNotFound();
            }
            // Khong edit don hang cancelled
            if(orderExist.OrderStatus.ToLower() == "cancelled")
            {
                TempData["ErrorMessage"] = "Không thể edit đơn hàng đã hủy";
                return RedirectToAction("Index");
            }

            // Khong edit don da giao hang roi
            if (orderExist.OrderStatus.ToLower() == "delivered")
            {
                TempData["ErrorMessage"] = "Không thể edit đơn hàng đã giao";
                return RedirectToAction("Index");
            }

            if(!ModelState.IsValid)
            { 

                ViewBag.CouponId = new SelectList(db.Coupons, "CouponId", "Code", order.CouponId);
                ViewBag.ShippingAddressId = new SelectList(db.ShippingAddresses, "AddressId", "RecipientName", order.ShippingAddressId);
                ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", order.UserId);
                return View(order);
            }
            orderExist.PaymentMethod = order.PaymentMethod;
            orderExist.PaymentStatus = order.PaymentStatus;
            orderExist.OrderStatus = order.OrderStatus;
            orderExist.CouponId = order.CouponId;

            
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Admin/Orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Order order = await db.Orders.FindAsync(id);
                if (order == null)
                {
                    return HttpNotFound();
                }

                if (order.PaymentStatus == "Paid")
                {
                    TempData["ErrorMessage"] = "Không thể xóa đơn hàng đã thanh toán";
                    return RedirectToAction("Index");
                }

                if (order.OrderStatus == "Delivered")
                {
                    TempData["ErrorMessage"] = "Không thể xóa đơn hàng đã giao";
                    return RedirectToAction("Index");
                }

                if (order.OrderStatus == "Shipped")
                {
                    TempData["ErrorMessage"] = "Không thể xóa đơn hàng đang được giao";
                    return RedirectToAction("Index");
                }

                // Xóa OrderDetails trước (do FK constraint)
                var orderDetails = db.OrderDetails.Where(od => od.OrderId == id);
                db.OrderDetails.RemoveRange(orderDetails);

                // Hoàn lại stock nếu đơn chưa bị hủy
                if (order.OrderStatus != "Cancelled")
                {
                    foreach (var detail in orderDetails.ToList())
                    {
                        var bookEdition = await db.BookEditions.FindAsync(detail.BookEditionId);
                        if (bookEdition != null)
                        {
                            bookEdition.Stock += detail.Quantity;
                        }
                    }
                }

                db.Orders.Remove(order);
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Order deleted successfully!";
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
