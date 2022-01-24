using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace CyberScope.CS.Lab 
{
    public class CBGrid : RadGrid
    {
        private static IEnumerable<Type> GetTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsClass);
        }
        public CBGrid()
        {
            this.MasterTableView.AutoGenerateColumns = false;
            this.NeedDataSource += this.OnNeedDataSource;
            //this.ItemDataBound += this.OnItemDataBound;
            this.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
            this.MasterTableView.EditMode = GridEditMode.InPlace; 
            CreateGridTemplateColumn("StartingIP", "TextBox");
            CreateGridTemplateColumn("EndingIP", "TextBox");
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
        private void CreateGridTemplateColumn(string name, string ControlType)
        {
            GridTemplateColumn gtc = new GridTemplateColumn();
            gtc.UniqueName = name;
            gtc.ItemTemplate = new cItemTemplate(name);
            gtc.EditItemTemplate = new cEditTemplate(name, ControlType);
            this.MasterTableView.Columns.Add(gtc);
        }
      
        private class cEditTemplate : IBindableTemplate
        {
            protected Control ctrl;
            private string id;
            private string ctrlType;
            public cEditTemplate(string id, string ControlType)
            {
                this.id = id;
                this.ctrlType = ControlType;
            }
            public IOrderedDictionary ExtractValues(Control container)
            {
                var od = new System.Collections.Specialized.OrderedDictionary();
                var val = PropGetter(ctrl);
                od.Add(this.id, val);
                return od;
            }
            public void InstantiateIn(System.Web.UI.Control container)
            {
                var typ = GetTypes().Where(t => t.Name.ToUpper() == ctrlType.ToUpper()).FirstOrDefault();
                this.ctrl = (Control)Activator.CreateInstance(Type.GetType($"{typ.FullName}, {typ.Namespace}"), new object[] { });
                ctrl.ID = this.id;
                ctrl.DataBinding += new EventHandler(_DataBinding);
                container.Controls.Add(ctrl);
            }
            void _DataBinding(object sender, EventArgs e)
            {
                Control ctrl = (Control)sender;
                GridDataItem container = (GridDataItem)ctrl.NamingContainer;
                var val = ((DataRowView)container.DataItem)[this.id].ToString();
                PropSetter(ctrl, val);
            }
            private void PropSetter(Control ctrl, string val)
            {
                PropertyInfo pi;
                Type ct = ctrl.GetType();
                foreach (var prop in new string[] { "SelectedValue", "Value", "Text" })
                {
                    pi = ctrl.GetType().GetProperty(prop);
                    if (pi != null)
                    {
                        pi?.SetValue(ctrl, val.ToString());
                    }
                }
            }
            private string PropGetter(Control control)
            {
                PropertyInfo pi;
                Type ct = control.GetType();
                foreach (var prop in new string[] { "SelectedValue", "Value", "Text" })
                {
                    pi = control.GetType().GetProperty(prop);
                    if (pi != null)
                    {
                        var val = pi.GetValue(control)?.ToString();
                        return val;
                    }
                }
                return null;
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
        protected void OnNeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CAClientConnectionString"].ConnectionString);
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "EinsteinPublicIP_CRUD";
            cmd.Parameters.AddWithValue("@PK_OrgSubmission", "26037"); 
            cmd.Parameters.AddWithValue("@MODE", "SELECT");
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
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
    }
}
