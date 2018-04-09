using AosuApp;
using AosuApp.AosuFramework;
using System;
using System.Collections;
using System.Data;

namespace API.CP.BASE
{
    public static class CommonDBUtils
    {
        public static DataTable GetAreas(string parentId = "")
        {
            DataTable dtResult = null;
            try
            {
                string filter = "";
                if (string.IsNullOrEmpty(parentId))
                {
                    filter = "LEVEL = 1";
                }
                else
                {
                    filter = "PARENTID='" + parentId + "'";
                }

                ParamUtil aPU = new ParamUtil()
                    .SQLCmdLoadData()
                    .SQLWithOutSchema()
                    .SQLEntityScript("CAIKA_AREAS", string.Format("SELECT * FROM CAIKA_AREAS WHERE {0} ORDER BY ID", filter))
                    .ExecuteCmd(ADataLoader.DataLoader());
                if (aPU.IsOK())
                {
                    dtResult = aPU.GetValueAsDataSet().Tables[0];
                }
            }
            catch (Exception)
            {
            }

            return dtResult;
        }

        public static DataTable GetPaymentList()
        {
            try
            {
                ParamUtil paramUtil = new ParamUtil().SQLCmdLoadData().SQLWithOutSchema()
                    .SQLEntityScript("CAIKA_PAYMENT", string.Format("SELECT PAYID,NAME,PAYLINK,PAYMETHOD,CSSNAME,SEQNO FROM CAIKA_PAYMENT WHERE STATE='{0}'", state.Enabled))
                    .ExecuteCmd(ADataLoader.DataLoader());
                if (paramUtil.IsOK())
                {
                    return paramUtil.GetValueAsDataSet().Tables["CAIKA_PAYMENT"];
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public static Hashtable GetPaymentInfo(string payid)
        {
            Hashtable aHT = null;
            try
            {
                ParamUtil paramUtil = new ParamUtil().SQLCmdLoadData().SQLWithOutSchema()
                    .SQLEntityScript("CAIKA_PAYMENT", string.Format("SELECT TOP 1 * FROM CAIKA_PAYMENT WHERE STATE='{0}' AND PAYID='{1}'", state.Enabled, payid))
                    .ExecuteCmd(ADataLoader.DataLoader());
                if (paramUtil.IsOK())
                {
                    if(paramUtil.GetValueAsDataSet().Tables["CAIKA_PAYMENT"].Rows.Count == 1)
                    {
                        aHT = new UriUtil().ImportRow(paramUtil.GetValueAsDataSet().Tables["CAIKA_PAYMENT"].Rows[0])
                            .ExportHashtable();
                    }
                }
            }
            catch (Exception)
            {
            }

            return aHT;
        }
    }
}