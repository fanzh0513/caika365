using System.Web.Mvc;
using System.Web.Routing;

namespace CAIKA365.WEB
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //路由规则匹配是从上到下的，优先匹配的路由一定要写在最上面。因为路由匹配成功以后，他不会继续匹配下去。
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "CAIKA365.WEB.Controllers" }
            );

            routes.MapRoute(
                name: "Member",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Member", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "CAIKA365.WEB.Controllers" }
            );

            //routes.MapRoute(
            //    name: "Football",
            //    url: "library/{controller}/{action}/{id}",
            //    defaults: new { controller = "Football", action = "Index", id = UrlParameter.Optional, area = "Library" }
            //);
        }
    }
}
