using AosuApp.AosuFramework;
using AosuApp.AosuSSO;
using API.CP.BASE;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CAIKA365.WEB.ClassLibs;
using CAIKA365.WEB.JSONObject;
using System.IO;

namespace CAIKA365.WEB.Controllers
{
    public class MemberController : BaseController
    {
        public ActionResult register()
        {
            return View("Register", new PageModel(this, @"model\pp.xml"));
        }

        public ActionResult login()
        {
            return View("Login", new PageModel(this, @"model\pp.xml"));
        }

        [HttpPost]
        public ActionResult preDeposit()
        {
            Hashtable aHT = Authorizes.GetAuthorizeCache(Request);
            if (aHT != null)
            {
                // 防止重复提交，客户端会在触发充值时生成一个时间戳作为标识。
                string t_id = Request["t_id"];
                string username = Request["username"];
                double amount;
                if (!string.IsNullOrEmpty(t_id) && !string.IsNullOrEmpty(username) && double.TryParse(Request["amount"], out amount) && amount > 0)
                {
                    Hashtable payment = CommonDBUtils.GetPaymentInfo(Request["payid"]);
                    if (payment != null)
                    {
                        switch (Request["channel"])
                        {
                            case "netpay":
                                break;
                            case "fastway":
                                break;
                            case "thirdway":
                                break;
                        }

                        PageModel model = new PageModel(this, @"model\usercenter.xml");
                        model.Parameters.TID = t_id;
                        model.Parameters.Account = username;
                        model.Parameters.PayLink = payment["PAYLINK"];
                        model.Parameters.PayMethod = payment["PAYMETHOD"];
                        model.Parameters.Amount = amount;

                        return View("PreDeposit", model);
                    }
                }

                return Redirect("/Error");
            }

            return Redirect("/");
        }

