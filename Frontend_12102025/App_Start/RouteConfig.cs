using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Frontend_12102025
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Route đặc biệt cho Account - trỏ vào root Controllers
            //routes.MapRoute(
            //    name: "Account",
            //    url: "Account/{action}/{id}",
            //    defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
            //    namespaces: new[] { "Frontend_12102025.Controllers" }  // Chỉ root namespace
            //);

            //// Default route
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "Frontend_12102025.Controllers" }  // Chỉ root namespace
            //);
        }
    }
}
