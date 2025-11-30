using Frontend_12102025.Models;
using Frontend_12102025.Models.ViewModels;
using Frontend_12102025.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Frontend_12102025.Areas.Admin.Controllers
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
        public async Task<ActionResult> Create(UserVM user)
        {
            // Pass
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.AddModelError("Password", "Password khong the trong");
            }
            if(!string.IsNullOrWhiteSpace(user.Password) && string.IsNullOrWhiteSpace(user.ConfirmPassword))
            {
                ModelState.AddModelError("ConfirmPasswod", "Mat khau khong trung khop");
            }
            // Username
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
            // Email
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var emailExists = await db.Users
                    .AnyAsync(u => u.Email.ToLower() == user.Email.Trim().ToLower());
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                }
            }
            // Sdt
            if (!string.IsNullOrWhiteSpace(user.Phone))
            {
                var phoneExists = await db.Users
                    .AnyAsync(u => u.Phone == user.Phone.Trim());
                if (phoneExists)
                {
                    ModelState.AddModelError("Phone", "Số điện thoại đã tồn tại");
                }
            }
            // Validate passwod
            if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    Username = user.Username.Trim(),
                    FullName = user.FullName.Trim(),
                    Email = user.Email.Trim(),
                    Phone = user.Phone?.Trim(),
                    UserRole = user.UserRole,
                    PasswordHash = HashPassword(user.Password),
                    CreatedAt = DateTime.Now
                };
                user.CreatedAt = DateTime.Now;
                db.Users.Add(newUser);
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
            var model = new UserVM
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                UserRole = user.UserRole,
                CreatedAt = user.CreatedAt
                // Password và ConfirmPassword để null (không load từ DB)
            };
            return View(model);
        }

        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserVM user)
        {
            var existingUser = await db.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);

            if (existingUser == null)
            {
                return HttpNotFound();
            }
            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                var usernameExists = await db.Users.AnyAsync(u => u.Username.ToLower() == user.Username.Trim().ToLower() && u.UserId != user.UserId);
                if (usernameExists)
                {
                    ModelState.AddModelError("Username", "Username đã tồn tại");
                }
            }
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var emailExists = await db.Users.AnyAsync(u => u.Email.ToLower() == user.Email.Trim().ToLower() && u.UserId != user.UserId);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email đã tồn tại");
                }
            }
            if (!string.IsNullOrWhiteSpace(user.Phone))
            {
                existingUser.Username = user.Username.Trim();
                existingUser.FullName = user.FullName.Trim();
                existingUser.Email = user.Email.Trim();
                existingUser.Phone = user.Phone?.Trim();
                existingUser.UserRole = user.UserRole;
                var phoneExists = await db.Users
                    .AnyAsync(u => u.Phone == user.Phone.Trim() && u.UserId != user.UserId);
                if (phoneExists)
                {
                    ModelState.AddModelError("Phone", "Phone number already exists!");
                }
            }
            if (ModelState.IsValid)
            {
                db.Entry(existingUser).State = EntityState.Modified;
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

                // Khong the xoa admin cuoi
                if (user.UserRole == "Admin")
                {
                    var adminCount = await db.Users.CountAsync(u => u.UserRole == "Admin");
                    if (adminCount <= 1)
                    {
                        TempData["ErrorMessage"] = "Không thể xóa tài khoản admin cuối cùng!";
                        return RedirectToAction("Index");
                    }
                }

                // Validate khong cho xoa user neu dang co order
                var hasOrders = await db.Orders.AnyAsync(o => o.UserId == id);
                if (hasOrders)
                {
                    TempData["ErrorMessage"] = "Không thể xóa người dùng có đơn hàng!";
                    return RedirectToAction("Index");
                }

                // Xoa cac phan lien quan neu la customer
                var customer = await db.Customers.FirstOrDefaultAsync(c => c.UserId == id);
                if (customer != null)
                {
                    db.Customers.Remove(customer);
                }

                var addresses = db.ShippingAddresses.Where(a => a.UserId == id);
                db.ShippingAddresses.RemoveRange(addresses);

                // Xoa user
                db.Users.Remove(user);
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Xóa người dùng thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
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
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
