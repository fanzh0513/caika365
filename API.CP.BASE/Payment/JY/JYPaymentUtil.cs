using AosuApp.AosuFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Data;
using AosuApp.AosuData;

namespace API.CP.BASE.Payment
{
    public class JYPaymentUtil : BaseAction
    {
        public const string CGetQRImage = "CGetQRImage";

        public const string CQR4ALiPay = "CQR4ALiPay";

        public const string CQR4WeiX = "CQR4WeiX";

        public const string CQR4Tencent = "CQR4Tencent";

        protected override void Execute(Hashtable Params)
        {
            switch (ActionUtil.GetCmd(Params))
            {
                case CGetQRImage:
                    GetQRImage(Params);
                    break;
                case CQR4ALiPay:
                    QR4ALiPay(Params);
                    break;
                case CQR4WeiX:
                    QR4WeiX(Params);
                    break;
                case CQR4Tencent:
                    QR4Tencent(Params);
                    break;
            }
        }

        private void GetQRImage(Hashtable Params)
        {
            // HTTP请求参数
            // 商户ID	                        p1_mchtid	    是	int		        商户ID,由金阳支付分配
            // 支付方式	                        p2_paytype	    是	String(20)	    WEIXIN	支付网关(参见附录说明4.3)
            // 支付金额	                        p3_paymoney	    是	decimal	0.01	订单金额最小0.01(以元为单位）
            // 商户平台唯一订单号	            p4_orderno	    是	String(50)		商户系统内部订单号，要求50字符以内，同一商户号下订单号唯一
            // 商户异步回调通知地址	            p5_callbackurl	是	String(200)		商户异步回调通知地址
            // 商户同步通知地址	                p6_notifyurl	否	String(200)		商户同步通知地址
            // 版本号	                        p7_version	    是	String(4)	    V2.8	V2.8
            // 签名加密方式	                    p8_signtype	    是	int	1.MD5	    签名加密方式
            // 备注信息，上行中attach原样返回	p9_attach	    否	String(128)		备注信息，上行中attach原样返回
            // 分成标识	                        p10_appname	    否	Strng(25)		分成标识
            // 是否显示收银台	                p11_isshow	    是	int	0	        是否显示PC收银台
            // 商户的用户下单IP	                p12_orderip	    否	String(20)	    192.168.10.1	商户的用户下单IP
            // 签名	                            sign	        是	String(40)		MD5签名

            // HTTP响应数据（JSON）
            // rspCode	响应码	                int		响应码（参见附录说明4.2）	是
            // rspMsg	响应消息	            String 	200	Http请求响应消息	是
            // data		响应结果                JSON类			是

            // DATA的JSON格式：
            // r1_mchtid	    商户ID	            int		商户ID	是
            // r2_systemorderno	系统平台订单号	    String 	50	第三方平台订单号码	是
            // r3_orderno	    商户的平台订单号	String	50	商户系统内部订单号，要求50字符以内，同一商户号下订单号唯一	是
            // r4_amount	    支付金额	        decimal		订单金额(以元为单位）	是
            // r5_version	    版本号	            string	4	版本号（与请求参数一致）	是
            // r6_qrcode	    二维码信息	        String	200	二维码信息(QQ，支付宝，微信，银联，百度，京东)	是
            // r7_paytype	    支付方式	        String 	20	支付网关(参见附录说明4.3)	是
            // sign	            签名	            String 	40	MD5签名	是

            //string t = EncryptMD5("p1_mchtid=22225&p2_paytype=WEIXIN&p3_paymoney=0.99&p8_signtype=1&p11_isshow=1&p4_orderno=" + orderId + "&p5_callbackurl=http://localhost:15643/member/usercenter/submitDeposit");
            ////p1_mchtid=22222&p2_signtype=1&p3_orderno=20171014161133666&p4_version=v2.8e465b1d4cad5ca6201b05cc3ddf041aa


            //byte[] data = Encoding.UTF8.GetBytes("p1_mchtid=22225&p2_paytype=WEIXIN&p3_paymoney=0.99&p8_signtype=1&p11_isshow=1&p4_orderno=" +
            //    ""+ orderId+ "&p5_callbackurl=http://pay.095pay.com/api/order/pay&p7_version=v2.8&sign=" + t);


            DictSetUtil dictSet = new DictSetUtil(PickParam(Params).ExportDS());
            DictSetUtil dictParams = new DictSetUtil(new DSUtil(dictSet.MyDS).SetFilter(DictSet.TableName, string.Format("{0} LIKE 'params.%'", DictSet.FN_MCCanShu)).ExportDS());

            string s_id = PickParam(Params).GetValueAsString("v");
            string u_id = PickParam(Params).GetValueAsString("u");
            string a_id = PickParam(Params).GetValueAsString("a");
            string orderId = new DictSetUtil(null).PushSLItem(s_id).PushSLItem(u_id).DoSignature();

            List<string> aList = new List<string>();
            aList.Add("p1_mchtid=" + dictParams.GetValue("params.p1_mchtid"));     // 商户ID

            aList.Add("p2_paytype=" + dictParams.GetValue("params.p2_paytype"));    // 支付方式
            aList.Add("p3_paymoney="+ a_id);   // 支付金额
            aList.Add("p4_orderno=" + orderId);    // 商户平台唯一订单号
            aList.Add("p5_callbackurl="+dictParams.GetValue("params.p5_callbackurl"));   // 商户异步回调通知地址
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
                    jy_qr_response response_data = AosuApp.DataToJsonString.Deserialize<jy_qr_response>(aStream.ReadToEnd());
                    if(response_data != null)
                    {
                        switch (response_data.rspCode)
                        {
                            case 1:
                                // 创建支付订单，状态置为"未处理"，同时将二维码返回到前端页面。

                                PickParam(Params).SetParam(response_data.data.r6_qrcode);
                                break;
                            default:
                                PickParam(Params).SetError(response_data.rspMsg);
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

        private void QR4ALiPay(Hashtable Params)
        {
        }

        private void QR4WeiX(Hashtable Params)
        {

        }

        private void QR4Tencent(Hashtable Params)
        {

        }
    }
}
