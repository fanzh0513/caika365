using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAIKA365.WEB.ClassLibs;
using CAIKA365.WEB.JSONObject;

namespace CAIKA365.WEB.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View("Index", new PageModel(this, @"model\core.xml"));
        }

        [HttpGet]
        public JsonResult carousel(string id)
        {
            List<carouselItem> aList = new List<carouselItem>();
            aList.Add(new carouselItem { img = AppDomain + "content/images/headFigure/134580899612.jpg", title = "默认轮播", url = "http://888.163.com/?from=zycpgp" });
            aList.Add(new carouselItem { img = AppDomain + "content/images/headFigure/1473309705276_1.png", title = "欢乐购顶部彩票轮播", url = "http://888.163.com/?from=zycpgp" });
            aList.Add(new carouselItem { img = AppDomain + "content/images/headFigure/1473241637392_1.jpg", title = "贵金属", url = "http://fa.163.com/activity/pmecsimulate/sign.do?from=tgncppcttgj" });
            aList.Add(new carouselItem { img = AppDomain + "content/images/headFigure/1473334944050_1.png", title = "10元话费", url = "http://game.cp.163.com/mailGiftShow.html" });
            aList.Add(new carouselItem { img = AppDomain + "content/images/headFigure/2014071320TT68247858.jpg", title = "比分早知道秘密在客户端", url = "http://caipiao.163.com/outside/getclient_cp.html#from=toutu" });

            return Json(aList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getAreas(string id)
        {
            DataTable aDT = API.CP.BASE.CommonDBUtils.GetAreas(id);
            if (aDT != null)
            {
                var rows = aDT.Rows.OfType<DataRow>().Select(
                    cm => new regionItem()
                    {
                        id = cm["ID"].ToString(),
                        name1 = cm["NAME1"].ToString(),
                        name2 = cm["NAME2"].ToString(),
                        name3 = cm["NAME3"].ToString()
                    });

                return Json(new
                {
                    flag = "y",
                    rows = rows.ToList()
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { flag = "n" }, JsonRequestBehavior.AllowGet);
        }

    }
}