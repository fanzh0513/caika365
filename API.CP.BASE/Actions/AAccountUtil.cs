using AosuApp.AosuData;
using AosuApp.AosuFramework;
using AosuApp.AosuSSO;
using System;
using System.Collections;
using System.Data;

namespace API.CP.BASE
{
    public class AAccountUtil : APassport
    {
        protected override void Execute(System.Collections.Hashtable Params)
        {
            switch (ActionUtil.GetCmd(Params))
            {
                case CCreateNewAccount:
                    CreateNewAccount(Params);
                    break;
                case CGetAccountSummary:
                    GetAccountSummary(Params);
                    break;
                case CAppendJournal:
                    AppendJournal(Params);
                    break;
                case CGetJournalList:
                    GetJournalList(Params);
                    break;

                case CVerifyPasswrod:
                    VerifyPasswrod(Params);
                    break;
                case CChangePassword:
                    ChangePassword(Params);
                    break;
                case CChangeBank:
                    ChangeBank(Params);
                    break;
                case CPostMemberInfo:
                    PostMemberInfo(Params);
                    break;
                case CPostRequest:
                    PostRequest(Params);
                    break;
            }
        }

        /// <summary>
        /// 创建一个新账户
        /// </summary>
        public const string CCreateNewAccount = "CCreateNewAccount";

        /// <summary>
        /// 获取账户摘要信息
        /// </summary>
        public const string CGetAccountSummary = "CGetAccountSummary";

        /// <summary>
        /// 添加账户流水记录
        /// </summary>
        public const string CAppendJournal = "CAppendJournal";

        /// <summary>
        /// 得到账户流水清单
        /// </summary>
        public const string CGetJournalList = "CGetJournalList";

        /// <summary>
        /// 修改银行卡
        /// </summary>
        public const string CChangeBank = "CChangeBank";

        /// <summary>
        /// 修改会员信息
        /// </summary>
        public const string CPostMemberInfo = "CPostMemberInfo";

        /// <summary>
        /// 提交用户请求
        /// </summary>
        public const string CPostRequest = "CPostRequest";

        public static bool WithdrawIsEmpty(string account)
        {
            try
            {
                // 得到会员信息
                DataSet dsMember = new ParamUtil()
                    .SQLCmdLoadData()
                    .SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT ISNULL(WITHDRAWALS,'') FROM CAIKA_MEMBER WHERE SSOPST='{0}@caika.com'", account))
                    .SQLWithOutSchema()
                    .ExecuteCmd(ADataLoader.DataLoader()).GetValueAsDataSet();
                if (dsMember.Tables["CAIKA_MEMBER"].Rows.Count == 0)
                {
                    return false;
                }
                else
                {
                    return string.IsNullOrEmpty(dsMember.Tables[0].Rows[0].Field<string>(0).Trim());
                }
            }
            catch
            {
                return false;
            }
        }

