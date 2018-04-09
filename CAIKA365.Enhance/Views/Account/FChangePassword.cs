using AosuApp.AosuFramework;
using AosuApp.Windows;
using AosuApp.Windows.Controls;
using System;
using System.Windows.Forms;

namespace CAIKA365.Enhance.Views
{
    public partial class FChangePassword : BaseForm
    {
        private ControlValidationHelper _objValidationHelper = null;

        public string Password
        {
            get
            {
                return TxtPassword.Text;
            }
        }

        public FChangePassword()
        {
            InitializeComponent();
        }

        protected override void Execute(System.Collections.Hashtable Params)
        {
            switch (ActionUtil.GetCmd(Params))
            {
                case ActionUtil.AutoRun:
                    _objValidationHelper = new ControlValidationHelper(errorProvider1);
                    _objValidationHelper.Add(TxtPassword);
                    _objValidationHelper.Add(TxtConfirm);

                     _objValidationHelper.ResetAllState(false, false);
                    _objValidationHelper.ValidateAll();
                   break;
            }

            base.Execute(Params);
        }

        private void Controls_RequestValidationEvent(object sender, EventArgs e)
        {
            IControlValidation objControl = (IControlValidation)sender;
            switch (((Control)sender).Name)
            {
                case "TxtPassword":
                case "TxtConfirm":
                    objControl.IsValid = true;
                    objControl.IsValidated = true;
                    if (objControl.IsEmpty)
                    {
                        objControl.IsValid = false;
                        objControl.ValidationErrorMsg = "密码不能为空！";
                    }
                    else
                    {
                        if (objControl.CurrentText.Length < 4 || objControl.CurrentText.Length > 18)
                        {
                            objControl.IsValid = false;
                            objControl.ValidationErrorMsg = "密码长度必须在(4-18)之间！";
                        }
                    }

                    if (objControl.IsValidated && objControl.IsValid)
                    {
                        if (TxtPassword.Text != TxtConfirm.Text)
                        {
                            objControl.IsValid = false;
                            objControl.ValidationErrorMsg = "两次密码输入不一致！";
                            if (objControl == TxtPassword && string.IsNullOrEmpty(TxtConfirm.Text))
                            {
                                objControl.IsValid = true;
                            }
                        }
                        else
                        {
                            TxtPassword.IsValid = true;
                            TxtConfirm.IsValid = true;
                        }
                    }
                    break;
            }

            if (!objControl.IsValid && objControl.IsValidated)
            {
                ((Control)objControl).Focus();
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (!_objValidationHelper.ValidateAll())
            {
                AlertBox.ShowWarning("窗体输入项存在校检错误，请核对！", this, MessageBoxButtons.OK);
                return;
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
