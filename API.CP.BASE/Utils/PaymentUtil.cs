using AosuApp.AosuFramework;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

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

        public static string EncryptMD5(string source)
        {
            byte[] bysource = Encoding.UTF8.GetBytes(source);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(bysource);
            StringBuilder strbuilder = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                strbuilder.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位
            }

            return strbuilder.ToString();
        }
    }
}
