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

namespace Frontend_12102025.Areas.Admin
{
    public class UsersController : Controller
    {
        private dbprojectltwEntities db = new dbprojectltwEntities();

        // GET: Admin/Users
        public async Task<ActionResult> Index()
        {
            return View(await db.Users.ToListAsync());
        }

        // GET: Admin/Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Admin/Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "UserId,Username,PasswordHash,FullName,Email,Phone,UserRole,CreatedAt")] User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                ModelState.AddModelError("Username", "Username khong the trong");
            }
            else
            {
                var usernameExists = await db.Users
                    .AnyAsync(u => u.Username.ToLower() == user.Username.Trim().ToLower());
                if (usernameExists)
                {
                    ModelState.AddModelError("Username", "Username da ton tai");
                }
            }
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                ModelState.AddModelError("Email", "Email is required!");
            }
            else
            {
                var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(user.Email))
                {
                    ModelState.AddModelError("Email", "Invalid email format!");
                }

                var emailExists = await db.Users
                    .AnyAsync(u => u.Email.ToLower() == user.Email.Trim().ToLower());
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email already exists!");
                }
            }

            if (!string.IsNullOrWhiteSpace(user.Phone))
            {
                var phoneRegex = new System.Text.RegularExpressions.Regex(@"^[0-9]{10,11}$");
                if (!phoneRegex.IsMatch(user.Phone))
                {
                    ModelState.AddModelError("Phone", "Phone must be 10-11 digits!");
                }

                var phoneExists = await db.Users
                    .AnyAsync(u => u.Phone == user.Phone.Trim());
                if (phoneExists)
                {
                    ModelState.AddModelError("Phone", "Phone number already exists!");
                }
            }
            // Validate passwod
            if (ModelState.IsValid)
            {
                user.CreatedAt = DateTime.Now;
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Admin/Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UserId,Username,PasswordHash,FullName,Email,Phone,UserRole,CreatedAt")] User user)
        {
            var existingUser = await db.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);

            if (existingUser == null)
            {
                return HttpNotFound();
            }
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                ModelState.AddModelError("Username", "Username is required!");
            }
            else
            {
                var usernameExists = await db.Users
                    .AnyAsync(u => u.Username.ToLower() == user.Username.Trim().ToLower()
                                  && u.UserId != user.UserId);
                if (usernameExists)
                {
                    ModelState.AddModelError("Username", "Username already exists!");
                }
            }
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                ModelState.AddModelError("Email", "Email is required!");
            }
            else
            {
                var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailRegex.IsMatch(user.Email))
                {
                    ModelState.AddModelError("Email", "Invalid email format!");
                }

                var emailExists = await db.Users
                    .AnyAsync(u => u.Email.ToLower() == user.Email.Trim().ToLower()
                                  && u.UserId != user.UserId);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email already exists!");
                }
            }
            if (!string.IsNullOrWhiteSpace(user.Phone))
            {
                var phoneExists = await db.Users
                    .AnyAsync(u => u.Phone == user.Phone.Trim() && u.UserId != user.UserId);
                if (phoneExists)
                {
                    ModelState.AddModelError("Phone", "Phone number already exists!");
                }
            }
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Admin/Users/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                User user = await db.Users.FindAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Không cho xóa chính mình
                // var currentUserId = GetCurrentUserId();
                // if (user.UserId == currentUserId)
                // {
                //     TempData["ErrorMessage"] = "Cannot delete your own account!";
                //     return RedirectToAction("Index");
                // }

                if (user.UserRole == "Admin")
                {
                    var adminCount = await db.Users.CountAsync(u => u.UserRole == "Admin");
                    if (adminCount <= 1)
                    {
                        TempData["ErrorMessage"] = "Cannot delete the last admin account!";
                        return RedirectToAction("Index");
                    }
                }

                var hasOrders = await db.Orders.AnyAsync(o => o.UserId == id);
                if (hasOrders)
                {
                    TempData["ErrorMessage"] = "Cannot delete user with orders! Consider deactivating instead.";
                    return RedirectToAction("Index");
                }

                var customer = await db.Customers.FirstOrDefaultAsync(c => c.UserId == id);
                if (customer != null)
                {
                    db.Customers.Remove(customer);
                }

                var addresses = db.ShippingAddresses.Where(a => a.UserId == id);
                db.ShippingAddresses.RemoveRange(addresses);

                db.Users.Remove(user);
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
