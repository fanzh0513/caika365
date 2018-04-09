using AosuApp.AosuFramework;
using AosuApp.Windows;
using AosuApp.Windows.Controls;
using AosuApp.Windows.Modeling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace CAIKA365.Enhance
{
    public class CategoryAssistent : BaseAction
    {
        public const string CAppendNode4List = "CAppendNode4List";

        public const string CInsertChild4List = "CInsertChild4List";

        public const string CMarkDelete4List = "CMarkDelete4List";

        public const string CDisableSelected = "CDisableSelected";

        public const string CEnableSelected = "CEnableSelected";

        protected override void Execute(Hashtable Params)
        {
            switch (ActionUtil.GetCmd(Params))
            {
                case CAppendNode4List:
                    AppendNode4List(Params);
                    break;
                case CInsertChild4List:
                    InsertChild4List(Params);
                    break;
                case CMarkDelete4List:
                    MarkDelete4List(Params);
                    break;
                case CDisableSelected:
                    DisableSelected(Params);
                    break;
                case CEnableSelected:
                    EnableSelected(Params);
                    break;
            }
        }

        private void AppendNode4List(Hashtable Params)
        {
            IModelForm instance = PickParam(Params).GetValue<IModelForm>(ActionUtil.ActionID);
            if (instance != null)
            {
                string userId = GetControl().GetContext().GetValue<string>("USERID");

                StandardGrid gridList = instance.GetActived<ListModelFormProxy>().GridControl;
                if (gridList.CurrentRow != null)
                {
                    DataRowView curRow = (DataRowView)gridList.CurrentRow.DataBoundItem;

                    // 得到当前层级最大编号
                    DataView curLevel = new DataView(((DataSet)gridList.DataSource).Tables[gridList.DataMember], "PARENTID='" + curRow["PARENTID"] + "'", "CATEGORYID DESC", DataViewRowState.CurrentRows);
                    if (curLevel.Count == 0)
                        return;

                    int idx;
                    string cId = curLevel[0]["CATEGORYID"].ToString().Trim();
                    if (int.TryParse(cId.Substring(cId.Length - 3), out idx))
                    {
                        if (idx + 1 > 999)
                        {
                            AlertBox.ShowTips("已超过最大编码范围（0-999）！", instance, MessageBoxButtons.OK);
                            return;
                        }

                        // 新编码
                        string nId = string.Format("{0}{1:000}", cId.Substring(0, cId.Length - 3), idx + 1);
                        ParamUtil aPU = new ParamUtil()
                            .SQLCmdLoadData().SQLEntityScript("BASE_CATEGORY", string.Format("SELECT * FROM BASE_CATEGORY WHERE CATEGORYID='{0}'", nId))
                            .ExecuteCmd(ADataLoader.DataLoader());
                        if (aPU.IsOK())
                        {
                            bool isNew = false;

                            #region 构建分类数据

                            DataRow rowNew = ((DataSet)gridList.DataSource).Tables[gridList.DataMember].NewRow();
                            if (aPU.GetValueAsDataSet().Tables["BASE_CATEGORY"].Rows.Count == 0)
                            {
                                rowNew["CATEGORYID"] = nId;
                                rowNew["PARENTID"] = curRow["PARENTID"];
                                rowNew["CTYPE"] = curRow["CTYPE"];
                                rowNew["SEQNO"] = curRow.Row.Field<int>("SEQNO") + 1;
                                rowNew["LEVEL"] = curRow["LEVEL"];
                                rowNew["ISEND"] = "Y";
                                rowNew["STATE"] = AosuApp.state.Enabled;
                                rowNew["CTIME"] = DateTime.Now;
                                rowNew["CUSER"] = userId;
                                rowNew["ETIME"] = DateTime.Now;
                                rowNew["EUSER"] = userId;

                                isNew = true;
                            }
                            else
                            {
                                if (AlertBox.ShowWarning(string.Format("{0}编号已经存在，是否将数据放入当前窗体上下文中？", nId), instance, MessageBoxButtons.YesNo) != DialogResult.Yes)
                                {
                                    return;
                                }

                                rowNew.ItemArray = aPU.GetValueAsDataSet().Tables["BASE_CATEGORY"].Rows[0].ItemArray;
                                rowNew["STATE"] = AosuApp.state.Enabled;
                                rowNew["ETIME"] = DateTime.Now;
                                rowNew["EUSER"] = userId;
                            }

                            #endregion

                            #region 插入到列表

                            int newIdx = 0;
                            DataView dvChild = new DataView(((DataSet)gridList.DataSource).Tables[gridList.DataMember], "CATEGORYID LIKE '" + cId + "%'", "CATEGORYID DESC", DataViewRowState.CurrentRows);
                            if (dvChild.Count > 0)
                            {
                                DataGridViewRow gridRow = gridList.Rows.OfType<DataGridViewRow>().FirstOrDefault(cm => ((DataRowView)cm.DataBoundItem)["CATEGORYID"] == dvChild[0]["CATEGORYID"]);
                                if (gridRow != null)
                                {
                                    newIdx = gridRow.Index + 1;
                                }
                            }
                            else
                            {
                                DataGridViewRow gridRow = gridList.Rows.OfType<DataGridViewRow>().FirstOrDefault(cm => ((DataRowView)cm.DataBoundItem)["CATEGORYID"] == curLevel[0]["CATEGORYID"]);
                                if (gridRow != null)
                                {
                                    newIdx = gridRow.Index + 1;
                                }
                            }

                            ((DataSet)gridList.DataSource).Tables[gridList.DataMember].Rows.InsertAt(rowNew, newIdx);
                            if (!isNew)
                            {
                                // 行状态改为"Modified"
                                rowNew.AcceptChanges();
                                rowNew["EUSER"] = userId;
                                rowNew["ETIME"] = DateTime.Now;
                            }

                            gridList.CurrentCell = gridList[0, newIdx];

                            #endregion
                        }
                        else
                        {
                            AlertBox.ShowError(aPU.GetError(), instance, MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                        AlertBox.ShowTips(string.Format("无效的分类编号{0}！"), instance, MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void InsertChild4List(Hashtable Params)
        {
            IModelForm instance = PickParam(Params).GetValue<IModelForm>(ActionUtil.ActionID);
            if (instance != null)
            {
                StandardGrid gridList = instance.GetActived<ListModelFormProxy>().GridControl;
                if (gridList.CurrentRow != null)
                {
                    DataRowView curRow = (DataRowView)gridList.CurrentRow.DataBoundItem;
                    string userId = GetControl().GetContext().MyInfo["USERID"].ToString();
                    int idx;
                    string cId = "";

                    // 得到当前行的下一级子行的最大编号
                    DataView curChildren = new DataView(((DataSet)gridList.DataSource).Tables[gridList.DataMember], "PARENTID='" + curRow["CATEGORYID"] + "'", "CATEGORYID DESC", DataViewRowState.CurrentRows);
                    if (curChildren.Count == 0)
                    {
                        idx = 0;
                        cId = curRow["CATEGORYID"].ToString().Trim() + "000";
                    }
                    else
                    {
                        cId = curChildren[0]["CATEGORYID"].ToString().Trim();
                        if (!int.TryParse(cId.Substring(cId.Length - 3), out idx))
                        {
                            AlertBox.ShowTips(string.Format("无效的分类编号{0}！"), instance, MessageBoxButtons.OK);
                            return;
                        }
                    }

                    if (idx + 1 > 999)
                    {
                        AlertBox.ShowTips("已超过最大编码范围（0-999）！", instance, MessageBoxButtons.OK);
                        return;
                    }

                    // 新编码
                    string nId = string.Format("{0}{1:000}", cId.Substring(0, cId.Length - 3), idx + 1);
                    ParamUtil aPU = new ParamUtil()
                        .SQLCmdLoadData().SQLEntityScript("BASE_CATEGORY", string.Format("SELECT * FROM BASE_CATEGORY WHERE CATEGORYID='{0}'", nId))
                        .ExecuteCmd(ADataLoader.DataLoader());
                    if (aPU.IsOK())
                    {
                        bool isNew = false;

                        #region 构建分类数据

                        DataRow rowNew = ((DataSet)gridList.DataSource).Tables[gridList.DataMember].NewRow();
                        if (aPU.GetValueAsDataSet().Tables["BASE_CATEGORY"].Rows.Count == 0)
                        {
                            rowNew["CATEGORYID"] = nId;
                            rowNew["PARENTID"] = curRow["CATEGORYID"];
                            rowNew["CTYPE"] = curRow["CTYPE"];
                            rowNew["SEQNO"] = idx;
                            rowNew["LEVEL"] = curRow.Row.Field<int>("LEVEL") + 1;
                            rowNew["ISEND"] = "Y";
                            rowNew["STATE"] = AosuApp.state.Enabled;
                            rowNew["CTIME"] = DateTime.Now;
                            rowNew["CUSER"] = userId;
                            rowNew["ETIME"] = DateTime.Now;
                            rowNew["EUSER"] = userId;

                            isNew = true;
                        }
                        else
                        {
                            if (AlertBox.ShowWarning(string.Format("{0}编号已经存在，是否将数据放入当前窗体上下文中？", nId), instance, MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                return;
                            }

                            rowNew.ItemArray = aPU.GetValueAsDataSet().Tables["BASE_CATEGORY"].Rows[0].ItemArray;
                            rowNew["STATE"] = AosuApp.state.Enabled;
                            rowNew["ETIME"] = DateTime.Now;
                            rowNew["EUSER"] = userId;
                        }

                        #endregion

                        #region 插入到列表

                        int newIdx = 0;
                        DataView dvMaxChild = new DataView(((DataSet)gridList.DataSource).Tables[gridList.DataMember], "CATEGORYID LIKE '" + cId + "%'", "CATEGORYID DESC", DataViewRowState.CurrentRows);
                        if (dvMaxChild.Count > 0)
                        {
                            DataGridViewRow gridRow = gridList.Rows.OfType<DataGridViewRow>().FirstOrDefault(cm => ((DataRowView)cm.DataBoundItem)["CATEGORYID"] == dvMaxChild[0]["CATEGORYID"]);
                            if (gridRow != null)
                            {
                                newIdx = gridRow.Index + 1;
                            }
                        }
                        else
                        {
                            DataGridViewRow gridRow = gridList.Rows.OfType<DataGridViewRow>().FirstOrDefault(cm => ((DataRowView)cm.DataBoundItem)["CATEGORYID"] == curRow["CATEGORYID"]);
                            if (gridRow != null)
                            {
                                newIdx = gridRow.Index + 1;
                            }
                        }

                        ((DataSet)gridList.DataSource).Tables[gridList.DataMember].Rows.InsertAt(rowNew, newIdx);
                        if (!isNew)
                        {
                            // 行状态改为"Modified"
                            rowNew.AcceptChanges();
                            rowNew["ETIME"] = DateTime.Now;
                            rowNew["EUSER"] = userId;
                        }

                        gridList.CurrentCell = gridList[0, newIdx];

                        #endregion

                        curRow["ISEND"] = "N";
                        curRow["ETIME"] = DateTime.Now;
                        curRow["EUSER"] = userId;
                    }
                    else
                    {
                        AlertBox.ShowError(aPU.GetError(), instance, MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void MarkDelete4List(Hashtable Params)
        {
            IModelForm instance = PickParam(Params).GetValue<IModelForm>(ActionUtil.ActionID);
            if (instance != null)
            {
                StandardGrid gridList = instance.GetActived<ListModelFormProxy>().GridControl;
                if (gridList.SelectedRows.Count > 0)
                {
                    if (AlertBox.ShowWarning("是否确认删除当前及其所有子节点数据？", instance, MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        return;
                    }

                    List<DataGridViewRow> aList = new List<DataGridViewRow>();
                    foreach (DataGridViewRow item in gridList.SelectedRows)
                    {
                        List<DataGridViewRow> children = gridList.Rows.OfType<DataGridViewRow>().Where(cm =>
                        {
                            DataRowView aRow = cm.DataBoundItem as DataRowView;
                            if (aRow != null)
                            {
                                DataRowView curRow = item.DataBoundItem as DataRowView;
                                if (curRow != null)
                                {
                                    if (aRow.Row.Field<string>("CATEGORYID").Trim().StartsWith(curRow.Row.Field<string>("CATEGORYID").Trim()))
                                    {
                                        return true;
                                    }
                                }
                            }

                            return false;
                        }).ToList();

                        aList.AddRange(children.ToArray());
                    }

                    if (aList.Count > 0)
                    {
                        for (int i = aList.Count - 1; i >= 0; i--)
                        {
                            if (!aList[i].Displayed)
                                continue;

                            instance.GetActived<ListModelFormProxy>().MarkRemoved(aList[i]);
                        }
                    }
                }
            }
        }

        private void DisableSelected(Hashtable Params)
        {
            IModelForm instance = PickParam(Params).GetValue<IModelForm>(ActionUtil.ActionID);
            if (instance != null)
            {
                StandardGrid gridList = instance.GetActived<ListModelFormProxy>().GridControl;
                if (gridList.SelectedRows.Count > 0)
                {
                    if (AlertBox.ShowWarning("是否确认【停用】当前及其所有子节点数据？", instance, MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        return;
                    }

                    string userid = GetControl().GetContext().MyInfo["USERID"].ToString();
                    foreach (DataGridViewRow gridRow in gridList.SelectedRows)
                    {
                        DataRowView item = (DataRowView)gridRow.DataBoundItem;
                        item["STATE"] = AosuApp.state.Disabled;
                        item["ETIME"] = DateTime.Now;
                        item["EUSER"] = userid;

                        string pID = item["CATEGORYID"].ToString().Trim();

                        DataView dvChildren = new DataView(((DataSet)gridList.DataSource).Tables[gridList.DataMember], "CATEGORYID LIKE '" + pID + "%'", "CATEGORYID", DataViewRowState.CurrentRows);
                        if (dvChildren.Count > 0)
                        {
                            foreach (DataRowView child in dvChildren)
                            {
                                child["STATE"] = AosuApp.state.Disabled;
                                child["ETIME"] = DateTime.Now;
                                child["EUSER"] = userid;

                                DataGridViewRow gridChildRow = gridList.Rows.OfType<DataGridViewRow>().FirstOrDefault(cm => ((DataRowView)cm.DataBoundItem)["CATEGORYID"] == child["CATEGORYID"]);
                                if (gridChildRow != null)
                                {
                                    gridList.InvalidateRow(gridChildRow.Index);
                                }
                            }
                        }

                        gridList.InvalidateRow(gridRow.Index);
                    }
                }
            }
        }

        private void EnableSelected(Hashtable Params)
        {
            IModelForm instance = PickParam(Params).GetValue<IModelForm>(ActionUtil.ActionID);
            if (instance != null)
            {
                StandardGrid gridList = instance.GetActived<ListModelFormProxy>().GridControl;
                if (gridList.SelectedRows.Count > 0)
                {
                    if (AlertBox.ShowWarning("是否确认【启用】当前及其所有子节点数据？", instance, MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        return;
                    }

                    string userid = GetControl().GetContext().MyInfo["USERID"].ToString();
                    foreach (DataGridViewRow gridRow in gridList.SelectedRows)
                    {
                        DataRowView item = (DataRowView)gridRow.DataBoundItem;
                        item["STATE"] = AosuApp.state.Enabled;
                        item["ETIME"] = DateTime.Now;
                        item["EUSER"] = userid;

                        string pID = item["CATEGORYID"].ToString().Trim();

                        DataView dvChildren = new DataView(((DataSet)gridList.DataSource).Tables[gridList.DataMember], "CATEGORYID LIKE '" + pID + "%'", "CATEGORYID", DataViewRowState.CurrentRows);
                        if (dvChildren.Count > 0)
                        {
                            foreach (DataRowView child in dvChildren)
                            {
                                child["STATE"] = AosuApp.state.Enabled;
                                child["ETIME"] = DateTime.Now;
                                child["EUSER"] = userid;

                                DataGridViewRow gridChildRow = gridList.Rows.OfType<DataGridViewRow>().FirstOrDefault(cm => ((DataRowView)cm.DataBoundItem)["CATEGORYID"] == child["CATEGORYID"]);
                                if (gridChildRow != null)
                                {
                                    gridList.InvalidateRow(gridChildRow.Index);
                                }
                            }
                        }

                        gridList.InvalidateRow(gridRow.Index);
                    }
                }
            }
        }
    }
}