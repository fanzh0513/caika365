using AosuApp.AosuFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.CP.BASE.Payment.TXF
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

        }
    }
}
