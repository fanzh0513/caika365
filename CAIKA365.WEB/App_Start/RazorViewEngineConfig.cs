using System.Web.Mvc;

namespace CAIKA365.WEB.App_Start
{
    public class RazorViewEngineConfig : RazorViewEngine
    {
        public RazorViewEngineConfig()
        {
            ViewLocationFormats = new[]
            {
                 "~/Views/{1}/{0}.cshtml",
                 "~/Views/Shared/{0}.cshtml",

                 // 自定义的规则
                 "~/Views/library/{1}/{0}.cshtml",
                 "~/Views/lottery/{1}/{0}.cshtml"
            };

            // 分部视图位置
            PartialViewLocationFormats = new[]
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",

                // 自定义的规则
                "~/Views/library/{1}/{0}.cshtml",
                "~/Views/lottery/{1}/{0}.cshtml"
            };
        }
    }
}