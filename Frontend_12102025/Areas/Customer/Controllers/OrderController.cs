using Frontend_12102025.Models;
using Frontend_12102025.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Frontend_12102025.Areas.Customer.Controllers
{
    public class OrderController : Controller
    {
        private dbprojectltwEntities db = new dbprojectltwEntities();

        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        // GET: Customer/Order/Checkout
        [Authorize]
        public ActionResult Checkout()
        {
            var cartService = GetCartService();
            var cart = cartService.GetCart();
            var cartItems = cart.Items.ToList();

            if (!cartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng trống";
                return RedirectToAction("Index", "Cart");
            }

            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách địa chỉ đã lưu
            var savedAddresses = db.ShippingAddresses
                .Where(a => a.UserId == user.UserId)
                .Select(a => new ShippingAddressVM
                {
                    AddressId = a.AddressId,
                    RecipientName = a.RecipientName,
                    AddressLine = a.AddressLine,
                    Phone = a.Phone,
                    IsDefault = a.IsDefault ?? false
                })
                .OrderByDescending(a => a.IsDefault)
                .ToList();

            var model = new CheckoutVM
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                CartItems = cartItems,
                SubTotal = cart.TotalValue(),
                ShippingFee = CalculateShippingFee(cart.TotalValue()),
                DiscountAmount = 0,
                OrderDate = DateTime.Now,
                SavedAddresses = savedAddresses,
                NewAddress = new ShippingAddressVM(),
                UseNewAddress = !savedAddresses.Any() // Mặc định dùng địa chỉ mới nếu chưa có
            };

            model.TotalAmount = model.SubTotal + model.ShippingFee - model.DiscountAmount;

            return View(model);
        }

        // POST: Customer/Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Checkout(CheckoutVM model)
        {
            // Validate address
            if (model.UseNewAddress)
            {
                if (string.IsNullOrEmpty(model.NewAddress?.RecipientName) ||
                    string.IsNullOrEmpty(model.NewAddress?.AddressLine) ||
                    string.IsNullOrEmpty(model.NewAddress?.Phone))
                {
                    ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin địa chỉ mới");
                }
            }
            else if (!model.SelectedAddressId.HasValue)
            {
                ModelState.AddModelError("", "Vui lòng chọn địa chỉ giao hàng");
            }

            if (!ModelState.IsValid)
            {
                // Reload data
                var cartService = GetCartService();
                var cart = cartService.GetCart();
                model.CartItems = cart.Items.ToList();
                model.SubTotal = cart.TotalValue();
                model.ShippingFee = CalculateShippingFee(model.SubTotal);
                model.TotalAmount = model.SubTotal + model.ShippingFee - model.DiscountAmount;

                var user = db.Users.Find(model.UserId);
                model.SavedAddresses = db.ShippingAddresses
                    .Where(a => a.UserId == model.UserId)
                    .Select(a => new ShippingAddressVM
                    {
                        AddressId = a.AddressId,
                        RecipientName = a.RecipientName,
                        AddressLine = a.AddressLine,
                        Phone = a.Phone,
                        IsDefault = a.IsDefault ?? false
                    })
                    .OrderByDescending(a => a.IsDefault)
                    .ToList();

                return View(model);
            }

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var cartService = GetCartService();
                    var cart = cartService.GetCart();

                    if (cart == null || !cart.Items.Any())
                    {
                        TempData["Message"] = "Giỏ hàng trống";
                        return RedirectToAction("Index", "Cart");
                    }

                    // 1. Xử lý địa chỉ giao hàng
                    int shippingAddressId;

                    if (model.UseNewAddress)
                    {
                        var newAddress = new ShippingAddress
                        {
                            UserId = model.UserId,
                            RecipientName = model.NewAddress.RecipientName,
                            AddressLine = model.NewAddress.AddressLine,
                            Phone = model.NewAddress.Phone,
                            IsDefault = model.NewAddress.IsDefault
                        };

                        // Nếu đặt làm mặc định, bỏ mặc định các địa chỉ khác
                        if (newAddress.IsDefault == true)
                        {
                            var otherAddresses = db.ShippingAddresses
                                .Where(a => a.UserId == model.UserId);
                            foreach (var addr in otherAddresses)
                            {
                                addr.IsDefault = false;
                            }
                        }

                        db.ShippingAddresses.Add(newAddress);
                        db.SaveChanges();
                        shippingAddressId = newAddress.AddressId;
                    }
                    else
                    {
                        shippingAddressId = model.SelectedAddressId.Value;
                    }

                    // 2. Kiểm tra và áp dụng coupon
                    int? couponId = null;
                    decimal discountValue = 0;

                    if (!string.IsNullOrEmpty(model.CouponCode))
                    {
                        var coupon = db.Coupons
                            .FirstOrDefault(c => c.Code == model.CouponCode
                                && c.StartDate <= DateTime.Now
                                && c.EndDate >= DateTime.Now);

                        if (coupon != null)
                        {
                            couponId = coupon.CouponId;
                            discountValue = coupon.DiscountValue;
                        }
                    }

                    // 3. Tính tổng tiền
                    decimal subTotal = cart.TotalValue();
                    decimal shippingFee = CalculateShippingFee(subTotal);
                    decimal totalAmount = subTotal + shippingFee - discountValue;

                    // 4. Tạo Order
                    var order = new Order
                    {
                        UserId = model.UserId,
                        OrderDate = DateTime.Now,
                        TotalAmount = totalAmount,
                        PaymentMethod = model.PaymentMethod,
                        PaymentStatus = "Pending",
                        ShippingAddressId = shippingAddressId,
                        CouponId = couponId,
                        OrderStatus = "Pending"
                    };

                    db.Orders.Add(order);
                    db.SaveChanges(); // Save để lấy OrderId

                    // 5. Tạo OrderDetails và cập nhật Stock
                    foreach (var item in cart.Items)
                    {
                        var bookEdition = db.BookEditions.Find(item.BookEditionId);

                        if (bookEdition == null)
                        {
                            throw new Exception($"Không tìm thấy sách: {item.Title}");
                        }

                        if (bookEdition.Stock < item.Quantity)
                        {
                            throw new Exception($"Sách '{item.Title}' không đủ hàng. Còn lại: {bookEdition.Stock}");
                        }

                        // Tạo OrderDetail
                        var orderDetail = new OrderDetail
                        {
                            OrderId = order.OrderId,
                            BookEditionId = item.BookEditionId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice
                        };
                        db.OrderDetails.Add(orderDetail);

                        // Giảm stock
                        bookEdition.Stock -= item.Quantity;
                        db.Entry(bookEdition).State = EntityState.Modified;
                    }

                    // 6. Cập nhật thông tin Customer
                    var customer = db.Customers.FirstOrDefault(c => c.UserId == model.UserId);
                    if (customer != null)
                    {
                        customer.TotalOrders = (customer.TotalOrders ?? 0) + 1;
                        customer.TotalSpent = (customer.TotalSpent ?? 0) + totalAmount;
                        customer.LastOrderDate = DateTime.Now;
                        customer.UpdatedAt = DateTime.Now;

                        // Cập nhật CustomerType dựa trên tổng chi tiêu
                        if (customer.TotalSpent >= 10000000) // 10 triệu
                            customer.CustomerType = "VIP";
                        else if (customer.TotalSpent >= 5000000) // 5 triệu
                            customer.CustomerType = "Regular";
                        else
                            customer.CustomerType = "New";

                        db.Entry(customer).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    // Xóa giỏ hàng
                    cartService.ClearCart();

                    TempData["SuccessMessage"] = "Đặt hàng thành công!";
                    return RedirectToAction("OrderSuccess", new { id = order.OrderId });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    TempData["Message"] = "Có lỗi xảy ra: " + ex.Message;

                    // Reload data
                    var cart = GetCartService().GetCart();
                    model.CartItems = cart.Items.ToList();
                    model.SubTotal = cart.TotalValue();
                    model.ShippingFee = CalculateShippingFee(model.SubTotal);
                    model.TotalAmount = model.SubTotal + model.ShippingFee - model.DiscountAmount;

                    model.SavedAddresses = db.ShippingAddresses
                        .Where(a => a.UserId == model.UserId)
                        .Select(a => new ShippingAddressVM
                        {
                            AddressId = a.AddressId,
                            RecipientName = a.RecipientName,
                            AddressLine = a.AddressLine,
                            Phone = a.Phone,
                            IsDefault = a.IsDefault ?? false
                        })
                        .OrderByDescending(a => a.IsDefault)
                        .ToList();

                    return View(model);
                }
            }
        }

        // POST: Apply Coupon (AJAX)
        [HttpPost]
        public JsonResult ApplyCoupon(string couponCode, decimal subTotal)
        {
            try
            {
                var coupon = db.Coupons
                    .FirstOrDefault(c => c.Code == couponCode
                        && c.StartDate <= DateTime.Now
                        && c.EndDate >= DateTime.Now);

                if (coupon == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Mã giảm giá không hợp lệ hoặc đã hết hạn"
                    });
                }

                decimal shippingFee = CalculateShippingFee(subTotal);
                decimal total = subTotal + shippingFee - coupon.DiscountValue;

                return Json(new
                {
                    success = true,
                    discountValue = coupon.DiscountValue,
                    couponId = coupon.CouponId,
                    totalAmount = total,
                    message = "Áp dụng mã giảm giá thành công!"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Có lỗi xảy ra: " + ex.Message
                });
            }
        }

        // GET: Order Success
        public ActionResult OrderSuccess(int id)
        {
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = db.Orders
                .Include(o => o.OrderDetails.Select(od => od.BookEdition.BookTitle))
                .Include(o => o.OrderDetails.Select(od => od.BookEdition.BookTitle.Author))
                .Include(o => o.ShippingAddress)
                .Include(o => o.User)
                .FirstOrDefault(o => o.OrderId == id && o.UserId == user.UserId);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: My Orders
        [Authorize]
        public ActionResult MyOrders(string orderCode = null, string bookTitle = null)
        {
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = db.Orders.Where(o => o.UserId == user.UserId);

            // Tìm kiếm theo mã đơn
            if (!string.IsNullOrEmpty(orderCode))
            {
                query = query.Where(o => o.OrderId.ToString().Contains(orderCode));
            }

            // Tìm kiếm theo tên sách
            if (!string.IsNullOrEmpty(bookTitle))
            {
                query = query.Where(o => o.OrderDetails.Any(od =>
                    od.BookEdition.BookTitle.Title.Contains(bookTitle)));
            }

            var orders = query
                .Include(o => o.OrderDetails.Select(od => od.BookEdition.BookTitle))
                .Include(o => o.ShippingAddress)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        // GET: Order Detail
        [Authorize]
        public ActionResult OrderDetail(int id)
        {
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = db.Orders
                .Include(o => o.OrderDetails.Select(od => od.BookEdition))
                .Include(o => o.OrderDetails.Select(od => od.BookEdition.BookTitle))
                .Include(o => o.OrderDetails.Select(od => od.BookEdition.BookTitle.Author))
                .Include(o => o.ShippingAddress)
                .Include(o => o.Coupon)
                .FirstOrDefault(o => o.OrderId == id && o.UserId == user.UserId);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // POST: Reorder
        [HttpPost]
        [Authorize]
        public ActionResult Reorder(int id)
        {
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = db.Orders
                .Include(o => o.OrderDetails.Select(od => od.BookEdition))
                .Include(o => o.OrderDetails.Select(od => od.BookEdition.BookTitle))
                .Include(o => o.OrderDetails.Select(od => od.BookEdition.BookTitle.Author))
                .FirstOrDefault(o => o.OrderId == id && o.UserId == user.UserId);

            if (order == null)
            {
                return HttpNotFound();
            }

            var cartService = GetCartService();
            var cart = cartService.GetCart();

            foreach (var item in order.OrderDetails)
            {
                var bookEdition = item.BookEdition;
                if (bookEdition != null && bookEdition.Stock > 0)
                {
                    int quantityToAdd = Math.Min(item.Quantity, bookEdition.Stock);

                    cart.AddItem(
                        bookEdition.BookEditionId,
                        bookEdition.CoverImage,
                        bookEdition.BookTitle.Title,
                        bookEdition.BookTitle.Author.AuthorName,
                        bookEdition.ISBN,
                        bookEdition.Price,
                        quantityToAdd,
                        bookEdition.Stock
                    );
                }
            }

            cartService.SaveCart(cart);
            TempData["SuccessMessage"] = "Đã thêm lại sản phẩm vào giỏ hàng!";
            return RedirectToAction("Index", "Cart");
        }

        // Helper: Calculate Shipping Fee
        private decimal CalculateShippingFee(decimal subTotal)
        {
            if (subTotal >= 500000) return 0; // Miễn phí ship từ 500k
            if (subTotal >= 300000) return 20000; // 20k cho đơn 300k-500k
            return 30000; // 30k cho đơn dưới 300k
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
