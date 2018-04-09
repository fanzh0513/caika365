using AosuApp.AosuFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;
using System.Xml;

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


            byte[] data = Encoding.Default.GetBytes("p1_mchtid=xxx&p2_paytype=yyy&p3_paymoney=asfdasfd");
            HttpWebRequest aRequest = HttpWebRequest.Create("http://xxx.www.afasfda") as HttpWebRequest;
            aRequest.Method = "POST";
            aRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            aRequest.ContentLength = data.Length;
            using (Stream postStream = aRequest.GetRequestStream())
            {
                postStream.Write(data, 0, data.Length);
            }

            string tokenStr = "";
            using (HttpWebResponse postResponse = aRequest.GetResponse() as HttpWebResponse)
            {
                using (StreamReader aStream = new StreamReader(postResponse.GetResponseStream(), Encoding.UTF8))
                {
                    string responseStr = aStream.ReadToEnd();

                    tokenStr = responseStr.Replace("&amp;", "&");
                    using (XmlReader aReader = XmlReader.Create(new StringReader(responseStr)))
                    {
                        aReader.ReadToFollowing("attr");
                        tokenStr = "\"" + aReader.ReadElementContentAsString() + "\"";
                    }
                }
            }


            //PickParam(Params).GetValue<string>("a");
            //PickParam(Params).GetValueAsString("p");
            //string s_id  = PickParam(Params).GetValueAsString("v");
            //string u_id = PickParam(Params).GetValueAsString("u");

            //var m = Params["a"];

            //string orderId = new DictSetUtil(null).PushSLItem(s_id).PushSLItem(u_id).DoSignature();

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
