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
    public class CustomersController : Controller
    {
        private dbprojectltwEntities db = new dbprojectltwEntities();

        // GET: Admin/Customers
        public async Task<ActionResult> Index()
        {
            var customers = db.Customers.Include(c => c.User);
            return View(await customers.ToListAsync());
        }

        // GET: Admin/Customers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Frontend_12102025.Models.Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Admin/Customers/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username");
            return View();
        }

        // POST: Admin/Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CustomerId,UserId,DateOfBirth,Gender,CustomerType,TotalOrders,TotalSpent,LastOrderDate,CustomerStatus,Notes,CreatedAt,UpdatedAt")] Frontend_12102025.Models.Customer customer)
        {
            // Validate User
            var userExist = await db.Users.AnyAsync(u => u.UserId == customer.UserId);
            if (!userExist)
            {
                ModelState.AddModelError("UserId", "User khong ton tai");
            }
            // Validate age
            if (customer.DateOfBirth.HasValue)
            {
                if (customer.DateOfBirth > DateTime.Now)
                {
                    ModelState.AddModelError("DateOfBirth", "Date of birth cannot be in the future!");
                }
                if (customer.DateOfBirth < DateTime.Now.AddYears(-120))
                {
                    ModelState.AddModelError("DateOfBirth", "Invalid date of birth!");
                }
            }

            if (ModelState.IsValid)
            {
                customer.CreatedAt = DateTime.Now;
                customer.UpdatedAt = DateTime.Now;
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", customer.UserId);
            return View(customer);
        }

        // GET: Admin/Customers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Frontend_12102025.Models.Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", customer.UserId);
            return View(customer);
        }

        // POST: Admin/Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CustomerId,UserId,DateOfBirth,Gender,CustomerType,TotalOrders,TotalSpent,LastOrderDate,CustomerStatus,Notes,CreatedAt,UpdatedAt")] Frontend_12102025.Models.Customer customer)
        {
            var userExist = await db.Customers.AnyAsync(u => u.UserId == customer.UserId && u.CustomerId != customer.CustomerId);
            if (userExist)
            {
                ModelState.AddModelError("", "User da ton tai");
            }
            if(customer.DateOfBirth.HasValue || customer.DateOfBirth > DateTime.Now)
            {
                ModelState.AddModelError("DateOfBirth", "Ngay thang nam sinh khong hop le");
            }
            if (ModelState.IsValid)
            {
                customer.UpdatedAt = DateTime.Now;
                db.Entry(customer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", customer.UserId);
            return View(customer);
        }

        // GET: Admin/Customers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Frontend_12102025.Models.Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                Frontend_12102025.Models.Customer customer = await db.Customers.FindAsync(id);
                if (customer == null)
                {
                    return HttpNotFound();
                }

                bool hasOrders = await db.OrderDetails.AnyAsync(o => o.OrderId == id);
                if (hasOrders)
                {
                    TempData["ErrorMessage"] = "Cannot delete this customer because they have orders!";
                    return RedirectToAction("Index");
                }

                db.Customers.Remove(customer);
                await db.SaveChangesAsync();
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
