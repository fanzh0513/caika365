using AosuApp;
using AosuApp.AosuData;
using AosuApp.AosuFramework;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace API.CP.SSC
{
    public class A360Parser : BaseAction
    {
        protected override void Execute(Hashtable Params)
        {
            switch (ActionUtil.GetCmd(Params))
            {
                case CIssued4SSC:
                    Issue4SSC(Params);
                    break;
                case CBonus4SSC:
                    Bonus4SSC(Params);
                    break;
                case CBonus4SSCHistory:
                    Bonus4SSCHistory(Params);
                    break;
            }
        }

        /// <summary>
        /// 发布
        /// </summary>
        public const string CIssued4SSC = "CIssued4SSC";
        private void Issue4SSC(Hashtable Params)
        {
            // 得到时时彩的彩种编号（彩票360提供）
            string lotteryId = PickParam(Params).GetValueAsString();
            if (string.IsNullOrEmpty(lotteryId))
            {
                throw new ArgumentNullException(ActionUtil.RetResult, "未传入时时彩彩种编号！");
            }

            string lotteryType = PickParam(Params).GetValueAsString("LotteryType");
            if (string.IsNullOrEmpty(lotteryType))
            {
                throw new ArgumentNullException(ActionUtil.RetResult, "未传入时时彩彩种！");
            }

            string CAIKA_SSCCATE = "";
            string CAIKA_SSC_ISSUED = "";
            switch (lotteryType)
            {
                case 数字彩分类.重庆时时彩:
                    CAIKA_SSCCATE = "CAIKA_SSCCATE";
                    CAIKA_SSC_ISSUED = "CAIKA_SSCCQ_ISSUED";
                    break;
                case 数字彩分类.江西时时彩:
                    CAIKA_SSCCATE = "CAIKA_SSCCATE";
                    CAIKA_SSC_ISSUED = "CAIKA_SSCJX_ISSUED";
                    break;
                case 数字彩分类.新疆时时彩:
                    break;
                case 数字彩分类.天津时时彩:
                    break;
                default:
                    throw new ArgumentException(string.Format("未知彩种!{0}", lotteryType), "LotteryType");
            }

            bool flag1 = false;
            正在发行 obj正在开奖 = null;
            try
            {
                #region 正在开奖 - 创建记录，将状态设置为已发布

                // 得到正在开奖的期数信息
                obj正在开奖 = SSCUtils.GetCur(lotteryId, GetControl());
                if (obj正在开奖 != null)
                {
                    string issuedId = new DictSetUtil().PushSLItem(obj正在开奖.Issue).DoSignature();
                    try
                    {
                        // 得到正在开奖的记录，如果数据库没有记录则创建此记录
                        ParamUtil aPU = new ParamUtil().SQLCmdLoadData()
                            .SQLEntityScript(CAIKA_SSC_ISSUED, string.Format("SELECT * FROM " + CAIKA_SSC_ISSUED + " WHERE OBJECTID='{0}'", issuedId))
                            .SQLEntityScript(CAIKA_SSCCATE, string.Format("SELECT * FROM " + CAIKA_SSCCATE + " WHERE OBJECTID='{0}'", issuedId))
                            .ExecuteCmd(ADataLoader.DataLoader());
                        if (aPU.IsOK())
                        {
                            DataSet dsPersistent = aPU.GetValueAsDataSet();
                            if (dsPersistent.Tables[CAIKA_SSC_ISSUED].Rows.Count == 0)
                            {
                                DataRow rowNew = dsPersistent.Tables[CAIKA_SSC_ISSUED].NewRow();
                                rowNew["OBJECTID"] = issuedId;
                                rowNew["ISSUEDNO"] = obj正在开奖.Issue;
                                rowNew["ISSUEDDATE"] = DateTime.Now.Date;
                                rowNew["OPENTIME"] = obj正在开奖.OpenTime;
                                rowNew["BONUSTIME"] = obj正在开奖.BonusTime;
                                rowNew["STATE"] = 发行状态.已发布;
                                rowNew["CTIME"] = DateTime.Now;
                                rowNew["ETIME"] = DateTime.Now;
                                dsPersistent.Tables[CAIKA_SSC_ISSUED].Rows.Add(rowNew);
                            }

                            if (dsPersistent.Tables[CAIKA_SSCCATE].Rows.Count == 0)
                            {
                                DataRow rowNew = dsPersistent.Tables[CAIKA_SSCCATE].NewRow();
                                rowNew["OBJECTID"] = issuedId;
                                rowNew["CATEGORYID"] = lotteryType + 数彩看板.已开放;
                                rowNew["ISSUEDNO"] = obj正在开奖.Issue;
                                rowNew["BONUSTIME"] = obj正在开奖.BonusTime;
                                rowNew["CTIME"] = DateTime.Now;
                                rowNew["ETIME"] = DateTime.Now;
                                dsPersistent.Tables[CAIKA_SSCCATE].Rows.Add(rowNew);
                            }

                            flag1 = true;
                            if (dsPersistent.HasChanges())
                            {
                                ParamUtil aPU2 = new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader());
                                if (!aPU2.IsOK())
                                {
                                    GetControl().WriteError(aPU2.GetError());
                                    flag1 = false;
                                }
                            }
                        }
                        else
                        {
                            GetControl().WriteError(aPU.GetError());
                            flag1 = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        GetControl().WriteError(ex.ToString());
                        flag1 = false;
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                GetControl().WriteError(ex.ToString());
                flag1 = false;
            }

            if (flag1)
            {
                bool flag2 = false;
                string issuedId = new DictSetUtil().PushSLItem(obj正在开奖.Issue).DoSignature();
                try
                {
                    #region 更新记录，将状态设置为已截止

                    // 如果未到开奖时间则等待
                    DateTime dtOpen = Convert.ToDateTime(obj正在开奖.OpenTime).AddSeconds(-20);
                    DateTime dtNow1 = DateTime.Now;
                    while (DateTime.Compare(dtOpen, dtNow1) > 0)
                    {
                        dtNow1 = DateTime.Now;
                        Thread.Sleep(1000);
                    }

                    // 得到正在开奖的记录，如果数据库没有记录则创建此记录
                    ParamUtil aPU1 = new ParamUtil().SQLCmdLoadData()
                        .SQLEntityScript(CAIKA_SSC_ISSUED, string.Format("SELECT * FROM " + CAIKA_SSC_ISSUED + " WHERE OBJECTID='{0}'", issuedId))
                        .SQLEntityScript(CAIKA_SSCCATE, string.Format("SELECT * FROM " + CAIKA_SSCCATE + " WHERE OBJECTID='{0}'", issuedId))
                        .ExecuteCmd(ADataLoader.DataLoader());
                    if (aPU1.IsOK())
                    {
                        DataSet dsPersistent = aPU1.GetValueAsDataSet();
                        dsPersistent.Tables[CAIKA_SSC_ISSUED].Rows[0]["STATE"] = 发行状态.已截止;
                        dsPersistent.Tables[CAIKA_SSC_ISSUED].Rows[0]["ETIME"] = DateTime.Now;

                        dsPersistent.Tables[CAIKA_SSCCATE].Rows[0]["CATEGORYID"] = lotteryType + 数彩看板.已截止;
                        dsPersistent.Tables[CAIKA_SSCCATE].Rows[0]["ETIME"] = DateTime.Now;

                        flag2 = true;
                        string errMsg = new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader()).GetError();
                        if (errMsg != ActionUtil.DefaultError)
                        {
                            GetControl().WriteError(errMsg);
                            flag2 = false;
                        }
                    }
                    else
                    {
                        GetControl().WriteError(aPU1.GetError());
                    }

                    #endregion

                    #region 更新记录，将状态设置为待开奖

                    if (flag2)
                    {
                        // 如果未到开奖时间则等待
                        DateTime dtBonusTime = Convert.ToDateTime(obj正在开奖.BonusTime);
                        DateTime dtNow2 = DateTime.Now;
                        while (DateTime.Compare(dtBonusTime, dtNow2) > 0)
                        {
                            dtNow2 = DateTime.Now;
                            Thread.Sleep(1000);
                        }

                        // 得到正在开奖的记录，如果数据库没有记录则创建此记录
                        ParamUtil aPU2 = new ParamUtil().SQLCmdLoadData()
                            .SQLEntityScript(CAIKA_SSC_ISSUED, string.Format("SELECT * FROM " + CAIKA_SSC_ISSUED + " WHERE OBJECTID='{0}'", issuedId))
                            .ExecuteCmd(ADataLoader.DataLoader());
                        if (aPU2.IsOK())
                        {
                            DataSet dsPersistent = aPU2.GetValueAsDataSet();
                            dsPersistent.Tables[CAIKA_SSC_ISSUED].Rows[0]["STATE"] = 发行状态.待开奖;
                            dsPersistent.Tables[CAIKA_SSC_ISSUED].Rows[0]["ETIME"] = DateTime.Now;

                            string errMsg = new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader()).GetError();
                            if (errMsg != ActionUtil.DefaultError)
                            {
                                GetControl().WriteError(errMsg);
                            }
                        }
                        else
                        {
                            GetControl().WriteError(aPU2.GetError());
                        }
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    GetControl().WriteError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 开奖
        /// </summary>
        public const string CBonus4SSC = "CBonus4SSC";
        private void Bonus4SSC(Hashtable Params)
        {
            // 得到时时彩的彩种编号（彩票360提供）
            string lotteryId = PickParam(Params).GetValueAsString();
            if (string.IsNullOrEmpty(lotteryId))
            {
                throw new ArgumentNullException(ActionUtil.RetResult, "未传入时时彩彩种编号！");
            }

            string lotteryType = PickParam(Params).GetValueAsString("LotteryType");
            if (string.IsNullOrEmpty(lotteryType))
            {
                throw new ArgumentNullException(ActionUtil.RetResult, "未传入时时彩彩种！");
            }

            string CAIKA_SSCCATE = "";
            string CAIKA_SSC_ISSUED = "";
            string CAIKA_SSC_SUMMARIES = "";
            string CAIKA_SSC_ISSUEDMISSED = "";
            switch (lotteryType)
            {
                case 数字彩分类.重庆时时彩:
                    CAIKA_SSCCATE = "CAIKA_SSCCATE";
                    CAIKA_SSC_ISSUED = "CAIKA_SSCCQ_ISSUED";
                    CAIKA_SSC_SUMMARIES = "CAIKA_SSCCQ_SUMMARIES";
                    CAIKA_SSC_ISSUEDMISSED = "CAIKA_SSCCQ_ISSUEDMISSED";
                    break;
                case 数字彩分类.江西时时彩:
                    CAIKA_SSCCATE = "CAIKA_SSCCATE";
                    CAIKA_SSC_ISSUED = "CAIKA_SSCJX_ISSUED";
                    CAIKA_SSC_SUMMARIES = "CAIKA_SSCJX_SUMMARIES";
                    CAIKA_SSC_ISSUEDMISSED = "CAIKA_SSCJX_ISSUEDMISSED";
                    break;
                case 数字彩分类.新疆时时彩:
                    break;
                case 数字彩分类.天津时时彩:
                    break;
                default:
                    throw new ArgumentException(string.Format("未知彩种!{0}", lotteryType), "LotteryType");
            }

            try
            {
                // 得到所有待开奖的记录
                ParamUtil aPU = new ParamUtil().SQLCmdLoadData()
                    .SQLEntityScript(CAIKA_SSC_ISSUED, string.Format("SELECT * FROM {0} WHERE STATE='{1}' ORDER BY OPENTIME DESC", CAIKA_SSC_ISSUED, 发行状态.待开奖))
                    .ExecuteCmd(ADataLoader.DataLoader());
                if (aPU.IsOK())
                {
                    DataSet dsHistory = new DictSetUtil(new DataSet()).MyDS;
                    foreach (DataRow rowItem in new DataView(aPU.GetValueAsDataSet().Tables[CAIKA_SSC_ISSUED]).ToTable(true, "ISSUEDDATE").Rows)
                    {
                        DataSet dsTmp = SSCUtils.GetHistory(lotteryId, rowItem.Field<DateTime>("ISSUEDDATE"), GetControl());
                        if (dsTmp != null)
                        {
                            dsHistory.Merge(dsTmp);
                        }
                    }

                    // 更新所有等待开奖的记录
                    foreach (DataRow rowWaiting in aPU.GetValueAsDataSet().Tables[CAIKA_SSC_ISSUED].Rows)
                    {
                        try
                        {
                            DataSet dsPersistent = new ParamUtil()
                                .SQLCmdLoadData()
                                .SQLEntityScript(CAIKA_SSC_ISSUED, string.Format("SELECT * FROM " + CAIKA_SSC_ISSUED + " WHERE OBJECTID = '{0}'", rowWaiting["OBJECTID"]))
                                .SQLEntityScript(CAIKA_SSCCATE, string.Format("SELECT * FROM " + CAIKA_SSCCATE + " WHERE OBJECTID = '{0}'", rowWaiting["OBJECTID"]))
                                .ExecuteCmd(ADataLoader.DataLoader()).GetValueAsDataSet();

                            #region 更新记录，将状态设置为已开奖

                            DataRow rowOpened = Functions.FindRow(DictSet.FN_MCCanShu + "='" + rowWaiting["ISSUEDNO"] + "'", dsHistory.Tables[DictSet.TableName]);
                            DataRow rowIssued = Functions.FindRow("OBJECTID='" + rowWaiting["OBJECTID"] + "'", dsPersistent.Tables[CAIKA_SSC_ISSUED]);
                            if (rowOpened != null && rowIssued != null)
                            {
                                string WinNumber = rowOpened.Field<string>(DictSet.FN_CanShuZhi);
                                if (string.IsNullOrEmpty(WinNumber))
                                    continue;

                                rowIssued["OPENED"] = WinNumber;
                                for (int i = 0; i < 5; i++)
                                {
                                    rowIssued["N" + i] = WinNumber[i];
                                }

                                rowIssued["PRE2SUM"] = int.Parse(WinNumber[0].ToString()) + int.Parse(WinNumber[1].ToString());
                                rowIssued["LST2SUM"] = int.Parse(WinNumber[3].ToString()) + int.Parse(WinNumber[4].ToString());
                                rowIssued["PRE3SUM"] = int.Parse(WinNumber[0].ToString()) + int.Parse(WinNumber[1].ToString()) + int.Parse(WinNumber[2].ToString());
                                rowIssued["MID3SUM"] = int.Parse(WinNumber[1].ToString()) + int.Parse(WinNumber[2].ToString()) + int.Parse(WinNumber[3].ToString());
                                rowIssued["LST3SUM"] = int.Parse(WinNumber[2].ToString()) + int.Parse(WinNumber[3].ToString()) + int.Parse(WinNumber[4].ToString());
                                rowIssued["PRE3STATE"] = Utils.前三形态(WinNumber);
                                rowIssued["MID3STATE"] = Utils.中三形态(WinNumber);
                                rowIssued["LST3STATE"] = Utils.后三形态(WinNumber);
                                rowIssued["PRE2STATE"] = Utils.前二形态(WinNumber);
                                rowIssued["LST2STATE"] = Utils.后二形态(WinNumber);

                                rowIssued["STATE"] = 发行状态.已开奖;
                                rowIssued["ETIME"] = DateTime.Now;

                                // 移到已开奖的分类
                                dsPersistent.Tables[CAIKA_SSCCATE].Rows[0]["CATEGORYID"] = lotteryType + 数彩看板.已开奖;
                                dsPersistent.Tables[CAIKA_SSCCATE].Rows[0]["ETIME"] = DateTime.Now;

                                ParamUtil aPU2 = new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader());
                                if (!aPU2.IsOK())
                                {
                                    GetControl().WriteError(aPU2.GetError());
                                }
                            }

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            GetControl().WriteError("更新开奖状态失败！描述：" + ex.ToString());
                        }
                    }

                    // 更新摘要、遗漏数据
                    foreach (DataRow rowItem in new DataView(aPU.GetValueAsDataSet().Tables[CAIKA_SSC_ISSUED]).ToTable(true, "ISSUEDDATE").Rows)
                    {
                        try
                        {
                            DataSet dsPersistent = new ParamUtil()
                                .SQLCmdLoadData()
                                .SQLEntityScript(CAIKA_SSC_ISSUED, string.Format("SELECT * FROM " + CAIKA_SSC_ISSUED + " WHERE ISSUEDDATE = '{0}'", rowItem["ISSUEDDATE"]))
                                .SQLEntityScript(CAIKA_SSC_SUMMARIES, string.Format("SELECT * FROM " + CAIKA_SSC_SUMMARIES + " WHERE ISSUEDDATE='{0}'", rowItem["ISSUEDDATE"]))
                                .SQLEntityScript(CAIKA_SSC_ISSUEDMISSED, "SELECT * FROM " + CAIKA_SSC_ISSUEDMISSED)
                                .ExecuteCmd(ADataLoader.DataLoader()).GetValueAsDataSet();

                            // 得到最新开奖的记录
                            DataView dvLst = new DataView(dsPersistent.Tables[CAIKA_SSC_ISSUED], "STATE='" + 发行状态.已开奖 + "'", "ISSUEDNO DESC", DataViewRowState.CurrentRows);
                            if (dvLst.Count > 0)
                            {
                                string WinNumber = dvLst[0]["OPENED"].ToString();
                                string LstIssuedNo = dvLst[0]["ISSUEDNO"].ToString();
                                int Pre2SUM = Convert.ToInt32(dvLst[0]["PRE2SUM"]);
                                int Lst2SUM = Convert.ToInt32(dvLst[0]["LST2SUM"]);
                                int Pre3SUM = Convert.ToInt32(dvLst[0]["PRE3SUM"]);
                                int Mid3SUM = Convert.ToInt32(dvLst[0]["MID3SUM"]);
                                int Lst3SUM = Convert.ToInt32(dvLst[0]["LST3SUM"]);

                                #region 开奖摘要

                                // 每日开奖摘要
                                DataRow rowSummaries = null;
                                if (dsPersistent.Tables[CAIKA_SSC_SUMMARIES].Rows.Count == 0)
                                {
                                    rowSummaries = dsPersistent.Tables[CAIKA_SSC_SUMMARIES].NewRow();
                                    rowSummaries["ISSUEDDATE"] = DateTime.Now.Date;
                                    dsPersistent.Tables[CAIKA_SSC_SUMMARIES].Rows.Add(rowSummaries);
                                }
                                else
                                {
                                    rowSummaries = dsPersistent.Tables[CAIKA_SSC_SUMMARIES].Rows[0];
                                }

                                rowSummaries["PRE_DUIZHI"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("PRE2STATE='" + 二星形态.对子 + "'").Length;
                                rowSummaries["PRE_LIANHAO"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("PRE2STATE='" + 二星形态.连号 + "'").Length;
                                rowSummaries["LST_DUIZHI"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("LST2STATE='" + 二星形态.对子 + "'").Length;
                                rowSummaries["LST_LIANHAO"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("LST2STATE='" + 二星形态.连号 + "'").Length;
                                rowSummaries["PRE_ZU3"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("PRE3STATE='" + 三星形态.组三 + "'").Length;
                                rowSummaries["PRE_ZU6"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("PRE3STATE='" + 三星形态.组六 + "'").Length;
                                rowSummaries["PRE_BAOZHI"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("PRE3STATE='" + 三星形态.豹子 + "'").Length;
                                rowSummaries["MID_ZU3"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("MID3STATE='" + 三星形态.组三 + "'").Length;
                                rowSummaries["MID_ZU6"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("MID3STATE='" + 三星形态.组六 + "'").Length;
                                rowSummaries["MID_BAOZHI"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("MID3STATE='" + 三星形态.豹子 + "'").Length;
                                rowSummaries["LST_ZU3"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("LST3STATE='" + 三星形态.组三 + "'").Length;
                                rowSummaries["LST_ZU6"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("LST3STATE='" + 三星形态.组六 + "'").Length;
                                rowSummaries["LST_BAOZHI"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("LST3STATE='" + 三星形态.豹子 + "'").Length;

                                rowSummaries["PRE_ZU3MAX"] = SSCUtils.GetMax(三星形态.组三, "PRE3STATE", dsPersistent.Tables[CAIKA_SSC_ISSUED], GetControl());
                                rowSummaries["PRE_ZU6MAX"] = SSCUtils.GetMax(三星形态.组六, "PRE3STATE", dsPersistent.Tables[CAIKA_SSC_ISSUED], GetControl());
                                rowSummaries["MID_ZU3MAX"] = SSCUtils.GetMax(三星形态.组三, "MID3STATE", dsPersistent.Tables[CAIKA_SSC_ISSUED], GetControl());
                                rowSummaries["MID_ZU6MAX"] = SSCUtils.GetMax(三星形态.组六, "MID3STATE", dsPersistent.Tables[CAIKA_SSC_ISSUED], GetControl());
                                rowSummaries["LST_ZU3MAX"] = SSCUtils.GetMax(三星形态.组三, "LST3STATE", dsPersistent.Tables[CAIKA_SSC_ISSUED], GetControl());
                                rowSummaries["LST_ZU6MAX"] = SSCUtils.GetMax(三星形态.组六, "LST3STATE", dsPersistent.Tables[CAIKA_SSC_ISSUED], GetControl());
                                rowSummaries["ETIME"] = DateTime.Now;

                                #endregion

                                #region 遗漏数据 - 定位胆、二星组选、三星选初始化

                                for (int num = 0; num < 10; num++)
                                {
                                    DataRow rowMissed = Functions.FindRow("NUM='" + num + "'", dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED]);
                                    if (rowMissed == null)
                                    {
                                        rowMissed = dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED].NewRow();
                                        rowMissed["NUM"] = num;
                                        for (int j = 0; j < 5; j++)
                                        {
                                            rowMissed["N" + j] = 0;
                                            rowMissed["N" + j + "MAX"] = 0;
                                        }

                                        rowMissed["PRE2ZU"] = 0;
                                        rowMissed["PRE2ZUMAX"] = 0;
                                        rowMissed["LST2ZU"] = 0;
                                        rowMissed["LST2ZUMAX"] = 0;
                                        rowMissed["PRE3ZU"] = 0;
                                        rowMissed["PRE3ZUMAX"] = 0;
                                        rowMissed["MID3ZU"] = 0;
                                        rowMissed["MID3ZUMAX"] = 0;
                                        rowMissed["LST3ZU"] = 0;
                                        rowMissed["LST3ZUMAX"] = 0;
                                        dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED].Rows.Add(rowMissed);
                                    }
                                }

                                #endregion

                                #region 遗漏数据 - 二码、三码合值初始化

                                // 前、后二合值遗漏
                                for (int num = 1; num <= 18; num++)
                                {
                                    DataRow rowMissed = Functions.FindRow("NUM='N2SUM" + num.ToString().PadLeft(2, '0') + "'", dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED]);
                                    if (rowMissed == null)
                                    {
                                        rowMissed = dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED].NewRow();
                                        rowMissed["NUM"] = "N2SUM" + num.ToString().PadLeft(2, '0');
                                        rowMissed["N0"] = 0;
                                        rowMissed["N0MAX"] = 0;
                                        rowMissed["N1"] = 0;
                                        rowMissed["N1MAX"] = 0;
                                        dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED].Rows.Add(rowMissed);
                                    }
                                }

                                // 前、中、后三合值遗漏
                                for (int num = 1; num <= 27; num++)
                                {
                                    DataRow rowMissed1 = Functions.FindRow("NUM='N3SUM" + num.ToString().PadLeft(2, '0') + "'", dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED]);
                                    if (rowMissed1 == null)
                                    {
                                        rowMissed1 = dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED].NewRow();
                                        rowMissed1["NUM"] = "N3SUM" + num.ToString().PadLeft(2, '0');
                                        rowMissed1["N0"] = 0;
                                        rowMissed1["N0MAX"] = 0;
                                        rowMissed1["N1"] = 0;
                                        rowMissed1["N1MAX"] = 0;
                                        rowMissed1["N2"] = 0;
                                        rowMissed1["N2MAX"] = 0;
                                        dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED].Rows.Add(rowMissed1);
                                    }
                                }

                                #endregion

                                string curLstIssued = Convert.ToString(dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED].Rows[0]["LSTISSUEDNO"]);
                                if (curLstIssued != LstIssuedNo)
                                {
                                    #region 定位胆遗漏

                                    for (int posi = 0; posi < 5; posi++)
                                    {
                                        for (int num = 0; num < 10; num++)
                                        {
                                            DataRow rowMissed = Functions.FindRow("NUM='" + num + "'", dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED]);
                                            if (num != int.Parse(WinNumber[posi].ToString()))
                                            {
                                                rowMissed["N" + posi] = rowMissed.Field<int>("N" + posi) + 1;
                                            }
                                            else
                                            {
                                                rowMissed["N" + posi] = 0;
                                            }

                                            if (rowMissed.Field<int>("N" + posi + "MAX") < rowMissed.Field<int>("N" + posi))
                                            {
                                                rowMissed["N" + posi + "MAX"] = rowMissed["N" + posi];
                                            }

                                            rowMissed["LSTISSUEDNO"] = LstIssuedNo;
                                            rowMissed["ETIME"] = DateTime.Now;
                                        }
                                    }

                                    #endregion

                                    #region 组选遗漏

                                    for (int num = 0; num < 10; num++)
                                    {
                                        DataRow rowMissed = Functions.FindRow("NUM='" + num + "'", dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED]);

                                        // 前二组选遗漏
                                        if (num != int.Parse(WinNumber[0].ToString()) && num != int.Parse(WinNumber[1].ToString()))
                                        {
                                            rowMissed["PRE2ZU"] = rowMissed.Field<int>("PRE2ZU") + 1;
                                            if (rowMissed.Field<int>("PRE2ZUMAX") < rowMissed.Field<int>("PRE2ZU"))
                                            {
                                                rowMissed["PRE2ZUMAX"] = rowMissed["PRE2ZU"];
                                            }
                                        }
                                        else
                                        {
                                            rowMissed["PRE2ZU"] = 0;
                                        }

                                        // 后二组选遗漏
                                        if (num != int.Parse(WinNumber[3].ToString()) && num != int.Parse(WinNumber[4].ToString()))
                                        {
                                            rowMissed["LST2ZU"] = rowMissed.Field<int>("LST2ZU") + 1;
                                            if (rowMissed.Field<int>("LST2ZUMAX") < rowMissed.Field<int>("LST2ZU"))
                                            {
                                                rowMissed["LST2ZUMAX"] = rowMissed["LST2ZU"];
                                            }
                                        }
                                        else
                                        {
                                            rowMissed["LST2ZU"] = 0;
                                        }

                                        // 前三组选遗漏
                                        if (num != int.Parse(WinNumber[0].ToString()) && num != int.Parse(WinNumber[1].ToString()) && num != int.Parse(WinNumber[2].ToString()))
                                        {
                                            rowMissed["PRE3ZU"] = rowMissed.Field<int>("PRE3ZU") + 1;
                                            if (rowMissed.Field<int>("PRE3ZUMAX") < rowMissed.Field<int>("PRE3ZU"))
                                            {
                                                rowMissed["PRE3ZUMAX"] = rowMissed["PRE3ZU"];
                                            }
                                        }
                                        else
                                        {
                                            rowMissed["PRE3ZU"] = 0;
                                        }

                                        // 中三组选遗漏
                                        if (num != int.Parse(WinNumber[1].ToString()) && num != int.Parse(WinNumber[2].ToString()) && num != int.Parse(WinNumber[3].ToString()))
                                        {
                                            rowMissed["MID3ZU"] = rowMissed.Field<int>("MID3ZU") + 1;
                                            if (rowMissed.Field<int>("MID3ZUMAX") < rowMissed.Field<int>("MID3ZU"))
                                            {
                                                rowMissed["MID3ZUMAX"] = rowMissed["MID3ZU"];
                                            }
                                        }
                                        else
                                        {
                                            rowMissed["MID3ZU"] = 0;
                                        }

                                        // 后三组选遗漏
                                        if (num != int.Parse(WinNumber[2].ToString()) && num != int.Parse(WinNumber[3].ToString()) && num != int.Parse(WinNumber[4].ToString()))
                                        {
                                            rowMissed["LST3ZU"] = rowMissed.Field<int>("LST3ZU") + 1;
                                            if (rowMissed.Field<int>("LST3ZUMAX") < rowMissed.Field<int>("LST3ZU"))
                                            {
                                                rowMissed["LST3ZUMAX"] = rowMissed["LST3ZU"];
                                            }
                                        }
                                        else
                                        {
                                            rowMissed["LST3ZU"] = 0;
                                        }
                                    }

                                    #endregion

                                    #region 二星合值遗漏

                                    for (int num = 1; num <= 18; num++)
                                    {
                                        DataRow rowMissed = Functions.FindRow("NUM='N2SUM" + num.ToString().PadLeft(2, '0') + "'", dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED]);
                                        if (num == Pre2SUM)
                                        {
                                            rowMissed["N0"] = 0;
                                        }
                                        else
                                        {
                                            rowMissed["N0"] = rowMissed.Field<int>("N0") + 1;
                                            if (rowMissed.Field<int>("N0MAX") < rowMissed.Field<int>("N0"))
                                            {
                                                rowMissed["N0MAX"] = rowMissed["N0"];
                                            }
                                        }

                                        if (num == Lst2SUM)
                                        {
                                            rowMissed["N1"] = 0;
                                        }
                                        else
                                        {
                                            rowMissed["N1"] = rowMissed.Field<int>("N1") + 1;
                                            if (rowMissed.Field<int>("N1MAX") < rowMissed.Field<int>("N1"))
                                            {
                                                rowMissed["N1MAX"] = rowMissed["N1"];
                                            }
                                        }

                                        rowMissed["LSTISSUEDNO"] = LstIssuedNo;
                                        rowMissed["ETIME"] = DateTime.Now;
                                    }

                                    #endregion

                                    #region 三星合值遗漏

                                    for (int num = 1; num <= 27; num++)
                                    {
                                        DataRow rowMissed = Functions.FindRow("NUM='N3SUM" + num.ToString().PadLeft(2, '0') + "'", dsPersistent.Tables[CAIKA_SSC_ISSUEDMISSED]);
                                        if (num == Pre3SUM)
                                        {
                                            rowMissed["N0"] = 0;
                                        }
                                        else
                                        {
                                            rowMissed["N0"] = rowMissed.Field<int>("N0") + 1;
                                            if (rowMissed.Field<int>("N0MAX") < rowMissed.Field<int>("N0"))
                                            {
                                                rowMissed["N0MAX"] = rowMissed["N0"];
                                            }
                                        }

                                        if (num == Mid3SUM)
                                        {
                                            rowMissed["N1"] = 0;
                                        }
                                        else
                                        {
                                            rowMissed["N1"] = rowMissed.Field<int>("N1") + 1;
                                            if (rowMissed.Field<int>("N1MAX") < rowMissed.Field<int>("N1"))
                                            {
                                                rowMissed["N1MAX"] = rowMissed["N1"];
                                            }
                                        }

                                        if (num == Lst3SUM)
                                        {
                                            rowMissed["N2"] = 0;
                                        }
                                        else
                                        {
                                            rowMissed["N2"] = rowMissed.Field<int>("N2") + 1;
                                            if (rowMissed.Field<int>("N2MAX") < rowMissed.Field<int>("N2"))
                                            {
                                                rowMissed["N2MAX"] = rowMissed["N2"];
                                            }
                                        }

                                        rowMissed["LSTISSUEDNO"] = LstIssuedNo;
                                        rowMissed["ETIME"] = DateTime.Now;
                                    }

                                    #endregion
                                }

                                ParamUtil aPU2 = new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader());
                                if (!aPU2.IsOK())
                                {
                                    GetControl().WriteError(aPU2.GetError());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GetControl().WriteError("更新摘要/遗漏数据失败！描述：" + ex.ToString());
                        }
                    }
                }
                else
                {
                    GetControl().WriteError(aPU.GetError());
                }
            }
            catch (Exception ex)
            {
                GetControl().WriteError(ex.ToString());
            }
        }

        /// <summary>
        /// 按天得到开奖数据
        /// </summary>
        public const string CBonus4SSCHistory = "CBonus4SSCHistory";
        private void Bonus4SSCHistory(Hashtable Params)
        {
            // 得到时时彩的彩种编号（彩票360提供）
            string lotteryId = PickParam(Params).GetValueAsString();
            if (string.IsNullOrEmpty(lotteryId))
            {
                throw new ArgumentNullException(ActionUtil.RetResult, "未传入时时彩彩种编号！");
            }

            string lotteryType = PickParam(Params).GetValueAsString("LotteryType");
            if (string.IsNullOrEmpty(lotteryType))
            {
                throw new ArgumentNullException(ActionUtil.RetResult, "未传入时时彩彩种！");
            }

            string CAIKA_SSCCATE = "";
            string CAIKA_SSC_ISSUED = "";
            string CAIKA_SSC_ISSUED_HISTORY = "";
            string CAIKA_SSC_SUMMARIES = "";
            switch (lotteryType)
            {
                case 数字彩分类.重庆时时彩:
                    CAIKA_SSCCATE = "CAIKA_SSCCATE";
                    CAIKA_SSC_ISSUED = "CAIKA_SSCCQ_ISSUED";
                    CAIKA_SSC_ISSUED_HISTORY = "CAIKA_SSCCQ_ISSUED_HISTORY";
                    CAIKA_SSC_SUMMARIES = "CAIKA_SSCCQ_SUMMARIES";
                    break;
                case 数字彩分类.江西时时彩:
                    CAIKA_SSCCATE = "CAIKA_SSCCATE";
                    CAIKA_SSC_ISSUED = "CAIKA_SSCJX_ISSUED";
                    CAIKA_SSC_ISSUED_HISTORY = "CAIKA_SSCJX_ISSUED_HISTORY";
                    CAIKA_SSC_SUMMARIES = "CAIKA_SSCJX_SUMMARIES";
                    break;
                case 数字彩分类.新疆时时彩:
                    break;
                case 数字彩分类.天津时时彩:
                    break;
                default:
                    throw new ArgumentException(string.Format("未知彩种!{0}", lotteryType), "LotteryType");
            }

            string dateStr = PickParam(Params).GetValueAsString("IssuedDate");
            if (string.IsNullOrEmpty(dateStr))
            {
                throw new ArgumentNullException(ActionUtil.RetResult, "未传入日期！");
            }

            DataSet dsHistory = SSCUtils.GetHistory(lotteryId, Convert.ToDateTime(dateStr), GetControl());
            if (dsHistory != null)
            {
                foreach (DataRow item in dsHistory.Tables[DictSet.TableName].Rows)
                {
                    string issuedId = new DictSetUtil().PushSLItem(item.Field<string>(DictSet.FN_MCCanShu)).DoSignature();
                    try
                    {
                        DataSet dsPersistent = new ParamUtil().SQLCmdLoadData()
                            .SQLEntityScript(CAIKA_SSC_ISSUED, string.Format("SELECT * FROM " + CAIKA_SSC_ISSUED + " WHERE ISSUEDDATE = '{0}'", Convert.ToDateTime(dateStr).Date))
                            .SQLEntityScript(CAIKA_SSCCATE, string.Format("SELECT * FROM " + CAIKA_SSCCATE + " WHERE OBJECTID = '{0}'", issuedId))
                            .SQLEntityScript(CAIKA_SSC_ISSUED_HISTORY, string.Format("SELECT * FROM " + CAIKA_SSC_ISSUED_HISTORY + " WHERE OBJECTID = '{0}'", issuedId))
                            .SQLEntityScript(CAIKA_SSC_SUMMARIES, string.Format("SELECT * FROM " + CAIKA_SSC_SUMMARIES + " WHERE ISSUEDDATE='{0}'", Convert.ToDateTime(dateStr).Date))
                            .ExecuteCmd(ADataLoader.DataLoader())
                            .GetValueAsDataSet();

                        string WinNumber = item.Field<string>(DictSet.FN_CanShuZhi);
                        if (string.IsNullOrEmpty(WinNumber))
                            return;

                        DataRow rowIssued = Functions.FindRow("OBJECTID='" + issuedId + "'", dsPersistent.Tables[CAIKA_SSC_ISSUED]);
                        if (rowIssued == null)
                        {
                            rowIssued = dsPersistent.Tables[CAIKA_SSC_ISSUED].NewRow();
                            rowIssued["OBJECTID"] = issuedId;
                            rowIssued["ISSUEDNO"] = item[DictSet.FN_MCCanShu];
                            rowIssued["ISSUEDDATE"] = Convert.ToDateTime(dateStr).Date;
                            //rowIssued["OPENTIME"] = obj正在开奖.OpenTime;
                            dsPersistent.Tables[CAIKA_SSC_ISSUED].Rows.Add(rowIssued);
                        }
                        else
                        {
                            if (rowIssued.Field<string>("STATE") == 发行状态.已开奖)
                            {
                                continue;
                            }
                        }

                        rowIssued["BONUSTIME"] = DateTime.Now;
                        rowIssued["OPENED"] = WinNumber;
                        for (int i = 0; i < 5; i++)
                        {
                            rowIssued["N" + i] = WinNumber[i];
                        }

                        rowIssued["PRE2SUM"] = int.Parse(WinNumber[0].ToString()) + int.Parse(WinNumber[1].ToString());
                        rowIssued["LST2SUM"] = int.Parse(WinNumber[3].ToString()) + int.Parse(WinNumber[4].ToString());
                        rowIssued["PRE3SUM"] = int.Parse(WinNumber[0].ToString()) + int.Parse(WinNumber[1].ToString()) + int.Parse(WinNumber[2].ToString());
                        rowIssued["MID3SUM"] = int.Parse(WinNumber[1].ToString()) + int.Parse(WinNumber[2].ToString()) + int.Parse(WinNumber[3].ToString());
                        rowIssued["LST3SUM"] = int.Parse(WinNumber[2].ToString()) + int.Parse(WinNumber[3].ToString()) + int.Parse(WinNumber[4].ToString());
                        rowIssued["PRE3STATE"] = Utils.前三形态(WinNumber);
                        rowIssued["MID3STATE"] = Utils.中三形态(WinNumber);
                        rowIssued["LST3STATE"] = Utils.后三形态(WinNumber);
                        rowIssued["PRE2STATE"] = Utils.前二形态(WinNumber);
                        rowIssued["LST2STATE"] = Utils.后二形态(WinNumber);
                        rowIssued["STATE"] = 发行状态.已开奖;
                        rowIssued["CTIME"] = DateTime.Now;
                        rowIssued["ETIME"] = DateTime.Now;

                        DataRow rowIssudCate = Functions.FindRow("OBJECTID='" + issuedId + "'", dsPersistent.Tables[CAIKA_SSCCATE]);
                        if (rowIssudCate == null)
                        {
                            rowIssudCate = dsPersistent.Tables[CAIKA_SSCCATE].NewRow();
                            rowIssudCate["OBJECTID"] = issuedId;
                            rowIssudCate["CATEGORYID"] = lotteryType + 数彩看板.已开奖;
                            rowIssudCate["ISSUEDNO"] = item[DictSet.FN_MCCanShu];
                            rowIssudCate["BONUSTIME"] = rowIssued["BONUSTIME"];
                            rowIssudCate["CTIME"] = DateTime.Now;
                            dsPersistent.Tables[CAIKA_SSCCATE].Rows.Add(rowIssudCate);
                        }

                        rowIssudCate["CATEGORYID"] = lotteryType + 数彩看板.已开奖;
                        rowIssudCate["ETIME"] = DateTime.Now;

                        // 历史记录
                        DataRow rowHistory = Functions.FindRow("OBJECTID='" + issuedId + "'", dsPersistent.Tables[CAIKA_SSC_ISSUED_HISTORY]);
                        if (rowHistory != null)
                        {
                            rowHistory.Delete();
                        }

                        dsPersistent.Tables[CAIKA_SSC_ISSUED_HISTORY].Rows.Add(rowIssued.ItemArray);

                        // 摘要
                        DataRow rowSummaries = Functions.FindRow("ISSUEDDATE='" + Convert.ToDateTime(dateStr).Date + "'", dsPersistent.Tables[CAIKA_SSC_SUMMARIES]);
                        if (rowSummaries == null)
                        {
                            rowSummaries = dsPersistent.Tables[CAIKA_SSC_SUMMARIES].NewRow();
                            rowSummaries["ISSUEDDATE"] = Convert.ToDateTime(dateStr).Date;
                            dsPersistent.Tables[CAIKA_SSC_SUMMARIES].Rows.Add(rowSummaries);
                        }

                        rowSummaries["PRE_DUIZHI"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("PRE2STATE='" + 二星形态.对子 + "'").Length;
                        rowSummaries["PRE_LIANHAO"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("PRE2STATE='" + 二星形态.连号 + "'").Length;
                        rowSummaries["LST_DUIZHI"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("LST2STATE='" + 二星形态.对子 + "'").Length;
                        rowSummaries["LST_LIANHAO"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("LST2STATE='" + 二星形态.连号 + "'").Length;
                        rowSummaries["PRE_ZU3"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("PRE3STATE='" + 三星形态.组三 + "'").Length;
                        rowSummaries["PRE_ZU6"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("PRE3STATE='" + 三星形态.组六 + "'").Length;
                        rowSummaries["PRE_BAOZHI"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("PRE3STATE='" + 三星形态.豹子 + "'").Length;
                        rowSummaries["MID_ZU3"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("MID3STATE='" + 三星形态.组三 + "'").Length;
                        rowSummaries["MID_ZU6"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("MID3STATE='" + 三星形态.组六 + "'").Length;
                        rowSummaries["MID_BAOZHI"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("MID3STATE='" + 三星形态.豹子 + "'").Length;
                        rowSummaries["LST_ZU3"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("LST3STATE='" + 三星形态.组三 + "'").Length;
                        rowSummaries["LST_ZU6"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("LST3STATE='" + 三星形态.组六 + "'").Length;
                        rowSummaries["LST_BAOZHI"] = dsPersistent.Tables[CAIKA_SSC_ISSUED].Select("LST3STATE='" + 三星形态.豹子 + "'").Length;

                        rowSummaries["PRE_ZU3MAX"] = SSCUtils.GetMax(三星形态.组三, "PRE3STATE", dsPersistent.Tables[CAIKA_SSC_ISSUED], GetControl());
                        rowSummaries["PRE_ZU6MAX"] = SSCUtils.GetMax(三星形态.组六, "PRE3STATE", dsPersistent.Tables[CAIKA_SSC_ISSUED], GetControl());
                        rowSummaries["MID_ZU3MAX"] = SSCUtils.GetMax(三星形态.组三, "MID3STATE", dsPersistent.Tables[CAIKA_SSC_ISSUED], GetControl());
                        rowSummaries["MID_ZU6MAX"] = SSCUtils.GetMax(三星形态.组六, "MID3STATE", dsPersistent.Tables[CAIKA_SSC_ISSUED], GetControl());
                        rowSummaries["LST_ZU3MAX"] = SSCUtils.GetMax(三星形态.组三, "LST3STATE", dsPersistent.Tables[CAIKA_SSC_ISSUED], GetControl());
                        rowSummaries["LST_ZU6MAX"] = SSCUtils.GetMax(三星形态.组六, "LST3STATE", dsPersistent.Tables[CAIKA_SSC_ISSUED], GetControl());
                        rowSummaries["ETIME"] = DateTime.Now;

                        ParamUtil aPU2 = new ParamUtil().SQLCmdPersistent().SetParam(dsPersistent).ExecuteCmd(ADataLoader.DataLoader());
                        if (!aPU2.IsOK())
                        {
                            GetControl().WriteError(aPU2.GetError());
                        }
                    }
                    catch (Exception ex)
                    {
                        GetControl().WriteError(ex.ToString());
                    }
                }
            }
        }

        #region Utils方法

        private class SSCUtils
        {
            public static 正在发行 GetCur(string lotteryId, IControl curControl)
            {
                正在发行 obj正在开奖 = null;
                int tryCount = 0;
                while (tryCount < 5)
                {
                    try
                    {
                        // 得到正在开奖的期数信息
                        HttpWebRequest aRequest = HttpWebRequest.Create(string.Format("http://chart.cp.360.cn/int/qcurissue?LotID={0}&r={1}", lotteryId, Functions.ToTimestamp(DateTime.Now))) as HttpWebRequest;
                        aRequest.Method = "GET";
                        aRequest.Accept = "text/html,application/xhtml+xml";
                        using (HttpWebResponse aResponse = aRequest.GetResponse() as HttpWebResponse)
                        {
                            using (StreamReader aReader = new StreamReader(aResponse.GetResponseStream(), Encoding.GetEncoding("gb2312")))
                            {
                                obj正在开奖 = DataToJsonString.Deserialize<正在发行>(aReader.ReadToEnd());
                            }

                            aRequest.Abort();
                        }
                    }
                    catch (Exception ex)
                    {
                        curControl.WriteError(ex.ToString());
                        obj正在开奖 = null;
                    }

                    if (obj正在开奖 != null)
                    {
                        tryCount = 5;
                    }
                    else
                    {
                        Thread.Sleep(1000 * tryCount);
                        tryCount++;
                    }
                }

                return obj正在开奖;
            }

            public static 最近开奖 GetLast(string lotteryId, IControl curControl)
            {
                最近开奖 obj最近开奖 = null;
                int tryCount = 0;
                while (tryCount < 5)
                {
                    try
                    {
                        HttpWebRequest aRequest = HttpWebRequest.Create(string.Format("http://chart.cp.360.cn/zst/qkj/?lotId={0}&r={1}", lotteryId, Functions.ToTimestamp(DateTime.Now))) as HttpWebRequest;
                        aRequest.Method = "GET";
                        aRequest.Accept = "text/html,application/xhtml+xml";
                        using (HttpWebResponse aResponse = aRequest.GetResponse() as HttpWebResponse)
                        {
                            using (StreamReader aReader = new StreamReader(aResponse.GetResponseStream(), Encoding.GetEncoding("gb2312")))
                            {
                                obj最近开奖 = DataToJsonString.Deserialize<最近开奖>(aReader.ReadToEnd().Replace("\"0\":", "\"curIssue\":"));
                            }
                        }

                        aRequest.Abort();
                    }
                    catch (Exception ex)
                    {
                        curControl.WriteError(ex.ToString());
                        obj最近开奖 = null;
                    }

                    if (obj最近开奖 != null)
                    {
                        tryCount = 5;
                    }
                    else
                    {
                        Thread.Sleep(1000 * tryCount);
                        tryCount++;
                    }
                }

                return obj最近开奖;
            }

            public static DataSet GetHistory(string lotteryId, DateTime date, IControl curControl)
            {
                DataSet dsResult = null;
                int tryCount = 0;
                while (tryCount < 5)
                {
                    try
                    {
                        // 得到指定日期的所有开奖记录
                        HttpWebRequest aRequest = HttpWebRequest.Create(string.Format("http://chart.cp.360.cn/kaijiang/kaijiang?lotId={0}&spanType=2&span={1}_{1}", lotteryId, date.ToString("yyyy-MM-dd"))) as HttpWebRequest;
                        aRequest.Accept = "text/html, application/xhtml+xml, */*";
                        aRequest.ContentType = "text/html";
                        using (HttpWebResponse aResponse = aRequest.GetResponse() as HttpWebResponse)
                        {
                            using (StreamReader aReader = new StreamReader(aResponse.GetResponseStream(), Encoding.GetEncoding("gb2312")))
                            {
                                // 先找到HTML内空中的关于“开奖清单”的内容块。
                                Match aMatch = Regex.Match(aReader.ReadToEnd(), @"(<div.*? id='his-tab'.*?>[\S\s]*?</div>)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (aMatch.Success)
                                {
                                    DictSetUtil aXTCS = new DictSetUtil(new DataSet());

                                    // 得到开奖的内容
                                    MatchCollection aCodes = Regex.Matches(aMatch.Value, @"(<tr><td class='gray'>[\s\S]*?</td><td.*?>[\s\S]*?</td>)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                    foreach (Match item in aCodes)
                                    {
                                        MatchCollection aTDs = Regex.Matches(item.Value, @"<td.*?>(.*?)</td>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                        if (aTDs.Count == 2)
                                        {
                                            try
                                            {
                                                string sn = string.Format("{0}{1}", date.ToString("yyMMdd"), aTDs[0].Groups[1].Value);
                                                string code = aTDs[1].Groups[1].Value.Replace("-", "").Trim();
                                                if (code.Length != 5)
                                                {
                                                    continue;
                                                }

                                                aXTCS.SetValue(sn, code);
                                            }
                                            catch (Exception ex1)
                                            {
                                                curControl.WriteError(string.Format("执行逻辑：{0}；调用方法：{1}", ex1.ToString()));
                                            }
                                        }
                                    }

                                    dsResult = aXTCS.MyDS;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        curControl.WriteError(ex.ToString());
                        dsResult = null;
                    }

                    if (dsResult != null)
                    {
                        tryCount = 5;
                    }
                    else
                    {
                        Thread.Sleep(1000 * tryCount);
                        tryCount++;
                    }
                }

                return dsResult;
            }

            public static int GetMax(string 形态, string field, DataTable dtIssued, IControl curControl)
            {
                int intCount = 0;
                try
                {
                    int tmp = 0;
                    DataRow[] rows = dtIssued.Select("(1=1)", "ISSUEDNO ASC", DataViewRowState.CurrentRows);
                    for (int i = 0; i < rows.Length; i++)
                    {
                        if (Convert.ToString(rows[i][field]) == 形态)
                        {
                            tmp++;
                        }
                        else
                        {
                            if (tmp > intCount)
                            {
                                intCount = tmp;
                            }

                            tmp = 0;
                        }
                    }

                    if (tmp > intCount)
                        intCount = tmp;
                }
                catch (Exception ex)
                {
                    curControl.WriteError(ex.ToString());
                    intCount = 0;
                }

                return intCount;
            }
        }

        #endregion

        #region JSON类

        public class 正在发行
        {
            /*
                {"Issue":"150721003","EndTime":"1437442225","FsEndTime":"1437442157","OpenTime":"2015-07-21 09:30","FsTimeSpace":"68","BonusTime":"2015-07-21 09:31","BonusWeek":"\u661f\u671f\u4e8c","IsOpen":"0"}
             */

            public string Issue
            {
                get;
                set;
            }

            public string EndTime
            {
                get;
                set;
            }

            public string FsEndTime
            {
                get;
                set;
            }

            public string OpenTime
            {
                get;
                set;
            }

            public string FsTimeSpace
            {
                get;
                set;
            }

            public string BonusTime
            {
                get;
                set;
            }

            public string BonusWeek
            {
                get;
                set;
            }

            public string IsOpen
            {
                get;
                set;
            }
        }

        public class 最近开奖
        {
            /*
                {
                "curIssue":{"Issue":"150721007","WinNumber":"85596","EndTime":"2015-07-21 10:10:44"},
                "preIssue":"150721007","preEndTime":"1437444661","now":"1437444956","openTime":"1437445260"}
             */

            public CurIssue curIssue
            {
                get;
                set;
            }

            public string preIssue
            {
                get;
                set;
            }

            public string preEndTime
            {
                get;
                set;
            }

            public string now
            {
                get;
                set;
            }

            public string openTime
            {
                get;
                set;
            }

            public class CurIssue
            {
                public string Issue
                {
                    get;
                    set;
                }

                public string WinNumber
                {
                    get;
                    set;
                }

                public string EndTime
                {
                    get;
                    set;
                }

            }
        }

        #endregion
    }
}