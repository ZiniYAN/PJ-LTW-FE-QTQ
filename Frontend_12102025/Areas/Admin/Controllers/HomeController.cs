using Frontend_12102025.Models;
using Frontend_12102025.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frontend_12102025.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        dbprojectltwEntities db = new dbprojectltwEntities();
        // GET: Admin/Home
        public ActionResult Index()
        {
            DashboardVM dashboard = new DashboardVM();
            dashboard.TotalUsers = db.Users.Count();
            dashboard.TotalProducts = db.BookEditions.Count();
            dashboard.TotalOrders= db.Orders.Count();
            dashboard.TotalRevenue = db.Orders
                .Where(o => o.PaymentStatus != null &&
                           o.PaymentStatus.ToLower() == "paid")
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;
            dashboard.LastOrders = db.Orders.OrderByDescending(o => o.OrderDate).Take(5).ToList();
            return View(dashboard);
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