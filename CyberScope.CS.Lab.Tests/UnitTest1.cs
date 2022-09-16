using CyberBalance.VB.Core;
using CyberBalance.VB.Web.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using CyberBalance.CS.Core;
using System.Data.SqlClient;
using System.Linq;

namespace CyberScope.CS.Lab.Tests
{
 
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Automator_Inits()
        {
            var dr = new DataRequest();
            var rs = new DataPostService().AddRequest(dr).Execute();
            rs = new DataPostService();
            rs.SprocName = "SprocName";
            rs.PARAMS.Add("", "");
            rs.Execute();
        }

        [TestMethod]
        public void TestMethod1()
        {
            bool a=true, b=false ;
            var x = (a) ? "src" : "dist";
        }
    }
    public class CBServiceUtil
    {
        public CBServiceUtil()
        {
            CAuser _CAUser = null;
            URLParms _UrlParams = null;
            CBWebBase.Init(ref _CAUser, ref _UrlParams);

        }
        //_UrlParams.EncryptURL(UrlToNavigate)
    }

    public class DataPostService{
        private Dictionary<string, DataRequest> requests = new Dictionary<string, DataRequest>();
        public Dictionary<string, string> PARAMS { get; set; } = new Dictionary<string, string>();
        public string SprocName { get; set; }
        public DataPostService()
        { 
        }
        public DataPostService(string SprocName)
        {
            this.SprocName = SprocName; 
        }
        #region EVENTS   
        public event EventHandler<CommandEventArgs> OnCommandExecuted;
        protected virtual void CommandExecuted(CommandEventArgs e)
        {
            OnCommandExecuted?.Invoke(this, e);
        }
        public event EventHandler<CommandEventArgs> OnCommandExecuting;
        protected virtual void CommandExecuting(CommandEventArgs e)
        {
            OnCommandExecuting?.Invoke(this, e);
        }
        public class CommandEventArgs : EventArgs
        {
            public SqlCommand cmd { get; set; }
            public DataRequest DataRequest { get; set; }
            public CommandEventArgs(ref SqlCommand cmd, ref DataRequest dataRequest)
            {
                this.cmd = cmd;
                this.DataRequest = dataRequest;
            }
        }  
        #endregion
        
        public DataPostService AddRequest(string SprocName, Dictionary<string, string> PARAMS)
        { 
            var req = new DataRequest(){ SprocName = SprocName, PARMS = PARAMS };  
            requests.Add(Guid.NewGuid().ToString(), req);
            return this; 
        }
        public DataPostService AddRequest(DataRequest request)
        {  
            requests.Add(Guid.NewGuid().ToString(), request); 
            return this;
        }
        public DataPostService AddRequests(Dictionary<string, DataRequest> DataRequests)
        {
            DataRequests.ToList().ForEach(i => this.requests.Add(i.Key, i.Value)); 
            return this;
        }
        public DataPostService Execute()
        {
            if (PARAMS.Count > 0 && !string.IsNullOrEmpty(SprocName)) 
                AddRequest(SprocName, PARAMS); 
      
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CAClientConnectionString"].ConnectionString);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            (from r in requests.Values select r).ToList<DataRequest>().ForEach(dr =>
            {
                cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = dr.SprocName; 
                try
                {
                    foreach (var @param in dr.PARMS) 
                        cmd.Parameters.AddWithValue($"@{@param.Key}", @param.Value);
                    var args = new CommandEventArgs(ref cmd, ref dr);
                    CommandExecuting(args);
                    cmd.ExecuteNonQuery();
                    CommandExecuted(args);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                { 
                    conn.Close();
                }
            });
            cmd.Dispose();
            conn.Close();
            return this;
        } 
    } 
}
