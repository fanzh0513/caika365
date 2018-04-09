using AosuApp.AosuFramework;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace CAIKA365.WEB.ClassLibs
{
    public static class ResUtils
    {
        public static MvcHtmlString IncludeRes(this UrlHelper url, PageModel pageModle)
        {
            string full_name = string.Format("{0}\\{1}", BaseControl.GlobalControl.GetContext().MyInfo[Context.CDataLocation], pageModle.ResConfigFile);
            if (!File.Exists(full_name))
            {
                throw new Exception(string.Format("缺少配置文件：{0}", full_name));
            }

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(
                new ExeConfigurationFileMap()
                {
                    ExeConfigFilename = new FileInfo(full_name).FullName
                },
                ConfigurationUserLevel.None);

            string ver = pageModle.ResVersion;
            if (string.IsNullOrEmpty(ver))
            {
                ver = AosuApp.Functions.ToTimestamp(DateTime.Now.Date) + "";
            }

            StringBuilder result = new StringBuilder();
            foreach (KeyValueConfigurationElement item in config.AppSettings.Settings)
            {
                string str = "";
                if (item.Key.StartsWith("css"))
                {
                    str = string.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}?ver={1}\"/>", url.Content(item.Value), ver);
                }
                else if (item.Key.StartsWith("js"))
                {
                    str = string.Format("<script type=\"text/javascript\"  src=\"{0}?ver={1}\" ></script>", url.Content(item.Value), ver);
                }
                else if (item.Key.StartsWith("html"))
                {
                    str = string.Format("<script src=\"{0}?ver={1}\" ></script>", url.Content(item.Value), ver);
                }

                result.Append(str);
            }

            return new MvcHtmlString(result.ToString());
        }
    }
}