using System.Web.Mvc;

namespace Frontend_12102025.Areas.Customer
{
    public class CustomerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Customer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //context.MapRoute(
            //    "Customer_default",
            //    "Customer/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);


            // Route cho Account trong Area - URL khác
            context.MapRoute(
                "Customer_Account",
                "Customer/Account/{action}/{id}",
                new { controller = "Account", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Frontend_12102025.Areas.Customer.Controllers" }
            );

            // Default route cho Area Customer
            context.MapRoute(
                "Customer_default",
                "Customer/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Frontend_12102025.Areas.Customer.Controllers" }
            );
        }
    }
}