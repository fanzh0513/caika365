using AosuApp.AosuData;
using AosuApp.AosuFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace API.CP.BASE.Payment
{
    public class SYPaymentUtil : BaseAction
    {
        public const string CGetQRImage = "CGetQRImage";

        protected override void Execute(Hashtable Params)
        {
            switch (ActionUtil.GetCmd(Params))
            {
                case CGetQRImage:
                    GetQRImage(Params);
                    break;
            }
        }


        private void GetQRImage(Hashtable Params)
        {
            DictSetUtil dictSet = new DictSetUtil(PickParam(Params).ExportDS());
            DictSetUtil dictParams = new DictSetUtil(new DSUtil(dictSet.MyDS).SetFilter(DictSet.TableName, string.Format("{0} LIKE 'params.%'", DictSet.FN_MCCanShu)).ExportDS());

            string s_id = PickParam(Params).GetValueAsString("v");
            string u_id = PickParam(Params).GetValueAsString("u");
            string a_id = PickParam(Params).GetValueAsString("a");
            string orderId = new DictSetUtil(null).PushSLItem(s_id).PushSLItem(u_id).DoSignature();

            List<string> aList = new List<string>();
            aList.Add("parter=" + dictParams.GetValue("params.customerid"));      // 商户id，由分配
            aList.Add("type=" + dictParams.GetValue("params.paytype")); //银行类型，具体请参考附录1
            aList.Add("value=" + a_id); //单位元（人民币），2位小数，最小支付金额为0.02
            aList.Add("orderid=" + "12345678910");  // 商户系统订单号，该订单号将作为接口的返回数据。该值需在商户系统内唯一，系统暂时不检查该值是否唯一
            aList.Add("callbackurl=" + dictParams.GetValue("params.callbackurl")); //下行异步通知过程的返回地址，需要以http://开头且没有任何参数
            string sign = PaymentUtil.EncryptMD5(string.Join("&", aList) + dictParams.GetValue("params.paymentkey"));
            aList.Add("refbackurl=" + dictParams.GetValue("params.refbackurl")); // 页面通知地址   
            aList.Add("payerIp=127.0.0.1"); //用户在下单时的真实IP，接口将会判断玩家支付时的ip和该值是否相同。若不相同，接口将提示用户支付风险(可为空)
            aList.Add("attach=test");      //备注信息，下行中会原样返回。若该值包含中文，请注意编码            
            aList.Add("sign=" + sign);       // 签名信息 ，MD5 后32位小写
           
            string postDataStr = string.Join("&", aList);
            HttpWebRequest aRequest = (HttpWebRequest)WebRequest.Create("http://pay.shengyuanpay.com/chargebank.aspx" + (postDataStr == "" ? "" : "?") + postDataStr);
            aRequest.Method = "GET";
            aRequest.ContentType = "text/html; charset=UTF-8";
            
            using (HttpWebResponse postResponse = aRequest.GetResponse() as HttpWebResponse)
            {
                using (StreamReader aStream = new StreamReader(postResponse.GetResponseStream(), Encoding.UTF8))
                {
                    string retString = aStream.ReadToEnd();
                   
                }
            }
        }
    }
}
