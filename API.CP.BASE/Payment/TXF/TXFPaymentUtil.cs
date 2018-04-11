using AosuApp.AosuFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace API.CP.BASE.Payment
{
    public class TXFPaymentUtil : BaseAction
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
            aList.Add("p1_mchtid=" + dictParams.GetValue("params.p1_mchtid"));     // 商户ID

            aList.Add("p2_paytype=" + dictParams.GetValue("params.p2_paytype"));    // 支付方式
            aList.Add("p3_paymoney=" + a_id);   // 支付金额
            aList.Add("p4_orderno=" + orderId);    // 商户平台唯一订单号
            aList.Add("p5_callbackurl=" + dictParams.GetValue("params.p5_callbackurl"));   // 商户异步回调通知地址
            aList.Add("p6_notifyurl=" + dictParams.GetValue("params.p6_notifyurl"));   // 商户同步通知地址
            aList.Add("p7_version=" + dictParams.GetValue("params.p7_version"));    // 版本号
            aList.Add("p8_signtype=" + dictParams.GetValue("params.p8_signtype"));   // 签名加密方式
            aList.Add("p9_attach=");     // 备注信息，上行中attach原样返回
            aList.Add("p10_appname=");   // 分成标识
            aList.Add("p11_isshow=");    // 是否显示收银台
            aList.Add("p12_orderip=");   // 商户的用户下单IP
            aList.Add("sign=");          // 签名

            byte[] data = Encoding.UTF8.GetBytes(string.Join("&", aList));

            HttpWebRequest aRequest = HttpWebRequest.Create("http://pay.095pay.com/api/order/pay") as HttpWebRequest;
            aRequest.Method = "POST";
            aRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            aRequest.ContentLength = data.Length;
            using (Stream postStream = aRequest.GetRequestStream())
            {
                postStream.Write(data, 0, data.Length);
            }

            using (HttpWebResponse postResponse = aRequest.GetResponse() as HttpWebResponse)
            {
                using (StreamReader aStream = new StreamReader(postResponse.GetResponseStream(), Encoding.UTF8))
                {
                    txf_qr_response response_data = AosuApp.DataToJsonString.Deserialize<txf_qr_response>(aStream.ReadToEnd());
                    if (response_data != null)
                    {
                        switch (response_data.status)
                        {
                            case 1:
                                // 创建支付订单，状态置为"未处理"，同时将二维码返回到前端页面。

                                PickParam(Params).SetParam(response_data);
                                break;
                            default:
                                PickParam(Params).SetError(response_data.msg);
                                break;
                        }
                    }
                }
            }
        }
    }
}
