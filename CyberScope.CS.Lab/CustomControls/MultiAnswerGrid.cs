using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using CyberBalance.VB.Core; 
namespace CyberScope.CS.Lab 
{ 
    public class MultiAnswerGrid : RadGrid
    {
        #region CTOR 
        protected CAuser _CAUser;
        protected URLParms _UrlParams;
        public MultiAnswerGrid()
        {
            CBWebBase.Init(ref _CAUser, ref _UrlParams);
            this.CssClass += " _MultiAnswerGrid_ ";
        }
        #endregion

        #region FIELDS 
        private DataTable dataTable { get; set; } = new DataTable();
        private string PK_Question { get; set; }
        public string PK_QuestionGroup { get; set; }
        public string ColumnCaptionPKs { get; set; }
        public string RowCaptionField { get; set; } = "CAPTION";
        public string CaptionFieldHeaderText { get; set; } = "Metric";
        public string Fields { get; set; }
        public string StoredProcedure { get; set; }  
        public bool IsPostBack { get; set; } = false;
        private string _userid;
        public string UserId
        {
            get { return _userid ?? _CAUser.UserPK.ToString(); }
            set { _userid = value; }
        }
        private string _PK_OrgSubmission;
        public string PK_OrgSubmission
        {
            get { return _PK_OrgSubmission ?? _UrlParams.GetParm("PK_OrgSubmission"); }
            set { _PK_OrgSubmission = value; }
        }
        private List<string> _colNames = new List<string>(); 
        public List<string> ColNames
        {
            get {
                if (_colNames.Count < 1) 
                     getColNamesFromPKQuestions(); 
                return _colNames; 
            } 
        } 
        #endregion

