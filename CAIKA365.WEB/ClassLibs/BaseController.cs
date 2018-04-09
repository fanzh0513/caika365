using AosuApp.AosuFramework;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.Mvc;

namespace CAIKA365.WEB.ClassLibs
{
    public class BaseController : Controller, IAction
    {
        private string _strMyID = "";
        private IControl _objMyControl = null;

        public BaseController()
        {
            AppDomain = GetControl().GetContext()["CAppDomain"];
            ResVersion = GetControl().GetContext()["CResVersion"];
        }

        protected bool CloseLog
        {
            get;
            set;
        }

        public string AppDomain
        {
            get;
            set;
        }

        public string ResVersion
        {
            get;
            set;
        }

        protected ParamUtil PickParam(Hashtable Params = null)
        {
            if (Params == null)
            {
                return ParamUtil.Pick(new Hashtable());
            }

            ParamUtil paramUtil = ParamUtil.Pick(Params);
            paramUtil.SetAction(this);
            return paramUtil;
        }

        protected ParamUtil PickParam(NameValueCollection QueryString)
        {
            Hashtable aHT = new Hashtable();
            foreach (string aKey in QueryString.AllKeys)
            {
                PickParam(aHT).SetParam(aKey, QueryString[aKey]);
            }

            return PickParam(aHT);
        }

        public void Perform(IControl aControl, Hashtable Params)
        {
            PickParam(Params).SetError(ActionUtil.DefaultError);
            if (aControl != null)
            {
                SetControl(aControl);
                BaseControl.GlobalControl = aControl;
                if (PickParam(Params).GetValueAsString(ActionUtil.ActionID) != ActionUtil.ActionID && (String.IsNullOrEmpty(GetActionID())))
                {
                    this.SetActionID(PickParam(Params).GetValueAsString(ActionUtil.ActionID));
                }

                if (!this.CloseLog)
                {
                    if (String.IsNullOrEmpty(GetActionID()))
                    {
                        aControl.WriteTrace(GetType().ToString() + ":" + PickParam(Params).GetCmd());
                    }
                    else
                    {
                        aControl.WriteTrace(GetActionID() + ":" + PickParam(Params).GetCmd());
                    }
                }
            }

            if (!this.PickParam(Params).IsOK())
                return;

            this.Execute(Params);
        }

        protected virtual void Execute(Hashtable Params)
        {
            switch (ActionUtil.GetCmd(Params))
            {
                default:
                    PickParam(Params).SetError(string.Format("未找到处理该命令的执行体!{0}", ActionUtil.GetCmd(Params)));
                    break;
            }
        }

        public Hashtable Execute(IControl Control, Hashtable Params)
        {
            this.Perform(Control, Params);
            return Params;
        }

        public string GetActionID()
        {
            return this._strMyID;
        }

        public void SetActionID(string aActionID)
        {
            this._strMyID = aActionID;
        }

        public IControl GetControl()
        {
            if (this._objMyControl == null)
                return BaseControl.GlobalControl;

            return this._objMyControl;
        }

        public void SetControl(IControl aControler)
        {
            this._objMyControl = aControler;
        }

        public string GetFullName(string aStr = "")
        {
            string str = this.GetType().FullName;
            if (!String.IsNullOrEmpty(aStr))
                str = str + "." + aStr;

            return str;
        }
    }
}