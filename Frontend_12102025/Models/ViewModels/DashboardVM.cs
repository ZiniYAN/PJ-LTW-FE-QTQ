using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frontend_12102025.Models.ViewModels
{
    public class DashboardVM
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<Order> LastOrders { get; set; }
    }
}