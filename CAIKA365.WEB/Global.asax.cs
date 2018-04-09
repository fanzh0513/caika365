using System.Web.Mvc;
using System.Web.Routing;
using CAIKA365.WEB.App_Start;

namespace CAIKA365.WEB
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngineConfig());
        }
    }
}