        #region ItemTemplates
        private class cEditTemplate : IBindableTemplate
        {
            protected RadNumericTextBox ctrl;
            private string id;
            public cEditTemplate(string id)
            {
                this.id = id;
            }
            public IOrderedDictionary ExtractValues(Control container)
            {
                var od = new System.Collections.Specialized.OrderedDictionary();
                od.Add(this.id, ctrl.Text);
                return od;
            }
            public void InstantiateIn(System.Web.UI.Control container)
            {
                ctrl = new RadNumericTextBox();
                ctrl.ID = this.id;
                var numberFormat = ctrl.NumberFormat;
                numberFormat.DecimalDigits = 0;
                ctrl.DataBinding += new EventHandler(_DataBinding);
                container.Controls.Add(ctrl);
            }
            void _DataBinding(object sender, EventArgs e)
            {
                RadNumericTextBox txt = (RadNumericTextBox)sender;
                GridDataItem container = (GridDataItem)txt.NamingContainer;
                txt.Text = ((DataRowView)container.DataItem)[this.id].ToString();
            }
        }
        private class cItemTemplate : ITemplate
        {
            protected Label ctrl;
            private string id;
            public cItemTemplate(string id)
            {
                this.id = id;
            }
            public void InstantiateIn(System.Web.UI.Control container)
            {
                ctrl = new Label();
                ctrl.ID = this.id;
                ctrl.DataBinding += new EventHandler(_DataBinding);
                container.Controls.Add(ctrl);
            }
            void _DataBinding(object sender, EventArgs e)
            {
                Label txt = (Label)sender;
                GridDataItem container = (GridDataItem)txt.NamingContainer;
                txt.Text = ((DataRowView)container.DataItem)[this.id].ToString();
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
         
        #region METHODS
         
        protected override void OnInit(EventArgs e)
        {  
            this.MasterTableView.AutoGenerateColumns = false;
            this.NeedDataSource += this.OnNeedDataSource; 
            this.ItemDataBound += this.OnItemDataBound;
            this.ItemCommand += this.OnItemCommand;
            this.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;

            var gecc = new GridEditCommandColumn();
            gecc.UniqueName = "EditCommandColumn";
            this.MasterTableView.Columns.Add(gecc);

            var bc = new GridBoundColumn();
            bc.DataField = this.RowCaptionField;
            bc.HeaderText = this.CaptionFieldHeaderText;
            bc.ReadOnly = true;
            this.MasterTableView.Columns.Add(bc);

            if (!string.IsNullOrEmpty(Fields))
            {
                var _fields = Regex.Replace(Fields, @"\s", "").Split(',');
                for (int i = 0; i < _fields.Count(); i++)
                {  
                    GridTemplateColumn gtc = new GridTemplateColumn();
                    gtc.UniqueName = _fields[i];
                    gtc.ItemTemplate = new cItemTemplate(_fields[i]);
                    gtc.EditItemTemplate = new cEditTemplate(_fields[i]);
                    gtc.HeaderText = (ColNames.Count > i) ? ColNames[i] : _fields[i];
                    this.MasterTableView.Columns.Add(gtc);
                } 
            } 
            this.MasterTableView.EditMode = GridEditMode.InPlace; 
            this.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None; 
            base.OnInit(e); 
        }  
        protected override void OnLoad(EventArgs e)
        { 
            base.OnLoad(e); 
        }   
        protected void OnItemDataBound(object source, Telerik.Web.UI.GridItemEventArgs e)
        {
            DataRowView _DataRowView; 
            if (e.Item.ItemType == GridItemType.EditItem && e.Item.IsInEditMode)
            {
                if (!(e.Item is GridEditFormInsertItem || e.Item is GridDataInsertItem))
                { 
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
            cmd.CommandText = this.StoredProcedure;
            cmd.Parameters.AddWithValue("@PK_OrgSubmission", this.PK_OrgSubmission);
            if (!string.IsNullOrEmpty(this.PK_QuestionGroup))
            {
                cmd.Parameters.AddWithValue("@PK_QuestionGroup", this.PK_QuestionGroup);
            } 
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
        protected void OnItemCommand(object source, GridCommandEventArgs e)
        {
            if (Regex.IsMatch(e.CommandName, $"Update"))
            {
                var args = new ValidatingEventArgs();
                args.GridCommandEventArgs = e;
                DataValidating(args);
                if (args.IsValid)
                {
                    UpdateRecord(source, e);
                }
            }
            if (Regex.IsMatch(e.CommandName, $"Edit"))
                this.Rebind();
        }

        protected void UpdateRecord(object source, GridCommandEventArgs e)
        {
            string command = e.CommandName; 
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = this.StoredProcedure;
            cmd.Parameters.AddWithValue("@PK_OrgSubmission", this.PK_OrgSubmission);
            cmd.Parameters.AddWithValue("@UserId", this.UserId);
            cmd.Parameters.AddWithValue("@MODE", command);
            foreach (var DataKey in this.MasterTableView.DataKeyNames)
            {
                if (!DataKey.Contains("PK_OrgSubmission"))
                {
                    var val = ((GridDataItem)e.Item).GetDataKeyValue(DataKey)?.ToString();
                    if (!string.IsNullOrEmpty(val)) 
                        cmd.Parameters.AddWithValue($"@{DataKey}", val); 
                } 
            }  
            cmd.Parameters.Add("@OUT", SqlDbType.Int);
            cmd.Parameters["@OUT"].Direction = ParameterDirection.Output;
       
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
            this.Rebind();
        }

        private void getColNamesFromPKQuestions()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CAClientConnectionString"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = this.StoredProcedure;
            if (!string.IsNullOrEmpty(this.ColumnCaptionPKs))
            {
                cmd.Parameters.AddWithValue("@ColumnCaptionPKs", this.ColumnCaptionPKs);
            }
            cmd.Parameters.AddWithValue("@MODE", "SELECT_LABELS");
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
            foreach (DataRow item in dataTable.Rows)
            {
                _colNames.Add(item["QuestionText"].ToString());
            }
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
    }
}
