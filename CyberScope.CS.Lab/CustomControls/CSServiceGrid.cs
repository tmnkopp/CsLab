using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CyberBalance.CS.Core.Document;
using CyberBalance.CS.Web.UI;
using CyberBalance.VB.Web.UI;
using SpreadsheetLight;
using Telerik.Web.UI;
[assembly: System.Web.UI.WebResource("CyberScope.CS.Lab.Resources.Script.js", "application/x-javascript")]
namespace CyberScope.CS.Lab 
{
  
    public class CSServiceGrid : RadGrid
    {
        #region FIELDS
        private DataTable dataTable { get; set; } = new DataTable();
        public string PK_OrgSubmission { get; set; } = "";
        public string TableName { get; set; } = ""; 
        public string UserId { get; set; } = "0";
        public string CommandText { get; set; } = "";
        public string GridRecordCaption { get; set; } = "Service";
        protected class CommandItemTemplate : ITemplate
        {
            protected RadButton AddNewRecordButton = new RadButton() { ID = "AddNewRecordButton", Text = "Add New", CommandName = "InitInsert" };
            public RadButton DeleteAll = new RadButton() { ID = "DeleteAll", Text = "DeleteAll", CommandName = "DeleteAll", CommandArgument = "DeleteAll" }; 
            public CheckBox chkNA  = new CheckBox() { ID = "chkNA", Text = "Services Not Applicable", AutoPostBack = false }; 
            public CommandItemTemplate()  {  }
            public void InstantiateIn(System.Web.UI.Control container)
            {
                DeleteAll.Attributes.Add("onclick", "OnDeleteAllClick(this);");
                container.Controls.Add(AddNewRecordButton);
                container.Controls.Add(DeleteAll); 
                container.Controls.Add(chkNA);  
            }
        }

        #endregion

        #region PROPS

