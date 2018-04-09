using AosuApp;
using AosuApp.AosuFramework;
using System;
using System.Collections;
using System.Data;

namespace API.CP.BASE
{
    public static class MemberDBUtils
    {
        /// <summary>
        /// 获取会员账号的资金摘要信息
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static DataSet GetMemberDigest(IControl objControl, string accountId = "")
        {
            DictSetUtil digestDS = new DictSetUtil(new DataSet());
            try
            {
                digestDS.SetValue("TOTALAMOUNT", "0.00");  // 账户总额
                digestDS.SetValue("AVAILABLE", "0.00"); // 可用余额
                digestDS.SetValue("FREEZED", "0.00");   // 冻结金额
                digestDS.SetValue("SCORE", "0");    // 积分
                digestDS.SetValue("DEPOSITED", "0.0");   // 充值金额
                digestDS.SetValue("BONUS", "0.0");   // 中奖金额
                digestDS.SetValue("EXCHANGED", "0.0");   // 积分兑换
                digestDS.SetValue("RETURNED", "0.0");   // 游戏返点
                digestDS.SetValue("CONSUME", "0.0");   // 投注金额
                digestDS.SetValue("WITHDRAW", "0.0");   // 提款金额

                // 得到会员账号的资金摘要信息
                if (!string.IsNullOrEmpty(accountId))
                {
                    ParamUtil aPU = new ParamUtil()
                        .SQLCmdLoadData()
                        .SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT * FROM CAIKA_MEMBER WHERE ACCOUNTID='{0}'", accountId))
                        .SQLWithOutSchema()
                        .ExecuteCmd(ADataLoader.DataLoader());
                    if (aPU.IsOK())
                    {
                        DataSet dsDigest = AEntryDic.Pick(objControl).GetDic(aPU.GetValueAsDataSet().Tables["CAIKA_MEMBER"].Rows[0].Field<string>("DIGEST"));
                        if (dsDigest != null)
                        {
                            digestDS.MyDS.Merge(dsDigest);
                            digestDS.AcceptChanges();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return digestDS.MyDS;
        }
        /// <summary>
        /// 会员账号是否存在？
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static bool MemberIsExist(IControl objControl, string accountId)
        {
            ParamUtil aPU = new ParamUtil();
            aPU.SQLCmdLoadData();
            aPU.SQLWithOutSchema();
            aPU.SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT ACCOUNTID FROM CAIKA_MEMBER WHERE ACCOUNTID='{0}'", accountId));
            if (aPU.ExecuteCmd(ADataLoader.DataLoader()).IsOK())
            {
                return aPU.GetValueAsDataSet().Tables["CAIKA_MEMBER"].Rows.Count > 0;
            }

            return false;
        }
        /// <summary>
        /// 创建一个新的会员账号
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="Params"></param>
        public static void CreateMemberRecord(IControl objControl, Hashtable Params)
        {
            Hashtable aHT = new Hashtable();
            ParamUtil.Pick(aHT).SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT * FROM CAIKA_MEMBER WHERE ACCOUNTID='{0}'", Params["DOMAINUSER"]));
            ParamUtil.Pick(aHT).SQLEntityScript("BASE_CATEGORY", string.Format("SELECT * FROM BASE_CATEGORY WHERE CATEGORYID LIKE '{0}%' ORDER BY CATEGORYID DESC", Params["PARENTAGENT"]));

            // 得到一个待持久化的记录集
            if (ParamUtil.Pick(aHT).SQLCmdLoadData().ExecuteCmd(ADataLoader.DataLoader()).IsOK())
            {
                DataSet dsPersistent = ParamUtil.Pick(aHT).GetValueAsDataSet();
                if (dsPersistent.Tables["CAIKA_MEMBER"].Rows.Count == 0)
                {
                    // 生成当前账号的代理编号
                    if (ParamUtil.Pick(Params).IsNullOrEmpty("PARENTAGENT") || dsPersistent.Tables["BASE_CATEGORY"].Rows.Count == 0)
                    {
                        throw new Exception("参数列表中没有解析到上级代理商编号。");
                    }

                    string strAgentCode = "";
                    int intLevel = 0;
                    int intSeqNo = 0;
                    if (dsPersistent.Tables["BASE_CATEGORY"].Rows.Count == 1)
                    {
                        strAgentCode = string.Format("{0}001", Params["PARENTAGENT"]);
                        intLevel = Convert.ToInt32(dsPersistent.Tables["BASE_CATEGORY"].Rows[0]["LEVEL"]) + 1;
                        intSeqNo = 1;
                    }
                    else
                    {
                        int idx = 0;
                        string curCode = dsPersistent.Tables["BASE_CATEGORY"].Rows[0]["CATEGORYID"] + "";
                        if (!int.TryParse(curCode.Substring(curCode.Length - 3), out idx))
                        {
                            throw new Exception(string.Format("解析代理商{0}的索引值出错。", dsPersistent.Tables["BASE_CATEGORY"].Rows[0]["CATEGORYID"]));
                        }

                        if ((idx + 1) > 998)
                        {
                            throw new Exception(string.Format("代理商{0}的下级代理数量已满。", Params["PARENTAGENT"]));
                        }

                        strAgentCode = Params["PARENTAGENT"] + (idx + 1).ToString().PadLeft(3, '0');
                        intLevel = Convert.ToInt32(dsPersistent.Tables["BASE_CATEGORY"].Rows[0]["LEVEL"]);
                        intSeqNo = Convert.ToInt32(dsPersistent.Tables["BASE_CATEGORY"].Rows[0]["SEQNO"]) + 1;
                    }

                    DataRow rowNew = dsPersistent.Tables["CAIKA_MEMBER"].NewRow();
                    rowNew["ACCOUNTID"] = Params["DOMAINUSER"];
                    rowNew["NICKNAME"] = Params["DOMAINUSER"];
                    rowNew["TYPE"] = Params["TYPE"];
                    if (Params["TYPE"].ToString() == account_type.AgentAccount)
                    {
                        rowNew["LEVEL"] = member_level.L3;
                    }
                    else
                    {
                        rowNew["LEVEL"] = member_level.L5;
                    }

                    rowNew["AGENTCODE"] = strAgentCode;
                    rowNew["CERTTYPE"] = "身份证";
                    rowNew["STATE"] = state.Enabled;
                    rowNew["CTIME"] = DateTime.Now;
                    rowNew["CUSER"] = objControl.GetContext().MyInfo["USERID"];
                    rowNew["ETIME"] = DateTime.Now;
                    rowNew["EUSER"] = objControl.GetContext().MyInfo["USERID"];

                    // 通行证+超级密码
                    string key = new DictSetUtil(null)
                        .PushSLItem(string.Format("{0}@{1}", Params["DOMAINUSER"], Params["DOMAINNAME"]))
                        .DoSignature();

                    rowNew["DIGEST"] = key;

                    DictSetUtil dictDigest = new DictSetUtil(ParamUtil.Pick(Params).GetValueAsDataSet());
                    rowNew["TOTALAMOUNT"] = dictDigest.GetValue("TOTALAMOUNT");
                    rowNew["AVAILABLE"] = dictDigest.GetValue("AVAILABLE");
                    rowNew["FREEZED"] = dictDigest.GetValue("FREEZED");
                    rowNew["SCORE"] = dictDigest.GetValue("SCORE");

                    dsPersistent.Tables["CAIKA_MEMBER"].Rows.Add(rowNew);

                    DataRow rowAgent = dsPersistent.Tables["BASE_CATEGORY"].NewRow();
                    rowAgent["CATEGORYID"] = strAgentCode;
                    rowAgent["NAME"] = Params["DOMAINUSER"];
                    rowAgent["DESC"] = Params["DOMAINUSER"];
                    rowAgent["PARENTID"] = Params["PARENTAGENT"];
                    rowAgent["LEVEL"] = intLevel;
                    rowAgent["SEQNO"] = intSeqNo;
                    rowAgent["ISEND"] = "Y";
                    rowAgent["CTYPE"] = category.CType_Label;
                    rowAgent["STATE"] = state.Enabled;
                    rowAgent["CTIME"] = DateTime.Now;
                    rowAgent["CUSER"] = objControl.GetContext().MyInfo["USERID"];
                    rowAgent["ETIME"] = DateTime.Now;
                    rowAgent["EUSER"] = objControl.GetContext().MyInfo["USERID"];

                    dsPersistent.Tables["BASE_CATEGORY"].Rows.Add(rowAgent);
                    dsPersistent.Tables["BASE_CATEGORY"].Rows[dsPersistent.Tables["BASE_CATEGORY"].Rows.Count - 1]["ISEND"] = "N";

                    if (new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader()).IsOK())
                    {
                        // 会员账户的金额摘要数据
                        if (AEntryDic.Pick(objControl).SetDic(dictDigest.MyDS, key))
                        {
                            ParamUtil.Pick(Params).Clear();
                            ParamUtil.Pick(Params).SetError(ActionUtil.DefaultError);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 返回会员账户信息及通行证数据
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="account"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static DataSet GetMemberAndPassport(IControl objControl, string account, string domain)
        {
            Hashtable aHT = new Hashtable();
            ParamUtil.Pick(aHT).SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT * FROM CAIKA_MEMBER WHERE ACCOUNTID='{0}'", account));
            ParamUtil.Pick(aHT).SQLEntityScript("SSODOMAINREF", string.Format("SELECT * FROM SSODOMAINREF WHERE DOMAINUSER='{0}' AND DOMAINNAME='{1}'", account, domain));
            ParamUtil.Pick(aHT).SQLEntityScript("SSOPASSPORT", string.Format("SELECT * FROM SSOPASSPORT WHERE SSOPST=(SELECT SSOPST FROM SSODOMAINREF WHERE DOMAINUSER='{0}' AND DOMAINNAME='{1}')", account, domain));

            // 得到会员账户信息（包括通行证）
            if (ParamUtil.Pick(aHT).SQLCmdLoadData().ExecuteCmd(ADataLoader.DataLoader()).IsOK())
            {
                return (DataSet)aHT[ActionUtil.RetResult];
            }

            return null;
        }
        /// <summary>
        /// 返回会员账号的信息
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static Hashtable GetMemberInfo(IControl objControl, string accountId)
        {
            // 判断会员账户是否存在？
            if (!MemberIsExist(objControl, accountId))
                return new Hashtable();

            try
            {
                ParamUtil aPU = new ParamUtil();
                aPU.SQLCmdLoadData();
                aPU.SQLWithOutSchema();
                aPU.SQLEntityScript("CAIKA_MEMBER", string.Format("SELECT ACCOUNTID,NICKNAME,TYPE,LEVEL,PHONE,EMAIL,REALNAME,CERTTYPE,CERTNUMS,ASKID1,ANSWER1,ASKID2,ANSWER2,DIGEST,STATE FROM CAIKA_MEMBER WHERE ACCOUNTID='{0}'", accountId));
                if (aPU.ExecuteCmd(ADataLoader.DataLoader()).IsOK())
                {
                    return new UriUtil()
                        .ImportRow(aPU.GetValueAsDataSet().Tables["CAIKA_MEMBER"].Rows[0])
                        .ImportHashtable(ParamUtil.Pick().ImportSets(GetMemberDigest(objControl, accountId)).ParamTable)
                        .ExportHashtable();
                }
            }
            catch (Exception)
            {
            }

            return new Hashtable();
        }

        /// <summary>
        /// 充值申请
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="Params"></param>
        public static void RequestDeposit(IControl objControl, Hashtable Params)
        {

        }
        /// <summary>
        /// 提交充值申请
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="Params"></param>
        public static void SubmitDeposit(IControl objControl, Hashtable Params)
        {

        }
        /// <summary>
        /// 用户充值申请审核通过
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="Params"></param>
        public static void ApprovedDeposit(IControl objControl, Hashtable Params)
        {

        }
        /// <summary>
        /// 用户充值申请审核拒绝
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="Params"></param>
        public static void RejectDeposit(IControl objControl, Hashtable Params)
        {

        }
        /// <summary>
        /// 提款申请
        /// </summary>
        /// <param name="objControl"></param>
        /// <param name="Params"></param>
        public static void RequestWithdraw(IControl objControl, Hashtable Params)
        {

        }
    }
}