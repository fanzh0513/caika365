using AosuApp.AosuData;
using AosuApp.AosuFramework;
using AosuApp.AosuSSO;
using AosuApp.Windows;
using AosuApp.Windows.Controls;
using AosuApp.Windows.Modeling;
using API.CP.BASE;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CAIKA365.Enhance.Views
{
    public partial class FCreateAccount : BaseForm
    {
        private ControlValidationHelper _objValidationHelper = null;

        public string AccountID
        {
            get { return TxtAccountID.Text; }
        }

        public string SuperDomain
        {
            get;
            private set;
        }

        public string[] Domains
        {
            get
            {
                List<string> arrDomains = new List<string>();
                foreach (CheckBox item in groupBox1.Controls.OfType<CheckBox>().ToArray())
                {
                    if (item.Checked)
                    {
                        arrDomains.Add((string)item.Tag);
                    }
                }

                return arrDomains.ToArray();
            }
            private set
            {
                foreach (CheckBox item in groupBox1.Controls.OfType<CheckBox>().ToArray())
                {
                    item.Checked = false;
                }

                foreach (string item in value)
                {
                    CheckBox chk = groupBox1.Controls.OfType<CheckBox>().FirstOrDefault(cm => (string)cm.Tag == item);
                    if (chk != null)
                        chk.Checked = true;
                }
            }
        }

        public FCreateAccount()
        {
            InitializeComponent();
            _objValidationHelper = new ControlValidationHelper(this.errorProvider1);
        }

        protected override void Execute(Hashtable Params)
        {
            switch (ActionUtil.GetCmd(Params))
            {
                case ActionUtil.AutoRun:
                    CBTypes.Items.Add(account_type.NormalAcount);
                    CBTypes.Items.Add(account_type.VirtualAccount);
                    CBTypes.Items.Add(account_type.AgentAccount);
                    CBTypes.Items.Add(account_type.ManageAccount);
                    CBTypes.SelectedIndex = 0;

                    _objValidationHelper.Add(TxtAccountID);
                    _objValidationHelper.Add(TxtPassword);
                    _objValidationHelper.Add(TxtConfirm);
                    _objValidationHelper.Add(CBTypes);
                    _objValidationHelper.Add(TxtParentAgent);

                    standardGrid1.DataMember = DictSet.TableName;
                    standardGrid1.DataSource = MemberDBUtils.GetMemberDigest(GetControl());

                    _objValidationHelper.ResetAllState(false, false);
                    _objValidationHelper.ValidateAll();
                    break;
            }

            base.Execute(Params);
        }

        private void CBTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CBTypes.SelectedIndex == -1)
                return;

            string curType = (string)CBTypes.SelectedItem;
            switch (curType)
            {
                case account_type.NormalAcount:
                case account_type.VirtualAccount:
                    SuperDomain = "caika.com";
                    Domains = new string[] { "caika.com" };

                    ChkCom.Enabled = true;
                    ChkAgent.Enabled = false;
                    ChkSystem.Enabled = false;
                    break;
                case account_type.AgentAccount:
                    SuperDomain = "caika.agent";
                    Domains = new string[] { "caika.com", "caika.agent" };

                    ChkCom.Enabled = true;
                    ChkAgent.Enabled = true;
                    ChkSystem.Enabled = false;
                    break;
                case account_type.ManageAccount:
                    SuperDomain = "caika.system";
                    Domains = new string[] { "caika.com", "caika.system" };

                    ChkCom.Enabled = true;
                    ChkAgent.Enabled = false;
                    ChkSystem.Enabled = true;
                    break;
            }
        }

        private void standardGrid1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            standardGrid1.ReadOnly = e.ColumnIndex == 0;
        }

        private void standardGrid1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                switch (e.Value.ToString())
                {
                    case "TOTALAMOUNT":
                        e.Value = "账户总额";
                        break;
                    case "AVAILABLE":
                        e.Value = "可用余额";
                        break;
                    case "FREEZED":
                        e.Value = "冻结资金";
                        break;
                    case "SCORE":
                        e.Value = "积分";
                        break;
                    case "DEPOSITED":
                        e.Value = "充值金额";
                        break;
                    case "BONUS":
                        e.Value = "中奖金额";
                        break;
                    case "EXCHANGED":
                        e.Value = "积分兑换";
                        break;
                    case "RETURNED":
                        e.Value = "游戏返点";
                        break;
                    case "CONSUME":
                        e.Value = "投注金额";
                        break;
                    case "WITHDRAW":
                        e.Value = "提款金额";
                        break;
                }
            }
        }

        private void Controls_RequestValidationEvent(object sender, EventArgs e)
        {
            IControlValidation objControl = (IControlValidation)sender;
            objControl.IsValid = true;
            objControl.IsValidated = true;

            switch (((Control)sender).Name)
            {
                case "TxtAgentCode":
                    if (objControl.IsEmpty)
                    {
                        objControl.IsValid = false;
                        objControl.ValidationErrorMsg = "上级代理商编号不能为空！";
                    }
                    else
                    {
                        ParamUtil aPU = new ParamUtil().SQLCmdLoadData().SQLWithOutSchema()
                            .SQLEntityScript("BASE_CATEGORY", string.Format("SELECT CATEGORYID FROM BASE_CATEGORY WHERE CATEGORYID='{0}'", objControl.CurrentText))
                            .ExecuteCmd(ADataLoader.DataLoader());
                        if (!aPU.IsOK())
                        {
                            objControl.IsValid = false;
                            objControl.ValidationErrorMsg = aPU.GetError();
                        }
                        else
                        {
                            if (aPU.GetValueAsDataSet().Tables["BASE_CATEGORY"].Rows.Count == 0)
                            {
                                objControl.IsValid = false;
                                objControl.ValidationErrorMsg = string.Format("代理商编号[{0}]无效！", objControl.CurrentText);
                            }
                        }
                    }
                    break;
                case "TxtAccountID":
                    if (objControl.IsEmpty)
                    {
                        objControl.IsValid = false;
                        objControl.ValidationErrorMsg = "账户名不能为空！";
                    }
                    else
                    {
                        if (objControl.CurrentText.Length < 4 || objControl.CurrentText.Length > 18)
                        {
                            objControl.IsValid = false;
                            objControl.ValidationErrorMsg = "账户名长度必须在(4-18)之间！";
                        }
                        else
                        {
                            // 是否存在？
                            if (MemberDBUtils.MemberIsExist(GetControl(), TxtAccountID.Text))
                            {
                                objControl.IsValid = false;
                                objControl.ValidationErrorMsg = string.Format("账号[{0}]已经存在！", TxtAccountID.Text);
                            }
                        }
                    }
                    break;
                case "TxtPassword":
                case "TxtConfirm":
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

        private void TxtAgentCode_ShowListEvent(object sender, ShowListEventArgs e)
        {
            FList aList = new FList();
            aList.PickData().SetParam(ActionUtil.CRedirect,PickXTCS().SetValue("CATEGORYID", TxtParentAgent.Text).MyDS);
            aList.PickData().SetParam("彩咖网.客服中心.账号管理.代理列表");
            if (aList.ShowViewDialog(this) == DialogResult.OK)
            {
                TxtParentAgent.Text = aList.SelectedRow["CATEGORYID"].ToString();
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (!_objValidationHelper.ValidateAll())
            {
                AlertBox.ShowWarning("窗体输入项存在校检错误，请核对！", this, MessageBoxButtons.OK);
                return;
            }

            try
            {
                Hashtable aHT = new Hashtable();
                PickParam(aHT).SetParam("DOMAINUSER", TxtAccountID.Text);
                PickParam(aHT).SetParam("DOMAINNAME", SuperDomain);
                PickParam(aHT).SetParam("PSTPWD", TxtPassword.Text);
                PickParam(aHT).SetParam("TYPE", (string)CBTypes.SelectedItem);
                PickParam(aHT).SetParam("PARENTAGENT", TxtParentAgent.Text);
                PickParam(aHT).SetParam(standardGrid1.DataSource);

                // 创建会员账户
                Hashtable aTmp = new Hashtable();
                MemberDBUtils.CreateMemberRecord(GetControl(), PickParam(aTmp).Merge(aHT).ParamTable);
                if (!PickParam(aTmp).IsOK())
                {
                    AlertBox.ShowWarning(PickParam(aTmp).GetError(), this.MyParent, MessageBoxButtons.OK);
                    return;
                }

                // 先注册通行证、然后再添加会员账户
                // 执行CRegister命令后会破坏aHT参数信息，所以在此创建一个临时aHT1用于避免原始参数集合不被破坏。
                Hashtable aHT1 = new Hashtable();
                if (PickParam(aHT1).Merge(aHT).SetCmd(APassport.CRegister).ExecuteCmd(new APassport()).IsOK())
                {
                    foreach (string domain in Domains.Where(cm => cm != SuperDomain))
                    {
                        Hashtable aHT2 = new Hashtable();

                        // 绑定关联的应用域
                        PickParam(aHT2).Merge(aHT).SetParam("DOMAINNAME", domain);
                        if (!PickParam(aHT2).SetCmd(APassport.CRegister).ExecuteCmd(new APassport()).IsOK())
                        {
                            GetControl().WriteError(string.Format("绑定关联的应用域{0}失败！", domain));
                        }
                    }
                }

                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (Exception ex)
            {
                AlertBox.ShowError(ex.ToString(), this.MyParent, MessageBoxButtons.OK);
            }
        }
    }
}