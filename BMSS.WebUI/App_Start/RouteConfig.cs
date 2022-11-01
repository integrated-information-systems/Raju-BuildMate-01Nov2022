using System.Web.Mvc;
using System.Web.Routing;

namespace BMSS.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            // To show Images in Crystal Report or otherwise images will not appear in Crystal Report
            //routes.IgnoreRoute("WForms/{resource}.aspx/{*pathInfo}");
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*(CrystalImageHandler).*" });

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
        }
    }
}
