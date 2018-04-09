using AosuApp;
using AosuApp.AosuData;
using AosuApp.AosuFramework;
using AosuApp.Windows;
using AosuApp.Windows.Controls;
using AosuApp.Windows.Modeling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace CAIKA365.Enhance.Views
{
    public partial class FAgent : BaseForm, IModelingForm, IModelingList
    {
        public FAgent()
        {
            InitializeComponent();
        }

        public string[] ItemNames
        {
            get
            {
                return new string[]
                { 
                    "代理商列表.当前代理商编号",
                    "下级代理商.当前选中行",
                    "返利策略.当前选中行",
                    "账户流水.当前选中行",
                    "应用通行证.当前选中行",
                    "线上银行卡.当前选中行"
                };
            }
        }

        public object GetItem(string name)
        {
            try
            {
                switch (name)
                {
                    case "代理商列表.当前代理商编号":
                        if (this.uConditionGridControl1.CurrentRow != null)
                        {
                            return ((DataRowView)this.uConditionGridControl1.CurrentRow.DataBoundItem)["AGENTCODE"];
                        }
                        break;
                    case "下级代理商.当前选中行":
                    case "返利策略.当前选中行":
                    case "账户流水.当前选中行":
                    case "应用通行证.当前选中行":
                    case "线上银行卡.当前选中行":
                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public void DoQuery(string ControlName = "")
        {
        }

        public void DoExport(string ControlName = "")
        {
        }

        public void CommitChanges(bool forcedCommit = false, string ControlName = "")
        {
            throw new NotImplementedException();
        }

        public void CancelChanges(bool forced = false, string ControlName = "")
        {
            throw new NotImplementedException();
        }

        protected override void Execute(Hashtable Params)
        {
            switch (ActionUtil.GetCmd(Params))
            {
                case ActionUtil.AutoRun:
                    // 初始化界面
                    InitControls(PickParam(Params).GetValueAsString("代理商清单"), uConditionGridControl1);
                    InitControls(PickParam(Params).GetValueAsString("下级代理"), uConditionGridControl2);
                    InitControls(PickParam(Params).GetValueAsString("返利策略"), uConditionGridControl3);
                    InitControls(PickParam(Params).GetValueAsString("每月流水"), uConditionGridControl4);
                    InitControls(PickParam(Params).GetValueAsString("通行证"), uConditionGridControl5);
                    InitControls(PickParam(Params).GetValueAsString("线上银行卡"), uConditionGridControl6);
                    break;
            }

            base.Execute(Params);
        }

        protected override void LoadDefaultData(Hashtable Params)
        {
            uConditionGridControl1.DoQuery();
        }

        private void InitControls(string dictSeed, UConditionGridControl gridControl)
        {
            // 从程序的缓存中找到是否已经载入当前TableAttributes信息；如果没找到则从数据库中获取并更新到程序缓存中
            DataSet dsAttributes = PickParam(GetControl().GetContext().MyInfo).GetValueAsDataSet(dictSeed);
            if (dsAttributes == null)
            {
                dsAttributes = AEntryDic.Pick(GetControl()).GetDic(dictSeed);
                PickParam(GetControl().GetContext().MyInfo).SetParam(dictSeed, dsAttributes);
            }

            ListModelSettings ListViewModel = new ListModelSettings(dsAttributes);

            // 得到ICON资源
            AssemblyLoader assemblyLoader = GetControl().GetContext().GetAssemblyLoader(ListViewModel.IconResource);
            ResourceManager resourceManager = null;
            if (assemblyLoader != null)
            {
                resourceManager = assemblyLoader.GetInstance<ResourceManager>("Properties.Resources");
            }

            // 配置工具栏和列表控件、条件区域
            gridControl.Tag = ListViewModel;
            gridControl.InitVars(ListViewModel, resourceManager);
            gridControl.SetCurrentProfile(ListViewModel.ListProfile1);
            gridControl.Init();
        }

        private void SetPropertyGrid(UConditionGridControl gridControl)
        {
            if (!typeof(ListModelSettings).IsInstanceOfType(gridControl.Tag))
                return;

            propertyGrid1.SelectedObject = null;
            try
            {
                propertyGrid1.ShowTitle = true;
                propertyGrid1.Text = ((ListModelSettings)gridControl.Tag).PropertyTitle;

                // 设置编辑区
                propertyGrid1.SelectedObject = propertyGrid1.GetPropertyCollection((ListModelSettings)gridControl.Tag,
                    ((DataRowView)gridControl.CurrentRow.DataBoundItem).Row);
            }
            catch (Exception)
            {
            }
        }

        private string GetExtenalConditions(UConditionGridControl gridControl)
        {
            List<string> CascadingConditional = new List<string>();
            try
            {
                foreach (DataRow rowItem in ((ListModelSettings)gridControl.Tag).ExtenalConditions.Tables[DictSet.TableName].Rows.OfType<DataRow>().Where(cm => new ExtenalConditionItem(cm).FilterConditionFlag && new ExtenalConditionItem(cm).HostForm.Action.EndsWith(GetFullName())))
                {
                    ExtenalConditionItem condition = new ExtenalConditionItem(rowItem);
                    switch (condition.TagValue)
                    {
                        case "代理商列表.当前代理商编号":
                            object curAgentCode = this.GetItem(condition.TagValue);
                            if (curAgentCode != null)
                            {
                                CascadingConditional.Add(ListModelSettings.GetConditionalStr(condition, curAgentCode));
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                GetControl().WriteError(string.Format("执行方法：{0}；错误描述：{1}。", GetFullName(), ex.ToString()));
                CascadingConditional.Add("1=2");
            }

            return string.Join(" AND ", CascadingConditional);
        }

        private void uConditionGridControl1_QueryCompleted(object sender, EventArgs e)
        {
            SetPropertyGrid((UConditionGridControl)sender);
            try
            {
                uConditionGridControl2.DoQuery(GetExtenalConditions(uConditionGridControl2));
                //uConditionGridControl3.DoQuery(GetExtenalConditions(uConditionGridControl3));
                //uConditionGridControl4.DoQuery(GetExtenalConditions(uConditionGridControl4));
                //uConditionGridControl5.DoQuery(GetExtenalConditions(uConditionGridControl5));
                //uConditionGridControl6.DoQuery(GetExtenalConditions(uConditionGridControl6));
            }
            catch (Exception ex)
            {
                GetControl().WriteError(ex.ToString());
            }
        }

        private void ConditionGridControl_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            SetPropertyGrid((UConditionGridControl)sender);
        }
    }
}