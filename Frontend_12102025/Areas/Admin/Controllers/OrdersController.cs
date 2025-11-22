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
            Order order = await db.Orders.FindAsync(id);
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
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CouponId = new SelectList(db.Coupons, "CouponId", "Code", order.CouponId);
            ViewBag.ShippingAddressId = new SelectList(db.ShippingAddresses, "AddressId", "RecipientName", order.ShippingAddressId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", order.UserId);
            return View(order);
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
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
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
