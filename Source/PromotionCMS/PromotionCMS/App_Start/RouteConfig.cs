using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PromotionCMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
            routes.MapRoute(
                name: "Details News",
                url: "tin-tuc-chi-tiet/{postAlias}",
                defaults: new { controller = "News", action = "Details", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "News",
               url: "tin-tuc",
               defaults: new { controller = "News", action = "Promotion", id = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "route_action_id_htm",
                url: "{controller}/{action}/{id}.htm",
                defaults: new { controller = "Home", action = "Index" },
                namespaces: new string[] { "PromotionCMS.Controllers" }
            );

            routes.MapRoute(
                name: "route_action_htm",
                url: "{controller}/{action}.htm",
                defaults: new { controller = "Home", action = "Index" },
                namespaces: new string[] { "PromotionCMS.Controllers" }
            );
            routes.MapRoute(
                name: "route_action_id_html",
                url: "{controller}/{action}/{id}.html",
                defaults: new { controller = "Home", action = "Index" },
                namespaces: new string[] { "PromotionCMS.Controllers" }
            );
            routes.MapRoute(
                name: "route_action_html",
                url: "{controller}/{action}.html",
                defaults: new { controller = "Home", action = "Index" },
                namespaces: new string[] { "PromotionCMS.Controllers" }
            );
            routes.MapRoute(
                name: "",
                url: "",
                defaults: new { controller = "Home", action = "Index" },
                namespaces: new string[] { "PromotionCMS.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
