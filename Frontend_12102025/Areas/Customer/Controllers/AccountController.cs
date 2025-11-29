using Frontend_12102025.Models;
using Frontend_12102025.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace Frontend_12102025.Areas.Customer.Controllers
{
    //Su dung Install-Package BCrypt.Net-Next de co the hash mat khau
    [RouteArea("Customer")]
    public class AccountController : Controller
    {
        private dbprojectltwEntities db = new dbprojectltwEntities();
        // GET: Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            // Nếu đã đăng nhập, redirect về trang chủ
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra username đã tồn tại
                if (db.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                    return View(model);
                }

                // Kiểm tra email đã tồn tại
                if (db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng");
                    return View(model);
                }

                // Kiểm tra số điện thoại đã tồn tại
                if (!string.IsNullOrEmpty(model.Phone) && db.Users.Any(u => u.Phone == model.Phone))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại đã được sử dụng");
                    return View(model);
                }

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Hash mật khẩu bằng BCrypt
                        string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(model.Password, 13);

                        // Tạo User mới
                        var user = new User
                        {
                            Username = model.Username,
                            PasswordHash = passwordHash,
                            FullName = model.FullName,
                            Email = model.Email,
                            Phone = model.Phone,
                            UserRole = "C", // C = Customer, A = Admin
                            CreatedAt = DateTime.Now
                        };

                        db.Users.Add(user);
                        db.SaveChanges();

                        // Tạo Customer record
                        var customer = new Frontend_12102025.Models.Customer
                        {
                            UserId = user.UserId,
                            DateOfBirth = model.DateOfBirth,
                            Gender = model.Gender,
                            CustomerType = "New",
                            TotalOrders = 0,
                            TotalSpent = 0,
                            CustomerStatus = "Active",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        db.Customers.Add(customer);
                        db.SaveChanges();

                        transaction.Commit();

                        TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                        return RedirectToAction("Login");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                        return View(model);
                    }
                }
            }

            return View(model);
        }
        // GET: Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            // Nếu đã đăng nhập, redirect về trang chủ
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                // Tìm user theo username
                var user = db.Users.FirstOrDefault(u => u.Username == model.Username);

                if (user != null)
                {
                    // Verify password bằng BCrypt
                    bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(model.Password, user.PasswordHash);

                    if (isPasswordValid)
                    {
                        // Kiểm tra user có bị block không
                        var customer = db.Customers.FirstOrDefault(c => c.UserId == user.UserId);
                        if (customer != null && customer.CustomerStatus == "Blocked")
                        {
                            ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
                            return View(model);
                        }

                        // Tạo authentication cookie
                        FormsAuthentication.SetAuthCookie(user.Username, model.RememberMe);

                        // Lưu thông tin vào Session (optional)
                        Session["UserId"] = user.UserId;
                        Session["Username"] = user.Username;
                        Session["FullName"] = user.FullName;
                        Session["UserRole"] = user.UserRole;

                        // Redirect
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            // Redirect dựa trên role
                            if (user.UserRole == "A") // Admin
                            {
                                return RedirectToAction("Index", "Home", new { area = "Admin" });
                            }
                            else // Customer
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
            }

            return View(model);
        }


        // GET: Account/ProfileInfo
        [Authorize]
        public ActionResult ProfileInfo()
        {
            // Lấy username hiện tại từ User.Identity
            var username = User.Identity.Name;

            // Tìm User trong database
            // Include("Customers") để lấy luôn thông tin Customer đi kèm (nếu có relation)
            var user = db.Users.Include("Customers").FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                // Trường hợp hiếm: User đã đăng nhập nhưng không tìm thấy trong DB (vd: bị xóa tay)
                FormsAuthentication.SignOut();
                return RedirectToAction("Login");
            }

            // Tìm Customer tương ứng với User này
            var customer = db.Customers.FirstOrDefault(c => c.UserId == user.UserId);

            // Nếu chưa có record Customer (vd: Admin đăng nhập), có thể tạo dummy hoặc handle riêng
            if (customer == null)
            {
                // Tạo tạm model customer để view không bị lỗi null
                customer = new Frontend_12102025.Models.Customer
                {
                    User = user,
                    // Các field khác để default
                };
            }
            else
            {
                // Gán ngược lại User để chắc chắn navigation property có dữ liệu
                customer.User = user;
            }

            return View(customer);
        }

        // GET: Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Account");
        }


        // GET: Account/Index (Trang Edit Profile)
        [Authorize]
        public ActionResult Index()
        {
            var username = User.Identity.Name;

            // Load Customer kèm thông tin User và ShippingAddresses để hiển thị
            // Include("User") để lấy FullName, Email, Phone từ bảng User
            // Include("User.ShippingAddresses") để lấy địa chỉ
            var customer = db.Customers
                             .Include("User")
                             .Include("User.ShippingAddresses")
                             .FirstOrDefault(c => c.User.Username == username);

            if (customer == null)
            {
                return RedirectToAction("Login");
            }

            return View(customer);
        }

        // POST: Account/UpdateContact (Xử lý cập nhật)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateContact(Frontend_12102025.Models.Customer model)
        {
            var username = User.Identity.Name;

            // Lấy record gốc từ DB ra để update (bao gồm cả User để update 2 bảng cùng lúc)
            var customerInDb = db.Customers
                                 .Include("User")
                                 .Include("User.ShippingAddresses")
                                 .FirstOrDefault(c => c.User.Username == username);

            if (customerInDb != null)
            {
                // 1. Update bảng Customer (Ngày sinh, Giới tính)
                customerInDb.DateOfBirth = model.DateOfBirth;
                customerInDb.Gender = model.Gender;
                customerInDb.UpdatedAt = DateTime.Now;

                // 2. Update bảng User (FullName, Phone, Email) - Dữ liệu này form gửi lên qua model.User
                if (model.User != null)
                {
                    customerInDb.User.FullName = model.User.FullName;
                    customerInDb.User.Phone = model.User.Phone;
                    customerInDb.User.Email = model.User.Email;
                }

                // 3. Update Địa chỉ (ShippingAddresses) - Lấy từ form field tên "AddressLine"
                string newAddress = Request.Form["AddressLine"];
                string recipientPhone = Request.Form["RecipientPhone"] ?? customerInDb.User.Phone;
                if (!string.IsNullOrWhiteSpace(newAddress)) // Chỉ update nếu form có gửi trường này
                {
                    var address = customerInDb.User.ShippingAddresses.FirstOrDefault();
                    if (address != null)
                    {
                        address.AddressLine = newAddress;
                        address.Phone = recipientPhone;
                    }
                    else
                    {
                        // Nếu chưa có địa chỉ thì tạo mới
                        db.ShippingAddresses.Add(new ShippingAddress
                        {
                            UserId = customerInDb.UserId,
                            AddressLine = newAddress,
                            Phone = recipientPhone,
                            IsDefault = true
                        });
                    }
                }

                try
                {
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                    return RedirectToAction("ProfileInfo"); // Cập nhật xong chuyển về trang xem Profile
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            // Nếu lỗi thì trả lại view cũ kèm data
            return View("Index", customerInDb);
        }

        // GET: Account/ChangePassword
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Verify old password
            bool isOldPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(model.OldPassword, user.PasswordHash);
            if (!isOldPasswordValid)
            {
                ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng");
                return View(model);
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(model.NewPassword, 13);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index", "Home");
        }

        // AJAX: Check username availability
        [AllowAnonymous]
        public JsonResult CheckUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return Json(new { available = false, message = "Vui lòng nhập tên đăng nhập" }, JsonRequestBehavior.AllowGet);
            }

            // Dùng using để tự động dispose
            using (var db = new dbprojectltwEntities())
            {
                bool exists = db.Users.Any(u => u.Username == username);
                return Json(new
                {
                    available = !exists,
                    message = exists ? "Tên đăng nhập đã tồn tại" : "Tên đăng nhập khả dụng"
                }, JsonRequestBehavior.AllowGet);
            }
        }


        // AJAX: Check email availability
        [AllowAnonymous]
        public JsonResult CheckEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { available = false, message = "Vui lòng nhập email" }, JsonRequestBehavior.AllowGet);
            }

            // Dùng using để tự động dispose
            using (var db = new dbprojectltwEntities())
            {
                bool exists = db.Users.Any(u => u.Email == email);
                return Json(new
                {
                    available = !exists,
                    message = exists ? "Email đã được sử dụng" : "Email khả dụng"
                }, JsonRequestBehavior.AllowGet);
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
