using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CyberBalance.VB.Core;
using Newtonsoft.Json;

namespace CyberScope.CS.Lab.Models
{
    public class SprocRequest
    {
        public string SprocName { get; set; } 
        public Dictionary<string, string> PARMS { get; set; }
        public SprocRequest()
        { 
            PARMS = new Dictionary<string, string>();
        }
    }  
    public class DataTableResponse
    { 
        public string ResponseId { get; set; }
        public DataTable DataTable { get; set; }
    }
    public class DataResponseService{

        #region FIELDS
        private Dictionary<string, DataTable> DataResponses { get; set; } = new Dictionary<string, DataTable>();
        private Dictionary<string, SprocRequest> RequestCollection { get; set; } = new Dictionary<string, SprocRequest>();
        private CAuser _CAUser { get; set; }
        #endregion

        #region CTOR
        public DataResponseService()
        { 
        }
        public DataResponseService(Dictionary<string, SprocRequest> RequestCollection):base()
        {
            this.RequestCollection = RequestCollection;
        }
        #endregion

        #region FLUENT ACCESSORS 
        public DataResponseService ApplySprocRequest(Dictionary<string, SprocRequest> RequestCollection)
        {
            this.RequestCollection = RequestCollection;
            return this;
        }
        public DataResponseService PerformRequest()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CAClientConnectionString"].ConnectionString);
            conn.Open();
            SqlCommand cmd;
            try
            {
                foreach (var request in RequestCollection)
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = request.Value.SprocName; 
                    foreach (var parm in request.Value.PARMS)
                    {
                        cmd.Parameters.AddWithValue($"@{parm.Key}", parm.Value);
                    }
                    if (_CAUser != null)
                    {
                        if (!cmd.Parameters.Contains("@PK_OrgSubmission") && SprocHasParam(cmd, "@PK_OrgSubmission"))
                            cmd.Parameters.AddWithValue($"@PK_OrgSubmission", _CAUser.PK_OrgSubmission);
                        if (!cmd.Parameters.Contains("@UserId") && SprocHasParam(cmd, "@UserId"))
                            cmd.Parameters.AddWithValue($"@UserId", _CAUser.UserPK);
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    this.DataResponses.Add(request.Key, dataTable);
                    cmd.Dispose();
                }
            }
            finally
            {
                conn.Close();
            }
            return this;
        }

        public DataResponseService SetUser(CAuser CAuser)
        {
            this._CAUser = CAuser;
            return this;
        }

        public DataResponseService ApplyUrlEncryption(Func<string, string> UrlEncrypter)
        {
            var keys = DataResponses.Keys.Select(k => k).ToArray();
            foreach (var key in keys)
            {
                var dt = DataResponses[key];
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        var val = dr[col].ToString();
                        if (Regex.IsMatch(val, $@"^~\/.+\.aspx\?.*"))
                        {
                            var url = dr[col].ToString();
                            url = System.Web.VirtualPathUtility.ToAbsolute(url);
                            url = UrlEncrypter(url);
                            dr[col] = url;
                        }
                    }
                }
                DataResponses[key] = dt;
            }
            return this;
        } 
        public Dictionary<string, DataTable> GetResponseAsDataTables()
        {
            return DataResponses;
        } 
        public string GetResponseAsJson()
        {
            return JsonConvert.SerializeObject(DataResponses);
        }

        #endregion
         
        #region PRIV METHODS
        private bool SprocHasParam(SqlCommand command, string Param)
        {
            SqlCommand cmd = command.Clone();
            SqlCommandBuilder.DeriveParameters(cmd);
            foreach (var @param in cmd.Parameters)
            {
                if (@param.ToString().Contains($"@{Param.Replace("@", "")}"))
                    return true;
            }
            return false;
        } 
        #endregion
    
    }
}
