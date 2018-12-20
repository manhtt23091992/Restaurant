using System.Configuration;
using System.Web.Mvc;

namespace PromotionCMS.Areas.Admin
{
    public class AreasAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //    "Areas_default",
            //    "Areas/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);
            var alias = ConfigurationManager.AppSettings["admin_alias"];

            context.MapRoute(
                name: "admin_route_action_id_htm",
                url: alias + "/{controller}/{action}/{id}.htm",
                defaults: new { controller = "Home", action = "Index" },
                namespaces: new string[] { "PromotionCMS.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                name: "admin_route_action_htm",
                url: alias + "/{controller}/{action}.htm",
                defaults: new { controller = "Home", action = "Index" },
                namespaces: new string[] { "PromotionCMS.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                name: "admin_route_action_id_html",
                url: alias + "/{controller}/{action}/{id}.html",
                defaults: new { controller = "Home", action = "Index" },
                namespaces: new string[] { "PromotionCMS.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                name: "admin_route_action_html",
                url: alias + "/{controller}/{action}.html",
                defaults: new { controller = "Home", action = "Index" },
                namespaces: new string[] { "PromotionCMS.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                name: "admin_route_action_default_html",
                url: alias,
                defaults: new { controller = "Home", action = "Index" },
                namespaces: new string[] { "PromotionCMS.Areas.Admin.Controllers" }
            );
        }
    }
}