        public static DataSet GetMemberInfo(string account)
        {
            try
            {
                DataSet dsMember = new ParamUtil()
                    .SQLCmdLoadData()
                    .SQLWithOutSchema()
                    .SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT * FROM CAIKA_MEMBER WHERE SSOPST='{0}'", account))
                    .ExecuteCmd(ADataLoader.DataLoader())
                    .GetValueAsDataSet();

                DictSetUtil dsInfo = new DictSetUtil(new UriUtil().ImportRow(dsMember.Tables[0].Rows[0]).DSQueryItem);
                return dsInfo.MyDS;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DataSet GetBankData(string account)
        {
            try
            {
                DataSet dsMember = new ParamUtil()
                    .SQLCmdLoadData()
                    .SQLWithOutSchema()
                    .SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT * FROM CAIKA_MEMBER WHERE SSOPST='{0}@caika.com'", account))
                    .ExecuteCmd(ADataLoader.DataLoader())
                    .GetValueAsDataSet();

                string key = dsMember.Tables[0].Rows[0]["BANKDIGEST"].ToString().Trim();
                return new DictSetUtil(AEntryDic.Pick().GetDic(key)).MyDS;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int NewMsgCount(string account)
        {
            try
            {
                DataSet ds = new ParamUtil()
                    .SQLCmdLoadData()
                    .SQLWithOutSchema()
                    .SQLEntityScript("CAIKA_MEMBER_MESSAGE", string.Format("SELECT COUNT(*) FROM CAIKA_MEMBER_MESSAGE WHERE SSOPST='{0}@caika.com' AND STATE='{1}'", account, message_state.UnRead))
                    .ExecuteCmd(ADataLoader.DataLoader())
                    .GetValueAsDataSet();

                return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            catch
            {
                return 0;
            }
        }

        public static DataSet GetMessageList(string account, string pageno = "")
        {
            if (string.IsNullOrEmpty(pageno))
            {
                pageno = "1";
            }

            try
            {
                string sql_core = string.Format("SELECT TOP 100 PERCENT  MSGID,MSGTYPE,MSGTITLE,CTIME,STATE FROM CAIKA_MEMBER_MESSAGE WHERE SSOPST='{0}@caika.com' ORDER BY CTIME", account);
                string sql = string.Format(@"SELECT TOP 30 * FROM (SELECT ROW_NUMBER() OVER (ORDER BY MSGID DESC) ROWNUM,T1.* FROM ({0}) T1) AS A WHERE ROWNUM > 30*({1}-1)", sql_core, pageno);
                DataSet ds = new ParamUtil()
                    .SQLCmdLoadData()
                    .SQLWithOutSchema()
                    .SQLEntityScript("list", sql)
                    .SQLEntityScript("pages", string.Format("SELECT CEILING(CAST(COUNT(*) AS FLOAT)/30) FROM CAIKA_MEMBER_MESSAGE WHERE SSOPST='{0}@caika.com'", account))
                    .ExecuteCmd(ADataLoader.DataLoader())
                    .GetValueAsDataSet();

                return ds;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetMessageBody(string msgid)
        {
            DataSet dsPersistent = new ParamUtil()
                .SQLCmdLoadData()
                .SQLEntityScript("CAIKA_MEMBER_MESSAGE", string.Format("SELECT * FROM CAIKA_MEMBER_MESSAGE WHERE MSGID='{0}'", msgid))
                .ExecuteCmd(ADataLoader.DataLoader())
                .GetValueAsDataSet();
            if (dsPersistent.Tables[0].Rows.Count > 0)
            {
                try
                {
                    if (dsPersistent.Tables[0].Rows[0]["STATE"].ToString() != message_state.Read)
                    {
                        dsPersistent.Tables[0].Rows[0]["STATE"] = message_state.Read;
                        new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader());
                    }

                    return dsPersistent.Tables[0].Rows[0]["MSGBODY"].ToString();
                }
                catch (Exception)
                {
                }
            }

            return "";
        }

        public static void DeleteMessage(string msgid)
        {
            DataSet dsPersistent = new ParamUtil()
                .SQLCmdLoadData()
                .SQLEntityScript("CAIKA_MEMBER_MESSAGE", string.Format("SELECT * FROM CAIKA_MEMBER_MESSAGE WHERE MSGID='{0}'", msgid))
                .ExecuteCmd(ADataLoader.DataLoader())
                .GetValueAsDataSet();
            if (dsPersistent.Tables[0].Rows.Count > 0)
            {
                try
                {
                    dsPersistent.Tables[0].Rows[0].Delete();
                    new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader());
                }
                catch (Exception)
                {
                }
            }
        }

        private void CreateNewAccount(Hashtable Params)
        {
            string account = PickParam(Params).GetValueAsString("DOMAINUSER");
            string domain = PickParam(Params).GetValueAsString("DOMAINNAME");
            try
            {
                // 注册通行证
                ParamUtil register = PickParam(Params).SetCmd(APassport.CRegister).ExecuteCmd(new APassport());
                if (register.IsOK())
                {
                    ParamUtil aPU = new ParamUtil()
                        .SQLCmdLoadData()
                        .SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT * FROM CAIKA_MEMBER WHERE SSOPST='{0}'", register.GetValue()))
                        .SQLEntityScript("CAIKA_MEMBER_MESSAGE", string.Format("SELECT * FROM CAIKA_MEMBER_MESSAGE WHERE SSOPST='{0}'", register.GetValue()))
                        .ExecuteCmd(ADataLoader.DataLoader());
                    if (aPU.IsOK())
                    {
                        DataSet dsPersistent = aPU.GetValueAsDataSet();
                        if (dsPersistent.Tables["CAIKA_MEMBER"].Rows.Count == 0)
                        {
                            DataRow rowNew = dsPersistent.Tables["CAIKA_MEMBER"].NewRow();
                            rowNew["SSOPST"] = register.GetValue();
                            rowNew["NICKNAME"] = account;
                            rowNew["UPDFLAG"] = "Y";
                            rowNew["STATE"] = AosuApp.state.Enabled;
                            rowNew["CTIME"] = DateTime.Now;
                            rowNew["ETIME"] = DateTime.Now;
                            dsPersistent.Tables["CAIKA_MEMBER"].Rows.Add(rowNew);

                            // 添加提示信息 - 详细资料
                            DataRow row_message1 = AosuApp.Functions.FindRow(string.Format("MSGTYPE={0}", message_id.Account_KeyIn_DetailInfo), dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"]);
                            if (row_message1 == null)
                            {
                                row_message1 = dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].NewRow();
                                row_message1["MSGID"] = Guid.NewGuid();
                                row_message1["SSOPST"] = register.GetValue();
                                row_message1["MSGTYPE"] = message_id.Account_KeyIn_DetailInfo;
                                row_message1["MSGTITLE"] = "请完善用户基本资料";
                                row_message1["MSGBODY"] = "<strong>完善用户基本资料提醒</strong><br/></br/>为了确保彩咖网更好的服务于您，请您完善<a href='/member/usercenter/grzl'><strong>个人资料信息</strong></a>。";
                                row_message1["STATE"] = message_state.UnRead;
                                row_message1["CTIME"] = DateTime.Now;
                                dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].Rows.Add(row_message1);
                            }

                            // 添加提示信息 - 设置提款密码
                            DataRow row_message2 = AosuApp.Functions.FindRow(string.Format("MSGTYPE={0}", message_id.Account_KeyIn_Withdrawals), dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"]);
                            if (row_message2 == null)
                            {
                                row_message2 = dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].NewRow();
                                row_message2["MSGID"] = Guid.NewGuid();
                                row_message2["SSOPST"] = register.GetValue();
                                row_message2["MSGTYPE"] = message_id.Account_KeyIn_Withdrawals;
                                row_message2["MSGTITLE"] = "请设置提款密码";
                                row_message2["MSGBODY"] = "<strong>设置提款密码提醒</strong><br/></br/>提款密码用于提取彩咖网账户可用余额的安全密码，请您务必设置<a href='/member/usercenter/zhmm'><strong>提款密码</strong></a>。定期重新设置提款密码能提高您的账户安全性！";
                                row_message2["STATE"] = message_state.UnRead;
                                row_message2["CTIME"] = DateTime.Now;
                                dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].Rows.Add(row_message2);
                            }

                            // 添加提示信息 - 绑定银行卡
                            DataRow row_message3 = AosuApp.Functions.FindRow(string.Format("MSGTYPE={0}", message_id.Account_KeyIn_BankCard), dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"]);
                            if (row_message3 == null)
                            {
                                row_message3 = dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].NewRow();
                                row_message3["MSGID"] = Guid.NewGuid();
                                row_message3["SSOPST"] = register.GetValue();
                                row_message3["MSGTYPE"] = message_id.Account_KeyIn_BankCard;
                                row_message3["MSGTITLE"] = "请绑定银行卡";
                                row_message3["MSGBODY"] = "<strong>绑定银行卡提醒</strong><br/></br/>当您购彩中奖后，彩咖网将会依据您提交的银行卡信息将彩金转入你的真实有效的银行卡账户中；所以请您务必<a href='/member/usercenter/bdyhk'><strong>绑定银行卡</strong></a>，并确保银行卡的真实有效。";
                                row_message3["STATE"] = message_state.UnRead;
                                row_message3["CTIME"] = DateTime.Now;
                                dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].Rows.Add(row_message3);
                            }

                            if (new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader()).IsOK())
                            {
                                // 生成账户的摘要集
                                DictSetUtil ds_summary = new DictSetUtil(new DataSet());
                                ds_summary.SetValue("TOTALAMOUNT", "0.00");  // 账户总额
                                ds_summary.SetValue("AVAILABLE", "0.00"); // 可用余额
                                ds_summary.SetValue("FREEZED", "0.00");   // 冻结金额
                                ds_summary.SetValue("SCORE", "0");    // 积分
                                ds_summary.SetValue("DEPOSITED", "0.0");   // 充值金额
                                ds_summary.SetValue("BONUS", "0.0");   // 中奖金额
                                ds_summary.SetValue("EXCHANGED", "0.0");   // 积分兑换
                                ds_summary.SetValue("RETURNED", "0.0");   // 游戏返点
                                ds_summary.SetValue("CONSUME", "0.0");   // 投注金额
                                ds_summary.SetValue("WITHDRAW", "0.0");   // 提款金额

                                // 通行证+超级密码
                                string key = new DictSetUtil(null).PushSLItem(PickParam(Params).GetValueAsString()).PushSLItem("6B276432FFAF4FD4E086E739009256B3")
                                    .DoSignature();

                                AEntryDic.Pick(GetControl()).SetDic(ds_summary.MyDS, key);
                            }
                            else
                            {
                                PickParam(Params).Clear().SetError("注册失败");
                            }
                        }
                        else
                        {
                            PickParam(Params).Clear().SetError("账户名已存在");
                        }
                    }
                    else
                    {
                        PickParam(Params).Clear().SetError(aPU.GetError());
                    }
                }
                else
                {
                    PickParam(Params).Clear().SetError(register.GetError());
                }
            }
            catch
            {
                PickParam(Params).Clear().SetError("注册失败");
            }
        }

        private void GetAccountSummary(Hashtable Params)
        {
            string passport = PickParam(Params).GetValueAsString();
            if (string.IsNullOrEmpty(passport))
            {
                PickParam(Params).Clear().SetError("未传入通行证");
            }
            else
            {
                // 得到会员信息
                DataSet dsMember = new ParamUtil().SQLCmdLoadData().SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT * FROM CAIKA_MEMBER WHERE SSOPST='{0}'", passport))
                    .ExecuteCmd(ADataLoader.DataLoader()).GetValueAsDataSet();
                if (dsMember.Tables["CAIKA_MEMBER"].Rows.Count == 0)
                {
                    PickParam(Params).Clear().SetError("未找到会员记录");
                }
                else
                {
                    // 通行证+超级密码
                    string key = new DictSetUtil(null).PushSLItem(passport).PushSLItem("6B276432FFAF4FD4E086E739009256B3")
                        .DoSignature();

                    DictSetUtil dsSummary = new DictSetUtil(AEntryDic.Pick().GetDic(key));
                    dsSummary.SetValue("NICKNAME", dsMember.Tables["CAIKA_MEMBER"].Rows[0]["NICKNAME"].ToString());

                    PickParam(Params).Clear();
                    PickParam(Params).SetParam(dsSummary.MyDS);

                    // 将账户摘要同步到Member表中
                    if (dsMember.Tables["CAIKA_MEMBER"].Rows[0]["UPDFLAG"].ToString() != "N")
                    {
                        dsMember.Tables["CAIKA_MEMBER"].Rows[0]["TOTALAMOUNT"] = dsSummary.GetValue("TOTALAMOUNT");
                        dsMember.Tables["CAIKA_MEMBER"].Rows[0]["AVAILABLE"] = dsSummary.GetValue("AVAILABLE");
                        dsMember.Tables["CAIKA_MEMBER"].Rows[0]["FREEZED"] = dsSummary.GetValue("FREEZED");
                        dsMember.Tables["CAIKA_MEMBER"].Rows[0]["SCORE"] = dsSummary.GetValue("SCORE");
                        dsMember.Tables["CAIKA_MEMBER"].Rows[0]["UPDFLAG"] = "N";

                        new ParamUtil().SQLCmdPersistent().SetParam(dsMember).ExecuteCmd(ADataLoader.DataLoader());
                    }
                }
            }
        }

        private void AppendJournal(Hashtable Params)
        {
            string username = PickParam(Params).GetValueAsString("username");
            string t_id = PickParam(Params).GetValueAsString("t_id");
            string journal_type = PickParam(Params).GetValueAsString();
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(t_id) || string.IsNullOrEmpty(journal_type))
                {
                    throw new ArgumentException("接收到的参数列表存在错误！");
                }

                string passport = string.Format("{0}@caika.com", username);
                DataSet dsPersistent = new ParamUtil()
                    .SQLCmdLoadData()
                    .SQLEntityScript("CAIKA_MEMBER_JOURNAL", string.Format("SELECT * FROM CAIKA_MEMBER_JOURNAL WHERE ID='{0}' AND SSOPST='{1}'", t_id, passport))
                    .ExecuteCmd(ADataLoader.DataLoader()).GetValueAsDataSet();

                DataRow row_joural = null;
                if (dsPersistent.Tables["CAIKA_MEMBER_JOURNAL"].Rows.Count == 0)
                {
                    row_joural = dsPersistent.Tables["CAIKA_MEMBER_JOURNAL"].NewRow();
                    row_joural["ID"] = t_id;
                    row_joural["SSOPST"] = passport;
                    row_joural["CTIME"] = DateTime.Now;
                    dsPersistent.Tables["CAIKA_MEMBER_JOURNAL"].Rows.Add(row_joural);
                }

                row_joural = dsPersistent.Tables["CAIKA_MEMBER_JOURNAL"].Rows[0];
                row_joural["JOURNALTIME"] = DateTime.Now;
                row_joural["TYPE"] = BASE.journal_type.充值;
                row_joural["AMOUNT"] = PickParam(Params).GetValue("amount");

                // 通行证+超级密码
                string key = new DictSetUtil(null).PushSLItem(passport).PushSLItem("6B276432FFAF4FD4E086E739009256B3")
                    .DoSignature();

                row_joural["AVAILABLE"] = new DictSetUtil(AEntryDic.Pick().GetDic(key)).GetValue("AVAILABLE");
                row_joural["CHECKSIGNATURE"] = new DictSetUtil(null).PushSLItem(passport).PushSLItem(t_id).DoSignature();
                row_joural["STATE"] = journal_state.待审核;
                row_joural["ETIME"] = DateTime.Now;

                // 生成一个流水账的对账单
                DictSetUtil ds_record = new DictSetUtil(new DataSet());
                ds_record.SetValue("channel", PickParam(Params).GetValueAsString("channel"));
                ds_record.SetValue("bank", PickParam(Params).GetValueAsString("bank"));
                ds_record.SetValue("amount", PickParam(Params).GetValueAsString("amount"));
                if (AEntryDic.Pick().SetDic(ds_record.MyDS, row_joural.Field<string>("CHECKSIGNATURE")))
                {
                    new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader());
                }
            }
            catch (Exception ex1)
            {
                PickParam(Params).Clear().SetError(ex1.Message);
            }
        }

        private void GetJournalList(Hashtable Params)
        {
            string account = PickParam(Params).GetValueAsString();
            if (string.IsNullOrEmpty(account))
            {
                throw new ArgumentException("接收到的参数列表存在错误！");
            }

            string type = PickParam(Params).GetValueAsString("type");
            string time = PickParam(Params).GetValueAsString("time");
            string pageno = PickParam(Params).GetValueAsString("pageno");
            if (string.IsNullOrEmpty(pageno))
            {
                pageno = "1";
            }

            try
            {
                switch (time)
                {
                    case "day1":
                        time = DateTime.Now.ToString("yyyy-MM-dd");
                        break;
                    case "month1":
                        time = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                        break;
                    case "month3":
                        time = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd");
                        break;
                    case "month6":
                        time = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
                        break;
                    default:
                        time = DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
                        break;
                }

                string script = string.Format("SELECT * FROM CAIKA_MEMBER_JOURNAL WHERE ACCOUNTID='{0}' AND JOURNALTIME>='{1}'", account, time);
                switch (type)
                {
                    case "deposit":
                        script += string.Format(" AND TYPE='{0}'", journal_type.充值);
                        break;
                    case "withdraw":
                        script += string.Format(" AND TYPE='{0}'", journal_type.提现);
                        break;
                    case "consume":
                        script += string.Format(" AND TYPE='{0}'", journal_type.投注);
                        break;
                    case "bonus":
                        script += string.Format(" AND TYPE='{0}'", journal_type.中奖);
                        break;
                    case "cancel":
                        script += string.Format(" AND TYPE='{0}'", journal_type.撤单);
                        break;
                    case "returned":
                        script += string.Format(" AND TYPE='{0}'", journal_type.返点);
                        break;
                    case "exchanged":
                        script += string.Format(" AND TYPE='{0}'", journal_type.积分兑换);
                        break;
                    case "activity":
                        script += string.Format(" AND TYPE='{0}'", journal_type.活动派送);
                        break;
                }

                DataSet ds_list = new ParamUtil()
                    .SQLCmdLoadData()
                    .SQLWithOutSchema()
                    .SQLEntityScript(DictSet.FN_MCCanShu, string.Format("SELECT TYPE MCCanShu,SUM(REQAMOUNT) CanShuZhi FROM CAIKA_MEMBER_JOURNAL WHERE ACCOUNTID='{0}' AND JOURNALTIME>='{1}' AND STATE='{2}' GROUP BY TYPE", account, time, journal_state.已审核))
                    .SQLEntityScript("list", string.Format(@"SELECT TOP 30 * FROM (SELECT ROW_NUMBER() OVER (ORDER BY JOURNALTIME DESC) ROWNUM,T1.* FROM ({0}) T1) AS A WHERE ROWNUM > 30*({1}-1)", script, pageno))
                    .SQLEntityScript("pages", string.Format("SELECT CEILING(CAST(COUNT(*) AS FLOAT)/30) FROM CAIKA_MEMBER_JOURNAL WHERE ACCOUNTID='{0}' AND JOURNALTIME>='{1}'", account, time))
                    .ExecuteCmd(ADataLoader.DataLoader())
                    .GetValueAsDataSet();

                PickParam(Params).Clear();
                PickParam(Params).SetParam(ds_list);
            }
            catch (Exception ex1)
            {
                PickParam(Params).SetError(ex1.Message);
            }
        }

        private void VerifyPasswrod(Hashtable Params)
        {
            string account = PickParam(Params).GetValueAsString();
            string pwd = PickParam(Params).GetValueAsString("pwd");
            string type = PickParam(Params).GetValueAsString("type");
            try
            {
                if (type == "pay")
                {
                    DataSet dsMember = new ParamUtil()
                        .SQLCmdLoadData()
                        .SQLWithOutSchema()
                        .SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT * FROM CAIKA_MEMBER WHERE SSOPST='{0}@caika.com'", account))
                        .ExecuteCmd(ADataLoader.DataLoader())
                        .GetValueAsDataSet();

                    DataRow rowMember = dsMember.Tables[0].Rows[0];
                    string signature = new DictSetUtil(null).DoSignature(pwd);
                    if (signature == rowMember.Field<string>("WITHDRAWALS").Trim())
                    {
                        PickParam(Params).Clear().SetParam(true);
                    }
                    else
                    {
                        PickParam(Params).Clear().SetError("密码输入不正确");
                        PickParam(Params).SetParam(false);
                    }
                }
                else
                {
                    // 得到通行证数据
                    DataSet dsPassport = new ParamUtil()
                        .SQLCmdLoadData()
                        .SQLEntityScript("SSODOMAINREF", string.Format("SELECT * FROM SSODOMAINREF WHERE DOMAINNAME='caika.com' AND DOMAINUSER='{0}'", account))
                        .SQLEntityScript("SSOPASSPORT", string.Format("SELECT * FROM SSOPASSPORT WHERE SSOPST = '{0}@caika.com'", account))
                        .ExecuteCmd(ADataLoader.DataLoader())
                        .GetValueAsDataSet();

                    if (dsPassport.Tables["SSODOMAINREF"].Rows.Count == 0 || dsPassport.Tables["SSOPASSPORT"].Rows.Count == 0)
                    {
                        PickParam(Params).Clear().SetError("用户名或密码错误");
                        PickParam(Params).SetParam(false);
                    }
                    else
                    {
                        ParamUtil aPU = new ParamUtil().SetCmd(CVerifyPasswrod).SetParam(dsPassport).SetParam("PSTPWD", pwd).ExecuteCmd(new APassport());
                        if (aPU.IsOK())
                        {
                            PickParam(Params).Clear();
                            PickParam(Params).SetParam(aPU.GetValue());
                        }
                        else
                        {
                            PickParam(Params).Clear().SetError(aPU.GetError());
                            PickParam(Params).SetParam(false);
                        }
                    }
                }
            }
            catch (Exception)
            {
                PickParam(Params).Clear().SetError("校验密码出错");
                PickParam(Params).SetParam(false);
            }
        }

        private void ChangePassword(Hashtable Params)
        {
            string account = PickParam(Params).GetValueAsString();
            string type = PickParam(Params).GetValueAsString("type");
            try
            {
                string c_pwd = PickParam(Params).GetValueAsString("c_pass");
                string n_pwd = PickParam(Params).GetValueAsString("n_pass");
                if (string.IsNullOrEmpty(n_pwd))
                {
                    throw new ArgumentNullException("n_pwd", "未传入新密码");
                }

                if (type == "pay")
                {
                    // 是否设置过提款密码？
                    if (AAccountUtil.WithdrawIsEmpty(account) == false)
                    {
                        if (string.IsNullOrEmpty(c_pwd))
                        {
                            throw new ArgumentNullException("c_pwd", "未传入当前密码");
                        }

                        // 验证密码
                        ParamUtil verifyPwd = new ParamUtil().SetCmd(CVerifyPasswrod).SetParam(account).SetParam("pwd", c_pwd).SetParam("type", type).ExecuteCmd(this);
                        if (verifyPwd.GetValueAsBool() == false)
                        {
                            throw new Exception(verifyPwd.GetError());
                        }
                    }

                    // 修改密码
                    DataSet dsMember = new ParamUtil()
                        .SQLCmdLoadData()
                        .SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT * FROM CAIKA_MEMBER WHERE SSOPST='{0}@caika.com'", account))
                        .ExecuteCmd(ADataLoader.DataLoader())
                        .GetValueAsDataSet();

                    dsMember.Tables[0].Rows[0]["WITHDRAWALS"] = new DictSetUtil(null).DoSignature(n_pwd);
                    dsMember.Tables[0].Rows[0]["ETIME"] = DateTime.Now;
                    new ParamUtil().SQLCmdPersistent().SetParam(dsMember).ExecuteCmd(ADataLoader.DataLoader());
                }
                else
                {
                    if (string.IsNullOrEmpty(c_pwd))
                    {
                        throw new ArgumentNullException("c_pwd", "未传入当前密码");
                    }

                    ParamUtil aPU = new ParamUtil()
                        .SetCmd(CChangePassword)
                        .SetParam("DOMAINUSER", account)
                        .SetParam("DOMAINNAME", BaseControl.GlobalControl.GetContext()[Context.CAppName])
                        .SetParam("PSTPWD", c_pwd)
                        .SetParam("NEWPWD", n_pwd)
                        .ExecuteCmd(new APassport());

                    PickParam(Params).Clear().SetError(aPU.GetError());
                }
            }
            catch (Exception ex1)
            {
                PickParam(Params).Clear().SetError(ex1.Message);
            }
        }

        private void ChangeBank(Hashtable Params)
        {
            string account = PickParam(Params).GetValueAsString();
            string c_pass = PickParam(Params).GetValueAsString("c_pass");
            string realname = PickParam(Params).GetValueAsString("realname");
            DataSet ds_digest = PickParam(Params).GetValueAsDataSet("digest");
            try
            {
                if (string.IsNullOrEmpty(account))
                {
                    throw new ArgumentNullException("account", "未传入账户名");
                }

                if (string.IsNullOrEmpty(c_pass))
                {
                    throw new ArgumentNullException("c_pass", "未传入支付密码");
                }

                if (string.IsNullOrEmpty(realname))
                {
                    throw new ArgumentNullException("realname", "未传入开户人姓名");
                }

                if (ds_digest == null)
                {
                    throw new ArgumentNullException("digest", "未传入银行卡签名数据");
                }

                // 验证密码
                ParamUtil verifyPwd = new ParamUtil().SetCmd(CVerifyPasswrod).SetParam(account).SetParam("pwd", c_pass).SetParam("type", "pay").ExecuteCmd(this);
                if (verifyPwd.GetValueAsBool() == false)
                {
                    throw new Exception(verifyPwd.GetError());
                }
                else
                {
                    // 修改银行卡信息
                    string passport = string.Format("{0}@caika.com", account);
                    DataSet dsMember = new ParamUtil()
                        .SQLCmdLoadData()
                        .SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT * FROM CAIKA_MEMBER WHERE SSOPST='{0}'", passport))
                        .ExecuteCmd(ADataLoader.DataLoader())
                        .GetValueAsDataSet();

                    string key = new DictSetUtil(null).PushSLItem(passport).PushSLItem(c_pass).PushSLItem("Bank").DoSignature();
                    if (AEntryDic.Pick(GetControl()).SetDic(ds_digest, key))
                    {
                        dsMember.Tables[0].Rows[0]["REALNAME"] = realname;
                        dsMember.Tables[0].Rows[0]["BANKDIGEST"] = key;
                        dsMember.Tables[0].Rows[0]["ETIME"] = DateTime.Now;
                        new ParamUtil().SQLCmdPersistent().SetParam(dsMember).ExecuteCmd(ADataLoader.DataLoader());
                    }
                }
            }
            catch (Exception ex1)
            {
                PickParam(Params).Clear().SetError(ex1.Message);
            }
        }

        private void PostMemberInfo(Hashtable Params)
        {
            string account = PickParam(Params).GetValueAsString();
            string c_pass = PickParam(Params).GetValueAsString("c_pass");
            DataSet ds_member = PickParam(Params).GetValueAsDataSet("member");
            try
            {
                if (string.IsNullOrEmpty(account))
                {
                    throw new ArgumentNullException("account", "未传入账户名");
                }

                if (string.IsNullOrEmpty(c_pass))
                {
                    throw new ArgumentNullException("c_pass", "未传入登录密码");
                }

                if (ds_member == null)
                {
                    throw new ArgumentNullException("member", "未传入会员数据");
                }

                DictSetUtil dsinfo = new DictSetUtil(ds_member);

                // 验证密码
                ParamUtil verifyPwd = new ParamUtil().SetCmd(CVerifyPasswrod).SetParam(account).SetParam("pwd", c_pass).SetParam("type", "pay").ExecuteCmd(this);
                if (verifyPwd.GetValueAsBool() == false)
                {
                    throw new Exception(verifyPwd.GetError());
                }
                else
                {
                    // 修改会员信息
                    string passport = string.Format("{0}@caika.com", account);
                    DataSet dsMember = new ParamUtil()
                        .SQLCmdLoadData()
                        .SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT * FROM CAIKA_MEMBER WHERE SSOPST='{0}'", passport))
                        .ExecuteCmd(ADataLoader.DataLoader())
                        .GetValueAsDataSet();

                    dsMember.Tables[0].Rows[0]["NICKNAME"] = dsinfo.GetValue("nickname");
                    dsMember.Tables[0].Rows[0]["EMAIL"] = dsinfo.GetValue("email");
                    dsMember.Tables[0].Rows[0]["REALNAME"] = dsinfo.GetValue("realname");
                    dsMember.Tables[0].Rows[0]["IDTYPE"] = dsinfo.GetValue("idtype");
                    dsMember.Tables[0].Rows[0]["UID"] = dsinfo.GetValue("id");
                    dsMember.Tables[0].Rows[0]["ASKID"] = dsinfo.GetValue("ask");
                    dsMember.Tables[0].Rows[0]["ANSWER"] = dsinfo.GetValue("answer");
                    dsMember.Tables[0].Rows[0]["ETIME"] = DateTime.Now;
                    new ParamUtil().SQLCmdPersistent().SetParam(dsMember).ExecuteCmd(ADataLoader.DataLoader());
                }
            }
            catch (Exception ex1)
            {
                PickParam(Params).Clear().SetError(ex1.Message);
            }
        }

        private void PostRequest(Hashtable Params)
        {
            string account = PickParam(Params).GetValueAsString();
            string type = PickParam(Params).GetValueAsString("type");
            string msgid = PickParam(Params).GetValueAsString("msgid");
            try
            {
                if (string.IsNullOrEmpty(account))
                {
                    throw new ArgumentNullException("account", "未传入账户名");
                }

                if (string.IsNullOrEmpty(type))
                {
                    throw new ArgumentNullException("type", "未传入请求类型");
                }

                if (string.IsNullOrEmpty(msgid))
                {
                    throw new ArgumentNullException("msgid", "未传入请求ID");
                }

                string uid = new DictSetUtil(null).DoSignature(msgid);

                // 修改会员信息
                string passport = string.Format("{0}@caika.com", account);
                DataSet dsPersistent = new ParamUtil()
                    .SQLCmdLoadData()
                    .SQLEntityScript("CAIKA_MEMBER_MESSAGE", string.Format("SELECT * FROM CAIKA_MEMBER_MESSAGE WHERE MSGID='{0}'", uid))
                    .ExecuteCmd(ADataLoader.DataLoader())
                    .GetValueAsDataSet();
                if (dsPersistent.Tables[0].Rows.Count == 0)
                {
                    DataRow row_message1 = dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].NewRow();
                    row_message1["MSGID"] = uid;
                    row_message1["SSOPST"] = "admin@caika.system";
                    switch (type)
                    {
                        case "1":
                            row_message1["MSGTYPE"] = message_id.UserRequest_ChangeMemberInfo;
                            row_message1["MSGTITLE"] = string.Format("{0}提交修改个人资料的请求", passport);
                            break;
                        case "2":
                            row_message1["MSGTYPE"] = message_id.UserRequest_ChangePayPassword;
                            row_message1["MSGTITLE"] = string.Format("{0}提交修改支付密码的请求", passport);
                            break;
                    }
                    row_message1["MSGBODY"] = "";
                    row_message1["STATE"] = message_state.UnRead;
                    row_message1["CTIME"] = DateTime.Now;
                    dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].Rows.Add(row_message1);

                    new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader());
                }
            }
            catch (Exception ex1)
            {
                PickParam(Params).Clear().SetError(ex1.Message);
            }
        }
    }

    //public class AAccountUtil1 : APassport
    //{
    //    /// <summary>
    //    /// 注册一个通行证，同时创建为用户创建一个彩咖网会员账户。如果是创建代理通行证，则在创建完通行证后会为代理同时创建一个会员账户和代理账户。
    //    /// <para>输入参数：</para>
    //    /// <para>DOMAINUSER：账号名；必选参数</para>
    //    /// <para>DOMAINNAME：应用域名称；必选参数</para>
    //    /// <para>PSTPWD：密码；必选参数。</para>
    //    /// <para>EFFECT：开放天数，单位：day；可选参数，默认为-1</para>
    //    /// <para>输出参数：</para>
    //    /// <para>RetResult：通行证</para>
    //    /// <para>RetErrorMsg：错误描述</para>
    //    /// </summary>
    //    public const string CCreateNewAccount = "CCreateNewAccount";
    //    public const string CBindAgent = "CBindAgent";
    //    public const string CGetDigest = "CGetDigest";
    //    public const string CAppendJournal = "CAppendJournal";
    //    public const string CCreateNewMessage = "CCreateNewMessage";

    //    public const string CRequestInfoChange = "CRequestInfoChange";
    //    public const string CRequestBankChange = "CRequestBankChange";
    //    public const string CRequestPasswordChange = "CRequestPasswordChange";

    //    public const string CListMember = "CListMember";
    //    public const string CListAgent = "CListAgent";
    //    public const string CListJournal = "CListJournal";
    //    public const string CListMessage = "CListMessage";

    //    protected override void Execute(Hashtable Params)
    //    {
    //        switch (ActionUtil.GetCmd(Params))
    //        {
    //            case CCreateNewAccount:
    //                CreateNewAccount(Params);
    //                break;
    //            default:
    //                base.Execute(Params);
    //                break;
    //        }
    //    }

    //    private void CreateNewAccount(Hashtable Params)
    //    {
    //        string account = PickParam(Params).GetValueAsString("DOMAINUSER");
    //        string domain = PickParam(Params).GetValueAsString("DOMAINNAME");
    //        string pstpwd = PickParam(Params).GetValueAsString("PSTPWD");
    //        try
    //        {
    //            // 注册通行证
    //            ParamUtil register = PickParam(Params).SetCmd(APassport.CRegister).ExecuteCmd(new APassport());
    //            if (register.IsOK())
    //            {
    //                ParamUtil aPU = new ParamUtil()
    //                    .SQLCmdLoadData()
    //                    .SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT * FROM CAIKA_MEMBER WHERE SSOPST='{0}'", register.GetValue()))
    //                    .SQLEntityScript("CAIKA_MEMBER_MESSAGE", string.Format("SELECT * FROM CAIKA_MEMBER_MESSAGE WHERE SSOPST='{0}'", register.GetValue()))
    //                    .ExecuteCmd(ADataLoader.DataLoader());
    //                if (aPU.IsOK())
    //                {
    //                    DataSet dsPersistent = aPU.GetValueAsDataSet();
    //                    if (dsPersistent.Tables["CAIKA_MEMBER"].Rows.Count == 0)
    //                    {
    //                        DataRow rowNew = dsPersistent.Tables["CAIKA_MEMBER"].NewRow();
    //                        rowNew["SSOPST"] = register.GetValue();
    //                        rowNew["NICKNAME"] = account;
    //                        rowNew["UPDFLAG"] = "Y";
    //                        rowNew["STATE"] = AosuApp.state.Enabled;
    //                        rowNew["CTIME"] = DateTime.Now;
    //                        rowNew["ETIME"] = DateTime.Now;
    //                        dsPersistent.Tables["CAIKA_MEMBER"].Rows.Add(rowNew);

    //                        // 添加提示信息 - 详细资料
    //                        DataRow row_message1 = AosuApp.Functions.FindRow(string.Format("MSGTYPE={0}", message_id.Account_KeyIn_DetailInfo), dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"]);
    //                        if (row_message1 == null)
    //                        {
    //                            row_message1 = dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].NewRow();
    //                            row_message1["MSGID"] = Guid.NewGuid();
    //                            row_message1["SSOPST"] = register.GetValue();
    //                            row_message1["MSGTYPE"] = message_id.Account_KeyIn_DetailInfo;
    //                            row_message1["MSGTITLE"] = "请完善用户基本资料";
    //                            row_message1["MSGBODY"] = "<strong>完善用户基本资料提醒</strong><br/></br/>为了确保彩咖网更好的服务于您，请您完善<a href='/member/usercenter/grzl'><strong>个人资料信息</strong></a>。";
    //                            row_message1["STATE"] = message_state.UnRead;
    //                            row_message1["CTIME"] = DateTime.Now;
    //                            dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].Rows.Add(row_message1);
    //                        }

    //                        // 添加提示信息 - 设置提款密码
    //                        DataRow row_message2 = AosuApp.Functions.FindRow(string.Format("MSGTYPE={0}", message_id.Account_KeyIn_Withdrawals), dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"]);
    //                        if (row_message2 == null)
    //                        {
    //                            row_message2 = dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].NewRow();
    //                            row_message2["MSGID"] = Guid.NewGuid();
    //                            row_message2["SSOPST"] = register.GetValue();
    //                            row_message2["MSGTYPE"] = message_id.Account_KeyIn_Withdrawals;
    //                            row_message2["MSGTITLE"] = "请设置提款密码";
    //                            row_message2["MSGBODY"] = "<strong>设置提款密码提醒</strong><br/></br/>提款密码用于提取彩咖网账户可用余额的安全密码，请您务必设置<a href='/member/usercenter/zhmm'><strong>提款密码</strong></a>。定期重新设置提款密码能提高您的账户安全性！";
    //                            row_message2["STATE"] = message_state.UnRead;
    //                            row_message2["CTIME"] = DateTime.Now;
    //                            dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].Rows.Add(row_message2);
    //                        }

    //                        // 添加提示信息 - 绑定银行卡
    //                        DataRow row_message3 = AosuApp.Functions.FindRow(string.Format("MSGTYPE={0}", message_id.Account_KeyIn_BankCard), dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"]);
    //                        if (row_message3 == null)
    //                        {
    //                            row_message3 = dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].NewRow();
    //                            row_message3["MSGID"] = Guid.NewGuid();
    //                            row_message3["SSOPST"] = register.GetValue();
    //                            row_message3["MSGTYPE"] = message_id.Account_KeyIn_BankCard;
    //                            row_message3["MSGTITLE"] = "请绑定银行卡";
    //                            row_message3["MSGBODY"] = "<strong>绑定银行卡提醒</strong><br/></br/>当您购彩中奖后，彩咖网将会依据您提交的银行卡信息将彩金转入你的真实有效的银行卡账户中；所以请您务必<a href='/member/usercenter/bdyhk'><strong>绑定银行卡</strong></a>，并确保银行卡的真实有效。";
    //                            row_message3["STATE"] = message_state.UnRead;
    //                            row_message3["CTIME"] = DateTime.Now;
    //                            dsPersistent.Tables["CAIKA_MEMBER_MESSAGE"].Rows.Add(row_message3);
    //                        }

    //                        if (new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader()).IsOK())
    //                        {
    //                            // 生成账户的摘要集
    //                            DictSetUtil ds_summary = new DictSetUtil(new DataSet());
    //                            ds_summary.SetValue("TOTALAMOUNT", "0.00");  // 账户总额
    //                            ds_summary.SetValue("AVAILABLE", "0.00"); // 可用余额
    //                            ds_summary.SetValue("FREEZED", "0.00");   // 冻结金额
    //                            ds_summary.SetValue("SCORE", "0");    // 积分
    //                            ds_summary.SetValue("DEPOSITED", "0.0");   // 充值金额
    //                            ds_summary.SetValue("BONUS", "0.0");   // 中奖金额
    //                            ds_summary.SetValue("EXCHANGED", "0.0");   // 积分兑换
    //                            ds_summary.SetValue("RETURNED", "0.0");   // 游戏返点
    //                            ds_summary.SetValue("CONSUME", "0.0");   // 投注金额
    //                            ds_summary.SetValue("WITHDRAW", "0.0");   // 提款金额

    //                            // 通行证+超级密码
    //                            string key = new DictSetUtil(null).PushSLItem(PickParam(Params).GetValueAsString()).PushSLItem("6B276432FFAF4FD4E086E739009256B3")
    //                                .DoSignature();

    //                            AEntryDic.Pick(GetControl()).SetDic(ds_summary.MyDS, key);
    //                        }
    //                        else
    //                        {
    //                            PickParam(Params).Clear().SetError("注册失败");
    //                        }
    //                    }
    //                    else
    //                    {
    //                        PickParam(Params).Clear().SetError("账户名已存在");
    //                    }
    //                }
    //                else
    //                {
    //                    PickParam(Params).Clear().SetError(aPU.GetError());
    //                }
    //            }
    //            else
    //            {
    //                PickParam(Params).Clear().SetError(register.GetError());
    //            }
    //        }
    //        catch
    //        {
    //            PickParam(Params).Clear().SetError("注册失败");
    //        }
    //    }
    //}
}