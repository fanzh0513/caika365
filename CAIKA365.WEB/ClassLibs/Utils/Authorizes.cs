using AosuApp;
using AosuApp.AosuFramework;
using API.CP.BASE;
using System;
using System.Collections;
using System.Web;
using System.Web.Security;

namespace CAIKA365.WEB.ClassLibs
{
    public class Authorizes
    {
        public static void SetAuthorizeCache(HttpRequestBase Request, HttpResponseBase Response, Hashtable aHT)
        {
            if (string.IsNullOrEmpty(Request["account"]))
            {
                throw new ArgumentNullException("account");
            }

            if (string.IsNullOrEmpty(Request["vcode"]))
            {
                throw new ArgumentNullException("vcode");
            }

            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                1,
                string.Format("{0}@{1}", Request["account"], aHT["DOMAINNAME"]),
                DateTime.Now,
                Request["remeber"] == "on" ? DateTime.Now.AddDays(10) : DateTime.Now.AddDays(1),
                false,
                Request.UserHostAddress
            );

            // 通过IP+时间戳+验证码生成一个唯一的sessionid。
            string __sid = new DictSetUtil(null).PushSLItem(Request.UserHostAddress).PushSLItem(Functions.ToTimestamp(DateTime.Now)).PushSLItem(Request["vcode"])
                .DoSignature();

            // 缓存到服务器内存中（仅保存30分钟，如果30分钟内处于闲置状态则会清除）
            ACachTool.PickCachTool().SetValue(__sid, FormsAuthentication.Encrypt(authTicket), 30);
            ACachTool.PickCachTool().SetValue(authTicket.Name, aHT, 30);

            // cookie保存一个月
            HttpCookie cookie = new HttpCookie("__sid", __sid);
            cookie.Expires = DateTime.Now.AddMonths(1);
            Response.Cookies.Add(cookie);
        }

        public static Hashtable GetAuthorizeCache(HttpRequestBase Request)
        {
            Hashtable result = null;
            if (Request.Cookies["__sid"] != null)
            {
                string sid = Request.Cookies["__sid"].Value;
                string ticket = ACachTool.PickCachTool().GetValue(sid) as string;
                if (!string.IsNullOrEmpty(ticket))
                {
                    // 从服务器缓存中得到seession的ticket信息
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(ticket);
                    if (authTicket != null && !authTicket.Expired && authTicket.UserData == Request.UserHostAddress)
                    {
                        Hashtable aHT = ACachTool.PickCachTool().GetValue(authTicket.Name) as Hashtable;
                        if (aHT != null)
                        {
                            ParamUtil.Pick(aHT).ImportSets(MemberDBUtils.GetMemberDigest(BaseControl.GlobalControl, ParamUtil.Pick(aHT).GetValueAsString("DOMAINUSER")));
                            ParamUtil.Pick(aHT).SetParam("ticket", authTicket);

                            // 缓存续期
                            ACachTool.PickCachTool().SetValue(sid, FormsAuthentication.Encrypt(authTicket), 30, true);
                            ACachTool.PickCachTool().SetValue(authTicket.Name, aHT, 30, true);

                            result = aHT;
                        }
                    }
                }
            }

            return result;
        }
    }

    public class MemberValidationAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            //HttpRequest request = HttpContext.Current.Request;
            //string userName = request["username"];

            //HttpCookie cookie = request.Cookies["__sid"];
            //if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            //{
            //    Hashtable aHT = Authorizes.GetAuthorizeCache(cookie.Value);
            //    if (aHT != null)
            //    {
            //        FormsAuthenticationTicket authTicket = aHT["ticket"] as FormsAuthenticationTicket;
            //        if (authTicket != null)
            //        {
            //            if (authTicket.UserData == request.UserHostAddress)
            //            {
            //                if (userName == ParamUtil.Pick(aHT).GetValueAsString("DOMAINUSER"))
            //                {
            //                    // 检验成功，跳出方法
            //                    return;
            //                }
            //            }

            //            ACachTool.PickCachTool().SetValue(authTicket.Name, null);
            //        }
            //    }

            //    ACachTool.PickCachTool().SetValue(cookie.Value, null);
            //}

            //// 如果验证失败，则跳到首页
            //filterContext.Result = new RedirectResult("/");
            //request.Cookies.Remove("__sid");
        }
    }

    public class AgentValidationAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
        }
    }
}