        private bool _IsNA;
        public bool IsNA
        {
            get
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CAClientConnectionString"].ConnectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(this.CommandText, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PK_OrgSubmission", this.PK_OrgSubmission);
                        cmd.Parameters.AddWithValue("@MODE", "GETNA");
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                _IsNA = Convert.ToInt32(rdr["ISNA"]) > 0;
                                break;
                            }
                        }
                    }
                }
                return _IsNA;
            }
        }

        #endregion

        #region EVENTS 
        public class ValidatingEventArgs : EventArgs
        {
            public bool IsValid { get; set; } = false;
            public GridCommandEventArgs GridCommandEventArgs { get; set; }
            public GridItem GridItem => GridCommandEventArgs?.Item;
            public ValidatingEventArgs()
            {
                this.IsValid = false;
            }
            public Control this[string ControlName] => GridItem?.FindControl(ControlName);
        }
        public event EventHandler<ValidatingEventArgs> OnRowValidating;
        protected virtual void DataValidating(ValidatingEventArgs e)
        {
            OnRowValidating?.Invoke(this, e);
        }
        public class RecordUpdatingEventArgs : EventArgs
        {
            public SqlCommand cmd { get; set; }
            public GridCommandEventArgs GridCommandEventArgs { get; set; }
            public GridItem GridItem => GridCommandEventArgs?.Item;
            public RecordUpdatingEventArgs(SqlCommand cmd, GridCommandEventArgs GridCommandEventArgs)
            {
                this.cmd = cmd;
                this.GridCommandEventArgs = GridCommandEventArgs;
            }
            public Control this[string ControlName] => GridItem?.FindControl(ControlName);
        }
        public event EventHandler<RecordUpdatingEventArgs> OnRecordUpdating;
        protected virtual void RecordUpdating(RecordUpdatingEventArgs e)
        {
            OnRecordUpdating?.Invoke(this, e);
        }
        public event EventHandler<RecordUpdatingEventArgs> OnRecordUpdated;
        protected virtual void RecordUpdated(RecordUpdatingEventArgs e)
        {
            OnRecordUpdated?.Invoke(this, e);
        }

        #endregion

        #region CTOR 
        public CSServiceGrid()
        {
            this.MasterTableView.AutoGenerateColumns = false;
            this.NeedDataSource += this.OnNeedDataSource;
            this.InsertCommand += this.Data_Submit;
            this.UpdateCommand += this.Data_Submit;
            this.DeleteCommand += this.Data_Submit;
            this.ItemDataBound += this.OnItemDataBound;
            this.ItemCommand += this.OnItemCommand;

            var cmt = new CommandItemTemplate();
            this.MasterTableView.CommandItemTemplate = cmt;
            this.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.Top;
            this.CssClass += " _CSServiceGrid_ ";
        } 
        #endregion

        #region METHODS

        #region METHODS: PROTECT
        protected override void OnInit(EventArgs e)
        { 
            base.OnInit(e);
        } 
     
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Page.ClientScript.RegisterClientScriptResource(typeof(CSServiceGrid), "CyberScope.CS.Lab.Resources.Script.js");
            GridItem[] gridItems = (GridItem[])this.MasterTableView.GetItems(GridItemType.CommandItem);
            if (gridItems.Count() > 0)
            {
                var item = this.MasterTableView.GetItems(GridItemType.CommandItem)[0];
                if (dataTable.Rows.Count < 1)
                {  
                    ((RadButton)item.FindControl("DeleteAll")).Enabled = false;
                } 
            }
            this.Visible = true;
        }
        protected void OnItemCommand(object source, GridCommandEventArgs e)
        {
            if (!",Cancel,InitInsert,Edit,Cancel,ExpandCollapse,Export".Contains($",{e.CommandName},"))
            {
                UpdateRecord(source, e);
            } 
        }
       
        protected void OnItemDataBound(object source, Telerik.Web.UI.GridItemEventArgs e)
        {
            DataRowView _DataRowView;
            GridCommandItem cmditem = (GridCommandItem)this.MasterTableView.GetItems(GridItemType.CommandItem)[0];
            if (this.MasterTableView.IsItemInserted && typeof(GridDataItem) == e.Item.GetType())
            {
                GridDataItem dataItem = ((GridDataItem)e.Item);
                dataItem["DeleteCol"].Controls[0].Visible = false;
                dataItem["EditCol"].Controls[0].Visible = false;
                ((RadButton)cmditem.FindControl("AddNewRecordButton")).Enabled = false;
                ((RadButton)cmditem.FindControl("DeleteAll")).Enabled = false;
            }
            if (e.Item.ItemType == GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.Item)
            {
                PK_OrgSubmission = ((GridDataItem)e.Item).GetDataKeyValue("PK_OrgSubmission")?.ToString();
            } 
            else if (e.Item.ItemType == GridItemType.CommandItem) 
            { 
                ((CheckBox)cmditem.FindControl("chkNA")).Checked = this.IsNA; 
                ((RadButton)cmditem.FindControl("AddNewRecordButton")).Enabled = !this.IsNA;
                ((RadButton)cmditem.FindControl("DeleteAll")).Enabled = !this.IsNA;
            }
            if (e.Item.ItemType == GridItemType.EditItem && e.Item.IsInEditMode)
            {
                if (!(e.Item is GridEditFormInsertItem || e.Item is GridDataInsertItem))
                {
                    PK_OrgSubmission = ((GridDataItem)e.Item).GetDataKeyValue("PK_OrgSubmission")?.ToString();
                    _DataRowView = (DataRowView)e.Item.DataItem;
                    int colCount = _DataRowView.Row.Table.Columns.Count;
                    for (int i = 0; i < colCount; i++)
                    {
                        var val = _DataRowView[i];
                        var key = _DataRowView.Row.Table.Columns[i];
                        Control control = ((GridDataItem)e.Item).FindControl(key.ColumnName);
                        SetControlValue(ref control, val);
                    }
                }
            } 
        } 
        protected void OnNeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CAClientConnectionString"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = this.CommandText;
            cmd.Parameters.AddWithValue("@PK_OrgSubmission", this.PK_OrgSubmission);
            cmd.Parameters.AddWithValue("@MODE", "SELECT");
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            dataTable = new DataTable();
            try
            {
                adapter.Fill(dataTable);
            }
            finally
            {
                conn.Close();
            }
            this.DataSource = dataTable;
        }
        #endregion

        #region METHODS: PRIV
        private void Data_Submit(object source, GridCommandEventArgs e)
        {
            var args = new ValidatingEventArgs();
            args.GridCommandEventArgs = e;
            DataValidating(args);
            if (args.IsValid)
            {
                UpdateRecord(source, e);
            }
        }
        private void UpdateRecord(object source, GridCommandEventArgs e)
        {
            string command = e.CommandName;
            string commandArg = (e.CommandArgument??"").ToString();
            if (!string.IsNullOrEmpty(commandArg)) 
                command = commandArg;
   
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = this.CommandText;
            cmd.Parameters.AddWithValue("@PK_OrgSubmission", this.PK_OrgSubmission);
            cmd.Parameters.AddWithValue("@UserId", this.UserId);
            cmd.Parameters.AddWithValue("@MODE", command);
            cmd.Parameters.Add("@OUT", SqlDbType.Int);
            cmd.Parameters["@OUT"].Direction = ParameterDirection.Output;
            if (command != "PerformInsert" && typeof(GridDataItem) == e.Item.GetType())
            {
                var key = this.MasterTableView.DataKeyNames.FirstOrDefault(); 
                var value = ((GridDataItem)e.Item).GetDataKeyValue(key)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    cmd.Parameters.AddWithValue($"@{key}", ((GridDataItem)e.Item).GetDataKeyValue(key)?.ToString());
                } 
            }

            var args = new RecordUpdatingEventArgs(cmd, e);
            ParamPopulator(ref cmd, e);
            RecordUpdating(args);

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CAClientConnectionString"].ConnectionString))
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                RecordUpdated(args);
            }
            this.DataBind();
        }
        private void SetControlValue(ref Control control, object val)
        {
            if (control != null)
            {

                Type ct = control.GetType();
                try
                {
                    if (ct == typeof(RadDropDownList))
                    {
                        DropDownListItem item = ((RadDropDownList)control).FindItemByValue(Convert.ToString(val));
                        if (item != null) 
                            item.Selected = true; 
                    }
                    else if (ct == typeof(RadComboBox))
                    {
                        var vals = Convert.ToString(val).Split(',');
                        foreach (RadComboBoxItem item in ((RadComboBox)control).Items)
                        {
                            item.Checked = (from v in vals where v == item.Value.Trim() select v).ToList().Count() > 0;
                        }
                    }
                    else if (ct == typeof(DropDownList))
                    {
                        ((DropDownList)control).Items.FindByValue(Convert.ToString(val)).Selected = true;
                    }
                    else if (ct == typeof(RadDatePicker))
                    {
                        ((RadDatePicker)control).SelectedDate = Convert.ToDateTime(val);
                    }
                    else if (ct == typeof(RadioButtonList))
                    {
                        ((RadioButtonList)control).SelectedValue = Convert.ToString(val);
                    }
                    else if (ct == typeof(RadCheckBox))
                    {
                        ((RadCheckBox)control).Checked = Convert.ToString(val) == "1";
                    }
                    else
                    {
                        PropertyInfo pi = control.GetType().GetProperty("Text");
                        if (pi != null)
                        {
                            pi?.SetValue(control, val.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
        }

        private void ParamPopulator(ref SqlCommand cmd, GridCommandEventArgs e)
        {
            int colCount = 0;
            if (GridItemType.EditItem == e.Item.ItemType)
            {
                colCount = ((GridDataItem)e.Item).OwnerTableView.Columns.Count;
            } 
            for (int i = 0; i < colCount; i++)
            {
                var col = ((GridDataItem)e.Item).OwnerTableView.Columns[i];
                if (!string.IsNullOrEmpty(col.UniqueName))
                {
                    var control = ((GridDataItem)e.Item).FindControl(col.UniqueName);
                    if (control != null)
                    {
                        object value;
                        PropertyInfo pi;
                        foreach (var item in new string[] { "Text", "SelectedValue" })
                        {
                            pi = control.GetType().GetProperty(item);
                            if (pi != null)
                            {
                                value = pi.GetValue(control);
                                cmd.Parameters.AddWithValue($"@{col.UniqueName}", value.ToString());
                                break;
                            }
                        }
                    }
                }
            }
        } 
        #endregion

        #endregion
    }
}
