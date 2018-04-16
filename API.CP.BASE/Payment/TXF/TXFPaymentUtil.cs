using AosuApp.AosuData;
using AosuApp.AosuFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
            /*------------天下付----------------
            //aList.Add("customerid=" + dictParams.GetValue("params.customerid"));   // 商户ID
            //aList.Add("paytype=" + dictParams.GetValue("params.paytype"));         // 支付方式
            //aList.Add("total_fee=" + a_id);         // 支付金额
            //aList.Add("sdorderno=" + orderId);      // 商户平台唯一订单号
            //aList.Add("notifyurl=" + dictParams.GetValue("params.notifyurl")); // 商户异步回调通知地址
            //aList.Add("returnurl=" + dictParams.GetValue("params.returnurl")); // 商户同步通知地址
            //aList.Add("version=" + dictParams.GetValue("params.version")); // 版本号
            //aList.Add("remark=");                                          //备注(可为空)
            //aList.Add("bankcode=");                                        // 网银直连不可为空，其他支付方式可为空
            //aList.Add("sign="+ PaymentUtil.EncryptMD5(string.Join("&", aList)+"&"+ dictParams.GetValue("paeam.paymentkey")));// 签名
            ------------------------------------*/
            aList.Add("Amount=" + a_id.Split('.')[0]);
            aList.Add("MerNo=" + dictParams.GetValue("params.customerid"));      // 商户号 
            aList.Add("NotifyUrl="+ dictParams.GetValue("params.notifyurl")); //异步通知URL
            aList.Add("PdtName=" + "测试账户"); //商品名称,不可空
            aList.Add("ProductId=" + dictParams.GetValue("params.ProductId")); // 产品类型(0601：微信扫码,0602 ：支付宝扫码,0603 ：银联扫码,0604 ：QQ扫码)
            aList.Add("Remark=test");                               //备注(可为空)
            aList.Add("ReturnUrl="+ dictParams.GetValue("params.returnurl")); // 页面通知地址       
            aList.Add("TxCode=" + dictParams.GetValue("params.TxCode"));      // 交易编码 ，默认值 ：210110
            aList.Add("TxSN=" + DateTime.Now.ToString("yyyyMMddHHmmss")); // AosuApp.Functions.ToTimestamp(DateTime.Now)商户交易流水号 唯一orderId   
            //string signString = string.Join("&", aList);
            //string sign = PaymentUtil.EncryptMD5(signString);
            //aList.Add("Signature=" + PaymentUtil.EncryptMD5(signString + sign));
            //aList.Add("SignMethod=" + "MD5");
            string sign = PaymentUtil.EncryptMD5(string.Join("&", aList)) + dictParams.GetValue("paeam.paymentkey");//
            aList.Add("Signature=" + sign);       // 签名信息 ，MD5 后32位小写
            aList.Add("SignMethod=" + "MD5");   // 签名方法 ，默认值 ：MD5

            byte[] data = Encoding.UTF8.GetBytes(System.Web.HttpUtility.UrlEncode(PaymentUtil.Encode(string.Join("&", aList)).Replace("+", "%2b"), System.Text.Encoding.UTF8));
            //http://pay.095pay.com/api/order/pay
            HttpWebRequest aRequest = HttpWebRequest.Create("http://api.1yigou.com.cn:8881/merchant-trade-api/command") as HttpWebRequest;
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
                        switch (response_data.Status)
                        {
                            case 1:
                                //PickParam(Params).SetParam(response_data);
                                //PickParam(Params).SetParam(imgstream);
                                //PickParam(Params).SetParam("content-type", "image/png");
                                using (HttpWebResponse response = HttpWebRequest.Create(response_data.ImgUrl).GetResponse() as HttpWebResponse)
                                {
                                    using (Image img = new Bitmap(response.GetResponseStream()))
                                    {
                                        MemoryStream ms = new MemoryStream();
                                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                                        PickParam(Params).SetParam(ms.ToArray());
                                        PickParam(Params).SetParam("content-type", "image/png");
                                    }
                                }
                                break;
                            default:
                                PickParam(Params).SetError(response_data.RspCod + response_data.RspMsg);
                                break;
                        }
                    }
                }
            }
        }
    }
}
