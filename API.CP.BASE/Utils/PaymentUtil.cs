using AosuApp.AosuFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace API.CP.BASE
{
    public class PaymentUtil
    {
        public static Hashtable GetPaymethod2Hashtable(string channel, string payId)
        {
            Hashtable aHT = new Hashtable();
            try
            {
                ParamUtil paramUtil = new ParamUtil().SQLCmdLoadData()
                    .SQLEntityScript("CAIKA_PAYMENT", string.Format("SELECT PAYID,VENDORID,PAYLINK,PAYMETHOD,PAYPARAMS,RATE,TOTALREQUEST,TOTALRECEIVED FROM CAIKA_PAYMENT WHERE PAYID='{0}' AND PAYMETHOD='{1}' AND STATE='{2}'", payId, channel, AosuApp.state.Enabled))
                    .ExecuteCmd(ADataLoader.DataLoader());
                if (paramUtil.IsOK())
                {
                    ParamUtil.Pick(aHT)
                        .Merge(new UriUtil().ImportRow(paramUtil.GetValueAsDataSet().Tables[0].Rows[0]).ExportHashtable())
                        .ImportSets(AEntryDic.Pick().GetDic(ParamUtil.Pick(aHT).GetValueAsString("PAYPARAMS")));

                }
            }
            catch (Exception)
            {
            }

            return aHT;
        }
    }
}
