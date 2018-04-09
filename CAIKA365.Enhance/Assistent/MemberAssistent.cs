using AosuApp.AosuData;
using AosuApp.AosuFramework;
using AosuApp.AosuSSO;
using AosuApp.Windows;
using AosuApp.Windows.Controls;
using AosuApp.Windows.Modeling;
using API.CP.BASE;
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CAIKA365.Enhance.Views;

namespace CAIKA365.Enhance
{
    public class MemberAssistent : BaseAction
    {
        public const string CChangePassword = "CChangePassword";

        public const string CCreateNewAccount = "CCreateNewAccount";

        public const string CDIGEST = "DIGEST";

        protected override void Execute(Hashtable Params)
        {
            switch (ActionUtil.GetCmd(Params))
            {
                case CCreateNewAccount:
                    CreateNewAccount(Params);
                    break;
                case CChangePassword:
                    ChangePassword(Params);
                    break;
                case CDIGEST:
                    DIGEST(Params);
                    break;
            }
        }

        private void CreateNewAccount(Hashtable Params)
        {
            IModelForm instance = PickParam(Params).GetValue<IModelForm>(ActionUtil.ActionID);
            if (instance != null)
            {
                FCreateAccount fInstance = new FCreateAccount();
                if (fInstance.ShowViewDialog(instance) == DialogResult.OK)
                {
                    DataSet dsPassport = MemberDBUtils.GetMemberAndPassport(GetControl(), fInstance.AccountID, fInstance.SuperDomain);
                    if (dsPassport != null)
                    {
                        StandardGrid gridList = instance.GetActived<ListModelFormProxy>().GridControl;

                        // 为列表数据源构建一条新记录，并定位到新记录上。
                        DataRow rowNew = ((DataSet)gridList.DataSource).Tables[gridList.DataMember].NewRow();
                        foreach (DataRow rowField in instance.GetItem<StandardModelSettings>(PickParam(Params).GetValueAsString("ListModelView")).Fields.Tables[DictSet.TableName].Rows)
                        {
                            FieldItem fieldItem = new FieldItem(rowField);
                            try
                            {
                                rowNew[fieldItem.FieldName] = dsPassport.Tables[fieldItem.PersistentTable].Rows[0][fieldItem.PersistentField];
                            }
                            catch (Exception)
                            {
                            }
                        }

                        ((DataSet)gridList.DataSource).Tables[gridList.DataMember].Rows.Add(rowNew);
                        rowNew.AcceptChanges();

                        // 插入新行并列表重新定位。
                        DataGridViewRow objCurrentRow = gridList.Rows.OfType<DataGridViewRow>().FirstOrDefault(cm => ((DataRowView)cm.DataBoundItem).Row == rowNew);
                        if (objCurrentRow != null)
                        {
                            gridList.CurrentCell = gridList[0, objCurrentRow.Index];
                            gridList.InvalidateRow(objCurrentRow.Index);
                        }
                    }

                    AlertBox.ShowTips("创建成功！", instance, MessageBoxButtons.OK);
                }
            }
        }

        private void ChangePassword(Hashtable Params)
        {
            IModelForm instance = PickParam(Params).GetValue<IModelForm>(ActionUtil.ActionID);
            if (instance != null)
            {
                StandardGrid gridList = instance.GetActived<ListModelFormProxy>().GridControl;
                if (gridList.CurrentRow == null)
                {
                    AlertBox.ShowWarning("当前列表窗体没有选中行。", instance, MessageBoxButtons.OK);
                    return;
                }

                DataRowView rowCurrent = (DataRowView)gridList.CurrentRow.DataBoundItem;

                FChangePassword fChangePassword = new FChangePassword();
                if (fChangePassword.ShowViewDialog(instance) == DialogResult.OK)
                {
                    Hashtable aHT = new Hashtable();
                    PickParam(aHT).SetCmd(APassport.CChangePassword);
                    PickParam(aHT).SetParam("FOCUSEDUPDATE", Boolean.TrueString);
                    PickParam(aHT).SetParam("DOMAINUSER", rowCurrent["DOMAINUSER"]);
                    PickParam(aHT).SetParam("DOMAINNAME", rowCurrent["DOMAINNAME"]);
                    PickParam(aHT).SetParam("NEWPWD", fChangePassword.Password);
                    if (PickParam(aHT).ExecuteCmd(new APassport()).IsOK())
                    {
                        AlertBox.ShowTips("设置成功！", instance, MessageBoxButtons.OK);
                        return;
                    }

                    AlertBox.ShowWarning("设置失败！", instance, MessageBoxButtons.OK);
                }
            }
        }

        private void DIGEST(Hashtable Params)
        {
            // 编辑器属性成员集合
            MyPropertyDescriptorCollection properties = PickParam(Params).GetValue() as MyPropertyDescriptorCollection;
            if (properties != null)
            {
                MyPropertyItemDescriptor curProperty = properties.FirstOrDefault(cm => cm.PropertyName == PickParam(Params).GetCmd());
                if (curProperty == null)
                {
                    throw new Exception(string.Format("在当前编辑器内未解析到【{0}】属性信息。", PickParam(Params).GetCmd()));
                }

                // 得到账户的资金摘要
                DictSetUtil dictDigest = new DictSetUtil(AEntryDic.Pick(GetControl()).GetDic(curProperty.Value as string));
                DataRow curRow = (DataRow)PickParam(Params).GetValue(ActionUtil.Current);

                curRow["TOTALAMOUNT"] = dictDigest.GetValue("TOTALAMOUNT");
                curRow["AVAILABLE"] = dictDigest.GetValue("AVAILABLE");
                curRow["FREEZED"] = dictDigest.GetValue("FREEZED");
                curRow["SCORE"] = dictDigest.GetValue("SCORE");

                // 总资金
                MyPropertyItemDescriptor total = properties.FirstOrDefault(cm => cm.PropertyName == "TOTALAMOUNT");
                if (total != null)
                {
                    total.Value = dictDigest.GetValue("TOTALAMOUNT");
                }

                // 可用资金
                MyPropertyItemDescriptor available = properties.FirstOrDefault(cm => cm.PropertyName == "AVAILABLE");
                if (available != null)
                {
                    available.Value = dictDigest.GetValue("AVAILABLE");
                }

                // 冻结资金
                MyPropertyItemDescriptor freezed = properties.FirstOrDefault(cm => cm.PropertyName == "FREEZED");
                if (freezed != null)
                {
                    freezed.Value = dictDigest.GetValue("FREEZED");
                }

                // 积分
                MyPropertyItemDescriptor score = properties.FirstOrDefault(cm => cm.PropertyName == "SCORE");
                if (score != null)
                {
                    score.Value = dictDigest.GetValue("SCORE");
                }
            }
        }
    }
}