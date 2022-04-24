using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CyberBalance.VB.Core;
using Newtonsoft.Json;

namespace CyberScope.CS.Lab.Services
{
    public class DataResponseService{

        #region FIELDS
        private Dictionary<string, DataTable> DataResponseDict { get; set; } = new Dictionary<string, DataTable>();
        private Dictionary<string, DataRequest> RequestCollection { get; set; } = new Dictionary<string, DataRequest>(); 
        private CAuser _CAuser { get; set; }
        private URLParms _UrlParams { get; set; }
        private List<ITransformData> DataTransforms { get; set; } = new List<ITransformData>();
        #endregion

        #region CTOR
        public DataResponseService()
        { 

        }
        #endregion

        #region BUILDERS PUBLIC  
        public DataResponseService ApplyRequest(Dictionary<string, DataRequest> Request)
        {
            this.RequestCollection = Request;
            return this;
        }
        public DataResponseService SetUser(CAuser CAuser)
        {
            this._CAuser = CAuser;
            return this;
        }
        private bool _ApplyUrlEncryption = false;
        public DataResponseService ApplyUrlEncryption(URLParms URLParms)
        {
            this._UrlParams = URLParms;
            _ApplyUrlEncryption = true;
            return this;
        } 
        public Dictionary<string, DataTable> GetDataTables()
        {
            PopulateDataTables();
            return DataResponseDict;
        }
        public string GetResponse()
        { 
            return JsonConvert.SerializeObject(GetDataTables()); 
        }
        #endregion

        #region PRIV METHODS
        private DataResponseService PopulateDataTables()
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
                    if (_CAuser != null)
                    {
                        if (!cmd.Parameters.Contains("@PK_OrgSubmission") && SprocHasParam(cmd, "@PK_OrgSubmission"))
                            cmd.Parameters.AddWithValue($"@PK_OrgSubmission", _CAuser.PK_OrgSubmission);
                        if (!cmd.Parameters.Contains("@UserId") && SprocHasParam(cmd, "@UserId"))
                            cmd.Parameters.AddWithValue($"@UserId", _CAuser.UserPK);
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                     
                    foreach (var dt in request.Value.DataTransforms)
                    {
                        var typ = AssmTypes().Where(t => t.Name == dt && typeof(ITransformData).IsAssignableFrom(t)).FirstOrDefault();
                        if (typ != null)
                        {
                            ITransformData obj = (ITransformData)Activator.CreateInstance(typ);
                            DataTransformContext dtc = new DataTransformContext(dataTable, _CAuser, _UrlParams); 
                            dataTable = obj.Transform(dtc);
                        }
                    }

                    this.DataResponseDict.Add(request.Key, dataTable);
                    cmd.Dispose(); 
                }
            }
            finally
            {
                conn.Close();
            }
 
            if (_ApplyUrlEncryption)
            {
                ApplyUrlEncryption();
            }
            return this;
        }
        private Type[] AssmTypes()
        {
            Assembly assm = Assembly.GetExecutingAssembly();
            return assm.GetTypes();
        }
        private void ApplyUrlEncryption()
        {
            var keys = DataResponseDict.Keys.Select(k => k).ToArray();
            foreach (var key in keys)
            {
                var dt = DataResponseDict[key];
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        var dataVal = dr[col].ToString();
                        if (Regex.IsMatch(dataVal, $@"^~\/.+\.aspx\?.*"))
                        { 
                            dataVal = System.Web.VirtualPathUtility.ToAbsolute(dataVal);
                            dataVal = _UrlParams.EncryptURL(dataVal); 
                        }
                        dr[col] = dataVal;
                    }
                }
                DataResponseDict[key] = dt;
            } 
        }   

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
