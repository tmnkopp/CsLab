using CyberBalance.CS.Web.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace CyberScope.CS.Lab 
{
   
    public class CBCommandItemGrid : CBDataGrid
    {
        private bool? _IsNA;
        public bool IsNA
        {
            get
            {
                if (!AllowNA)
                    _IsNA = false;
                if (_IsNA == null)
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CAClientConnectionString"].ConnectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(this.StoredProcedure, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@PK_OrgSubmission", this.PK_OrgSubmission);
                            cmd.Parameters.AddWithValue($"{StoredProcedureCommandParam}", "GETNA");
                            SqlParameter outParam = cmd.Parameters.Add("@OUT", SqlDbType.Int);
                            outParam.Direction = ParameterDirection.Output;
                            using (SqlDataReader rdr = cmd.ExecuteReader())
                            {
                            }
                            if (outParam.Value != DBNull.Value)
                                _IsNA = Convert.ToInt32(outParam?.Value ?? 0) > 0;
                        }
                    }
                }
                return _IsNA ?? false;
            }
            set { _IsNA = value; }
        }

        #region CommandItemTemplate
        public bool AllowNA { get; set; } = true;
        public bool AllowClearAll { get; set; } = true;
        public bool AllowAddNew { get; set; } = true;
        public bool AllowDownload { get; set; } = true; 
        protected class CommandItemTemplate : ITemplate
        {
            public RadButton AddNewRecordButton = new RadButton() { ID = "AddNewRecordButton", Text = "Add New", CommandName = "InitInsert" };
            public RadButton DownloadButton = new RadButton() { ID = "DownloadButton", Text = "Download", CommandName = "Export" };
            public RadButton DeleteAll = new RadButton() { ID = "DeleteAll", Text = "DeleteAll", CommandName = "DeleteAll", OnClientClicking = "OnDeleteAllClick" };
            public CheckBox chkNA = new CheckBox() { ID = "chkNA", AutoPostBack = false };
            public CommandItemTemplate() { }
            public void InstantiateIn(System.Web.UI.Control container)
            { 
                container.Controls.Add(AddNewRecordButton);
                container.Controls.Add(DeleteAll);
                container.Controls.Add(chkNA);
                container.Controls.Add(DownloadButton);
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Page.ClientScript.RegisterClientScriptInclude("CBDataGrid", Page.ResolveUrl("~/Scripts/CBDataGrid.js"));
            if (GridCommandItem != null )
            {
                ((RadButton)GridCommandItem.FindControl("DeleteAll")).Enabled = dataTable.Rows.Count > 0;
                ((CheckBox)GridCommandItem.FindControl("chkNA")).Checked = this.IsNA;
                ((CheckBox)GridCommandItem.FindControl("chkNA")).InputAttributes.Add("data-grid-id", this.ClientID);
                ((CheckBox)GridCommandItem.FindControl("chkNA")).Text = $"{GridRecordCaption} Not Applicable";
                ((CheckBox)GridCommandItem.FindControl("chkNA")).Visible = AllowNA;
                ((RadButton)GridCommandItem.FindControl("AddNewRecordButton")).Enabled = !this.IsNA;
                ((RadButton)GridCommandItem.FindControl("AddNewRecordButton")).Visible = AllowAddNew;
                ((RadButton)GridCommandItem.FindControl("DeleteAll")).Enabled = !this.IsNA;
                ((RadButton)GridCommandItem.FindControl("DeleteAll")).Visible = AllowClearAll;
                ((RadButton)GridCommandItem.FindControl("DownloadButton")).Visible = AllowDownload;
            }
        }
        #endregion
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e); 
            this.MasterTableView.CommandItemTemplate = new CommandItemTemplate();
            StoredProcedureCommands = $"PerformInsert, Update, Delete, DeleteAll, CancelAll, EditAll, Export, {StoredProcedureCommands ?? "PerformInsert"}";
        } 
    }
}