        [HttpPost]
        public JsonResult submitDeposit()
        {
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileStreamResult getQRImage()
        {
            Hashtable aHT = Authorizes.GetAuthorizeCache(Request);
            if (aHT != null)
            {
                // 防止重复提交，客户端会在触发充值时生成一个时间戳作为标识。
                string t_id = Request.QueryString["v"];
                string username = Request["u"];
                string channel = Request.QueryString["c"];
                string payId = Request.QueryString["p"];
                if (channel == "qrway" && double.TryParse(Request["a"], out double amount) && amount > 0)
                {
                    // 得到接口数据并解析其对应的支付接口逻辑。
                    UriUtil paymethod = PaymentUtil.GetInterface(channel, payId);
                    if(paymethod != null)
                    {
                        ParamUtil paramUtil = PickParam(Request.QueryString).SetCmd(paymethod.GetQueryItem(ActionUtil.Cmd)).ExecuteCmd(paymethod.GetActionInstance(GetControl()));
                        if (paramUtil.IsOK())
                        {
                            // 
                        }
                    }
                }
            }

            return File(new FileStream(Server.MapPath("~/content/images/noimage.png"), FileMode.Open, FileAccess.Read), "image/png");
        }

        [HttpGet]
        public FileContentResult getvImage()
        {
            string randomcode = VerificationCodeUtil.CreateCode(4);
            Session["vcode"] = randomcode;

            MemoryStream ms = new MemoryStream();
            using (Bitmap map = VerificationCodeUtil.CreateImage(randomcode))
            {
                //生成图片
                map.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            }

            return File(ms.ToArray(), "image/gif");
        }

        [HttpGet]
        public JsonResult getvCode()
        {
            string v_code = Session["vcode"] as string;
            Session.Remove("vcode");
            return Json(v_code, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult doRegister()
        {
            string flag = "success";
            string message = "";
            if (!string.IsNullOrEmpty(Request["vcode"]))
            {
                string strAccountId = Request["account"];
                string strPassword = Request["password"];
                string strAgentCode = Request["agentcode"];
                if (string.IsNullOrEmpty(strAgentCode))
                {
                    strAgentCode = "000101999";
                }

                // 是否存在？
                if (MemberDBUtils.MemberIsExist(GetControl(), strAccountId))
                {
                    flag = "failed";
                    message = "用户名已存在";
                }
                else
                {
                    Hashtable aHT = new Hashtable();
                    PickParam(aHT).SetParam("DOMAINUSER", strAccountId);
                    PickParam(aHT).SetParam("DOMAINNAME", "caika.com");
                    PickParam(aHT).SetParam("PSTPWD", strPassword);
                    PickParam(aHT).SetParam("TYPE", account_type.NormalAcount);
                    PickParam(aHT).SetParam("IP", Request.UserHostAddress);
                    PickParam(aHT).SetParam(MemberDBUtils.GetMemberDigest(GetControl()));
                    PickParam(aHT).SetParam("PARENTAGENT", strAgentCode);

                    // 校验应用许可权的合法性
                    ParamUtil checkLicense = PickParam().Merge(aHT).SetCmd(ADomain.CCheckLicense).ExecuteCmd(new ADomain());
                    if (!checkLicense.IsOK())
                    {
                        flag = "failed";
                        message = checkLicense.GetError();
                    }
                    else
                    {
                        // 先注册通行证、然后再添加会员账户
                        // 执行CRegister命令后会破坏aHT参数信息，所以在此创建一个临时aHT1用于避免原始参数集合不被破坏。
                        Hashtable aHT1 = new Hashtable();
                        if (PickParam(aHT1).Merge(aHT).SetCmd(APassport.CRegister).ExecuteCmd(new APassport()).IsOK())
                        {
                            // 创建会员账户
                            Hashtable aHT2 = new Hashtable();
                            MemberDBUtils.CreateMemberRecord(GetControl(), PickParam(aHT2).Merge(aHT).ParamTable);
                            if (!PickParam(aHT2).IsOK())
                            {
                                flag = "failed";
                                message = PickParam(aHT2).GetError();
                            }
                            else
                            {
                                // 注册完成后直接登录
                                if (PickParam(aHT).SetCmd(APassport.CSignOn).ExecuteCmd(new APassport()).IsOK())
                                {
                                    Authorizes.SetAuthorizeCache(Request, Response, PickParam(aHT).GetValue("passport") as Hashtable);
                                }
                                else
                                {
                                    flag = "failed";
                                    message = PickParam(aHT).GetError();
                                }
                            }
                        }
                        else
                        {
                            flag = "failed";
                            message = PickParam(aHT1).GetError();
                        }
                    }
                }
            }
            else
            {
                flag = "failed";
                message = "验证码失效";
            }

            return Json(new { state = flag, message = message });
        }

        [HttpPost]
        public JsonResult doLogin()
        {
            string flag = "success";
            string message = "";
            if (!string.IsNullOrEmpty(Request["vcode"]))
            {
                string strAccountId = Request["account"];
                string strPassword = Request["password"];

                // 是否存在？
                if (!MemberDBUtils.MemberIsExist(GetControl(), strAccountId))
                {
                    flag = "failed";
                    message = "用户名或密码错误";
                }
                else
                {
                    Hashtable aHT = new Hashtable();
                    PickParam(aHT).SetParam("DOMAINNAME", "caika.com");
                    PickParam(aHT).SetParam("DOMAINUSER", Request["account"]);
                    PickParam(aHT).SetParam("PSTPWD", Request["password"]);
                    PickParam(aHT).SetParam("IP", Request.UserHostAddress);
                    if (PickParam(aHT).SetCmd(APassport.CSignOn).ExecuteCmd(new APassport()).IsOK())
                    {
                        Authorizes.SetAuthorizeCache(Request, Response, PickParam(aHT).GetValue("passport") as Hashtable);
                    }
                    else
                    {
                        flag = "failed";
                        message = PickParam(aHT).GetError();
                    }
                }
            }
            else
            {
                flag = "failed";
                message = "验证码失效";
            }

            return Json(new { state = flag, message = message });
        }

        public RedirectResult doLogout()
        {
            HttpCookie cookie = Request.Cookies["__sid"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                string ticket = ACachTool.PickCachTool().GetValue(cookie.Value) as string;
                if (!string.IsNullOrEmpty(ticket))
                {
                    // 从服务器缓存中得到seession的ticket信息
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(ticket);
                    if (authTicket != null && authTicket.Name == string.Format("{0}@caika.com", Request["usrename"]))
                    {
                        ACachTool.PickCachTool().SetValue(authTicket.Name, null);
                    }
                }

                Request.Cookies.Remove("__sid");
                ACachTool.PickCachTool().SetValue(cookie.Value, null);

                // 登出
                new ParamUtil()
                    .SetCmd(APassport.CSignOut).SetParam("SSOPST", string.Format("{0}@caika.com", Request["username"]))
                    .ExecuteCmd(new APassport());
            }

            string url = Request["url"];
            if (string.IsNullOrEmpty(url))
            {
                url = "/";
            }

            return Redirect(url);
        }

        public ActionResult UserCenter(string id)
        {
            Hashtable aHT = Authorizes.GetAuthorizeCache(Request);
            if (aHT != null)
            {
                if (string.IsNullOrEmpty(id))
                    id = "zhmx";

                PageModel model = new PageModel(this, @"model\usercenter.xml");
                model.Parameters.Subpage = id;
                model.Parameters.Pid = string.Format("{0}{1}", id, Request["t"]);
                model.Parameters.Account = ParamUtil.Pick(aHT).GetValueAsString("DOMAINUSER");
                model.Parameters.NickName = ParamUtil.Pick(aHT).GetValueAsString("NICKNAME");
                model.Parameters.LoginTime = ParamUtil.Pick(aHT).GetValueAsString("LOGINTIME");
                model.Parameters.Available = string.Format("{0:f2}", Convert.ToDecimal(aHT["AVAILABLE"]));
                model.Parameters.Freezed = string.Format("{0:f2}", Convert.ToDecimal(aHT["FREEZED"]));
                model.Parameters.Score = aHT["SCORE"].ToString();
                switch (id)
                {
                    case "czlink":
                        model.Parameters.PayList = CommonDBUtils.GetPaymentList();
                        break;
                    case "zhmx":
                        // 返回账户流水清单
                        DataSet ds_result = new ParamUtil()
                            .SetCmd(AAccountUtil.CGetJournalList)
                            .SetParam(model.Parameters.Account).SetParam("type", Request["t"]).SetParam("time", Request["m"]).SetParam("pageno", Request["pageno"])
                            .ExecuteCmd(new AAccountUtil())
                            .GetValueAsDataSet();

                        model.Parameters.List = ds_result.Tables["list"];

                        DictSetUtil ds_total = new DictSetUtil(ds_result);
                        model.Parameters.Deposited = string.Format("{0:f}", string.IsNullOrEmpty(ds_total.GetValue(journal_type.充值)) ? "0.00" : ds_total.GetValue(journal_type.充值));
                        model.Parameters.Bonus = string.Format("{0:f}", string.IsNullOrEmpty(ds_total.GetValue(journal_type.中奖)) ? "0.00" : ds_total.GetValue(journal_type.中奖));
                        model.Parameters.Exchanged = string.Format("{0:f}", string.IsNullOrEmpty(ds_total.GetValue(journal_type.积分兑换)) ? "0.00" : ds_total.GetValue(journal_type.积分兑换));
                        model.Parameters.Returned = string.Format("{0:f}", string.IsNullOrEmpty(ds_total.GetValue(journal_type.返点)) ? "0.00" : ds_total.GetValue(journal_type.返点));
                        model.Parameters.Consume = string.Format("{0:f}", string.IsNullOrEmpty(ds_total.GetValue(journal_type.投注)) ? "0.00" : ds_total.GetValue(journal_type.投注));
                        model.Parameters.Withdraw = string.Format("{0:f}", string.IsNullOrEmpty(ds_total.GetValue(journal_type.提现)) ? "0.00" : ds_total.GetValue(journal_type.提现));

                        model.Parameters.Type = Request["t"];
                        model.Parameters.Time = Request["m"];
                        model.Parameters.PageNo = string.IsNullOrEmpty(Request["pageno"]) ? "1" : Request["pageno"];
                        model.Parameters.PageNums = 0;
                        if (ds_result.Tables["pages"].Rows.Count > 0)
                        {
                            model.Parameters.PageNums = ds_result.Tables["pages"].Rows[0][0];
                        }
                        break;
                    case "zhmm":
                        model.Parameters.WithdrawIsEmpty = AAccountUtil.WithdrawIsEmpty(ParamUtil.Pick(aHT).GetValueAsString("DOMAINUSER"));
                        break;
                    case "bdyhk":
                        model.Parameters.Provinces = CommonDBUtils.GetAreas();

                        // 返回账户的绑定的银行卡数据
                        DictSetUtil ds_bank = new DictSetUtil(AAccountUtil.GetBankData(ParamUtil.Pick(aHT).GetValueAsString("DOMAINUSER")));
                        model.Parameters.BankData = ds_bank.MyDS;
                        model.Parameters.HaveBank = ds_bank.DSxtcs.Rows.Count > 0;
                        break;
                    case "grzl":
                        DictSetUtil ds_info = new DictSetUtil(AAccountUtil.GetMemberInfo(ParamUtil.Pick(aHT).GetValueAsString("DOMAINUSER")));
                        model.Parameters.MemberInfo = ds_info.MyDS;
                        model.Parameters.InfoIsFull = !string.IsNullOrEmpty(ds_info.GetValue("REALNAME")) && !string.IsNullOrEmpty(ds_info.GetValue("UID")) && !string.IsNullOrEmpty(ds_info.GetValue("ANSWER"));
                        break;
                    case "msglist":
                        DataSet ds_msglist = AAccountUtil.GetMessageList(ParamUtil.Pick(aHT).GetValueAsString("DOMAINUSER"), Request["pageno"]);
                        model.Parameters.List = ds_msglist.Tables["list"];
                        model.Parameters.PageNums = ds_msglist.Tables["pages"].Rows.Count > 0 ? ds_msglist.Tables["pages"].Rows[0][0] : 0;
                        model.Parameters.PageNo = string.IsNullOrEmpty(Request["pageno"]) ? "1" : Request["pageno"];
                        break;
                }

                return View("UserCenter", model);
            }

            return Redirect("/");
        }

        [HttpPost]
        public JsonResult queryJournalList()
        {
            Hashtable aHT = Authorizes.GetAuthorizeCache(Request);
            if (aHT != null)
            {
                // 返回账户流水清单
                DataSet ds_result = new ParamUtil()
                    .SetCmd(AAccountUtil.CGetJournalList)
                    .SetParam(aHT["DOMAINUSER"]).SetParam("type", Request["t"]).SetParam("time", Request["m"]).SetParam("pageno", Request["pageno"])
                    .ExecuteCmd(new AAccountUtil())
                    .GetValueAsDataSet();

                var rows = ds_result.Tables["list"].Rows.OfType<DataRow>().Select(
                    cm => new journalItem()
                    {
                        t_id = cm["ID"].ToString(),
                        time = cm["JOURNALTIME"].ToString(),
                        type = cm["TYPE"].ToString(),
                        amount = string.Format("{0:f}", cm["AMOUNT"]),
                        available = string.Format("{0:f}", cm["AVAILABLE"]),
                        remark = cm["REMARK"].ToString(),
                        state = cm["STATE"].ToString()
                    });

                DictSetUtil ds_total = new DictSetUtil(ds_result);
                return Json(new
                {
                    flag = "y",
                    pagenums = ds_result.Tables["pages"].Rows.Count > 0 ? ds_result.Tables["pages"].Rows[0][0] : 0,
                    pageno = string.IsNullOrEmpty(Request["pageno"]) ? "1" : Request["pageno"],
                    deposited = string.Format("{0:f}", string.IsNullOrEmpty(ds_total.GetValue(journal_type.充值)) ? "0.00" : ds_total.GetValue(journal_type.充值)),
                    bonus = string.Format("{0:f}", string.IsNullOrEmpty(ds_total.GetValue(journal_type.中奖)) ? "0.00" : ds_total.GetValue(journal_type.中奖)),
                    exchanged = string.Format("{0:f}", string.IsNullOrEmpty(ds_total.GetValue(journal_type.积分兑换)) ? "0.00" : ds_total.GetValue(journal_type.积分兑换)),
                    returned = string.Format("{0:f}", string.IsNullOrEmpty(ds_total.GetValue(journal_type.返点)) ? "0.00" : ds_total.GetValue(journal_type.返点)),
                    consume = string.Format("{0:f}", string.IsNullOrEmpty(ds_total.GetValue(journal_type.投注)) ? "0.00" : ds_total.GetValue(journal_type.投注)),
                    withdraw = string.Format("{0:f}", string.IsNullOrEmpty(ds_total.GetValue(journal_type.提现)) ? "0.00" : ds_total.GetValue(journal_type.提现)),
                    rows = rows.ToList()
                });
            }

            return Json(new { flag = "n" });
        }

        [HttpPost]
        public JsonResult queryMessageList()
        {
            Hashtable aHT = Authorizes.GetAuthorizeCache(Request);
            if (aHT != null)
            {
                DataSet ds_result = AAccountUtil.GetMessageList(ParamUtil.Pick(aHT).GetValueAsString("DOMAINUSER"), Request["pageno"]);
                List<messageItem> rows = new List<messageItem>();
                foreach (DataRow item in ds_result.Tables["list"].Rows)
                {
                    messageItem a = new messageItem();
                    switch (item.Field<int>("MSGTYPE"))
                    {
                        case message_id.Account_KeyIn_BankCard:
                        case message_id.Account_KeyIn_DetailInfo:
                        case message_id.Account_KeyIn_Withdrawals:
                            a.msg_type = "账户提示";
                            break;
                        default:
                            a.msg_type = "系统消息";
                            break;
                    }
                    a.msg_id = item["MSGID"].ToString();
                    a.title = item["MSGTITLE"].ToString();
                    a.ctime = Convert.ToDateTime(item["CTIME"]).ToString("yyyy/MM/dd HH:mm");
                    a.state = item["STATE"].ToString();
                    rows.Add(a);
                }

                return Json(new
                {
                    flag = "y",
                    pagenums = ds_result.Tables["pages"].Rows.Count > 0 ? ds_result.Tables["pages"].Rows[0][0] : 0,
                    pageno = string.IsNullOrEmpty(Request["pageno"]) ? "1" : Request["pageno"],
                    rows = rows.ToList()
                });
            }

            return Json(new { flag = "n" });
        }

        [HttpGet]
        public JsonResult getMessageBody()
        {
            try
            {
                return Json(new
                {
                    flag = "y",
                    msgbody = AAccountUtil.GetMessageBody(Request["msgid"])
                },
                JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { flag = "n" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult deleteMessage()
        {
            try
            {
                AAccountUtil.DeleteMessage(Request["msgid"]);
                return Json(new { flag = "y" });
            }
            catch (Exception)
            {
                return Json(new { flag = "n" });
            }
        }

        [HttpPost]
        public JsonResult queryLoginStatus()
        {
            Hashtable aHT = Authorizes.GetAuthorizeCache(Request);
            if (aHT != null)
            {
                return Json(new
                {
                    flag = 'y',
                    pp = aHT["SSOPST"],
                    account = aHT["DOMAINUSER"],
                    nick = aHT["NICKNAME"],
                    available = string.Format("{0:f2}", Convert.ToDecimal(aHT["AVAILABLE"])),
                    freezed = string.Format("{0:f2}", Convert.ToDecimal(aHT["FREEZED"])),
                    score = aHT["SCORE"]
                });
            }

            return Json(new { flag = "n" });
        }

        [HttpPost]
        public JsonResult changePwd()
        {
            Hashtable aHT = Authorizes.GetAuthorizeCache(Request);
            if (aHT != null)
            {
                ParamUtil aPU = new ParamUtil()
                    .SetCmd(AAccountUtil.CChangePassword)
                    .SetParam(aHT["DOMAINUSER"]).SetParam("type", Request["t"]).SetParam("c_pass", Request["c_pass"]).SetParam("n_pass", Request["n_pass"])
                    .ExecuteCmd(new AAccountUtil());
                if (aPU.IsOK())
                {
                    return Json(new { flag = "y", message = "success" });
                }
                else
                {
                    return Json(new { flag = "n", message = aPU.GetError() });
                }
            }

            return Json(new { flag = "n", message = "账户登录状态失效" });
        }

        [HttpPost]
        public JsonResult changeBank()
        {
            Hashtable aHT = Authorizes.GetAuthorizeCache(Request);
            if (aHT != null)
            {
                DictSetUtil ds_digest = new DictSetUtil(new DataSet());
                ds_digest.SetValue("realname", Request["realname"]);
                ds_digest.SetValue("bankno", Request["bankno"]);
                ds_digest.SetValue("bankname", Request["bankname"]);
                ds_digest.SetValue("province", Request["province"]);
                ds_digest.SetValue("province_name", Request["province_name"]);
                ds_digest.SetValue("city", Request["city"]);
                ds_digest.SetValue("city_name", Request["city_name"]);
                ds_digest.SetValue("subbank", Request["subbank"]);
                ds_digest.SetValue("cardnum", Request["cardnum"]);

                ParamUtil aPU = new ParamUtil()
                  .SetCmd(AAccountUtil.CChangeBank)
                  .SetParam(aHT["DOMAINUSER"]).SetParam("c_pass", Request["c_pass"]).SetParam("realname", Request["realname"]).SetParam("digest", ds_digest.MyDS)
                  .ExecuteCmd(new AAccountUtil());
                if (aPU.IsOK())
                {
                    return Json(new { flag = "y", message = "success" });
                }
                else
                {
                    return Json(new { flag = "n", message = aPU.GetError() });
                }
            }

            return Json(new { flag = "n", message = "账户登录状态失效" });
        }

        [HttpPost]
        public JsonResult postMember()
        {
            Hashtable aHT = Authorizes.GetAuthorizeCache(Request);
            if (aHT != null)
            {
                DictSetUtil ds_member = new DictSetUtil(new DataSet());
                ds_member.SetValue("nickname", Request["nickname"]);
                ds_member.SetValue("email", Request["email"]);
                ds_member.SetValue("realname", Request["realname"]);
                ds_member.SetValue("idtype", Request["idtype"]);
                ds_member.SetValue("id", Request["id"]);
                ds_member.SetValue("ask", Request["ask"]);
                ds_member.SetValue("answer", Request["answer"]);

                ParamUtil aPU = new ParamUtil()
                  .SetCmd(AAccountUtil.CPostMemberInfo)
                  .SetParam(aHT["DOMAINUSER"]).SetParam("c_pass", Request["c_pass"]).SetParam("member", ds_member.MyDS)
                  .ExecuteCmd(new AAccountUtil());
                if (aPU.IsOK())
                {
                    return Json(new { flag = "y", message = "success" });
                }
                else
                {
                    return Json(new { flag = "n", message = aPU.GetError() });
                }
            }

            return Json(new { flag = "n", message = "账户登录状态失效" });
        }

        [HttpPost]
        public JsonResult postRequest(string id)
        {
            Hashtable aHT = Authorizes.GetAuthorizeCache(Request);
            if (aHT != null)
            {
                ParamUtil aPU = new ParamUtil()
                    .SetCmd(AAccountUtil.CPostRequest)
                    .SetParam(aHT["DOMAINUSER"]).SetParam("type", id).SetParam("msgid", Request["msgid"])
                    .ExecuteCmd(new AAccountUtil());
                if (aPU.IsOK())
                {
                    return Json(new { flag = "y", message = "success" });
                }
                else
                {
                    return Json(new { flag = "n", message = aPU.GetError() });
                }
            }

            return Json(new { flag = "n", message = "账户登录失效" });
        }

        public JsonResult message_polling()
        {
            return null;
        }

        public JsonResult message_system()
        {
            return null;
        }

        public JsonResult mymessage_updateAllRead()
        {
            return null;
        }
    }
}