using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.CP.BASE.Payment
{
    /*
         {
            "data":
                {
                    "r1_mchtid":22222,
                    "r2_systemorderno":"gt125423057",
                    "r3_orderno":"20171014110226",
                    "r4_amount":1.1000,
                    "r5_version":1,
                    "r6_qrcode":"http://1241234124124213",
                    "r7_paytype":"v2.8",
                    "sign":"cf09bdca2b15620a69c82dad1c0313f4"
                },
            "rspCode":1,
            "rspMsg":"下单成功"
          }
        */

    public class jy_qr_response
    {

        public jy_qr_response_data data;

        public int rspCode;

        public string rspMsg;


        public class jy_qr_response_data
        {
            public string r1_mchtid;

            public string r2_systemorderno;
            
            public string r3_orderno;

            public double r4_amount;

            public string r5_version;

            public string r6_qrcode;

            public string r7_paytype;

            public string sign;
        }
    }
}
