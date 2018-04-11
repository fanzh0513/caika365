using AosuApp.AosuData;
using AosuApp.AosuFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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
            aList.Add("customerid=" + dictParams.GetValue("params.customerid"));   // 商户ID
            aList.Add("paytype=" + dictParams.GetValue("params.paytype"));         // 支付方式
            aList.Add("total_fee=" + a_id);         // 支付金额
            aList.Add("sdorderno=" + orderId);      // 商户平台唯一订单号
            aList.Add("notifyurl=" + dictParams.GetValue("params.notifyurl")); // 商户异步回调通知地址
            aList.Add("returnurl=" + dictParams.GetValue("params.returnurl")); // 商户同步通知地址
            aList.Add("version=" + dictParams.GetValue("params.version")); // 版本号
            aList.Add("remark=");                                          //备注(可为空)
            aList.Add("bankcode=");                                        // 网银直连不可为空，其他支付方式可为空
            aList.Add("sign="+ EncryptMD5(string.Join("&", aList)+"&"+ dictParams.GetValue("paeam.paymentkey")));// 签名

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
                                //PickParam(Params).SetParam(response_data);

                                //PickParam(Params).SetParam(imgstream);
                                PickParam(Params).SetParam("content-type", "image/png");
                                break;
                            default:
                                PickParam(Params).SetError(response_data.msg);
                                break;
                        }
                    }
                }
            }
        }

        private string EncryptMD5(string source)
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
