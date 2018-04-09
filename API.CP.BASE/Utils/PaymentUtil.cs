using AosuApp.AosuFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace API.CP.BASE
{
    public class PaymentUtil
    {
        public static UriUtil GetInterface(string channel, string payId)
        {
            try
            {
                ParamUtil paramUtil = new ParamUtil().SQLCmdLoadData()
                    .SQLEntityScript("CAIKA_PAYMENT", string.Format("SELECT PAYLINK FROM CAIKA_PAYMENT WHERE PAYID='{0}' AND PAYMETHOD='{1}' AND STATE='{2}'", payId, channel, AosuApp.state.Enabled)).ExecuteCmd(ADataLoader.DataLoader());
                if (paramUtil.IsOK())
                {
                    return new UriUtil(
                        paramUtil.GetValueAsDataSet().Tables[0].Rows[0].Field<string>(0));
                }
            }
            catch (Exception)
            {
            }

            return UriUtil.CreateIControlUriUtil("CInnerDataID", typeof(ANone));
        }
    }
}
