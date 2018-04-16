using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.CP.BASE.Payment
{
    public class txf_qr_response
    {
        //public int status;
        //public string msg;
        //public string sdorderno;
        //public string total_fee;
        //public string sdpayno;
        public string SignMethod;
        public string Signature;
        public string RspCod;
        public string RspMsg;
        public string MerNo;
        public string ProductId;
        public string TxSN;
        public string Amount;
        public string PdtName;
        public string Remark;
        public string ReturnUrl;
        public string NotifyUrl;
        public int Status;
        public string PlatTxSN;
        public string CodeUrl;
        public string ImgUrl;
    }
}
