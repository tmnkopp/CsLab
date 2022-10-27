using CyberBalance.VB.Core;
using CyberBalance.VB.Web.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CyberBalance.CS.Core;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace CyberScope.CS.Lab.Tests
{
    class parm{
        public string ParamName { get; set; }
        public int ParamMaxLen { get; set; }
        public bool ParamIsOutput { get; set; }
        public string ParamType { get; set; }
    } 
    class sp{
        public sp()
        {
            parms = new List<parm>();
        }
        public string SprocName { get; set; }
        public string ApiHandler { get; set; }
        public List<parm> parms { get; set; }
    }
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Automator_Inits()
        {
            var sprocs = GetData(); 
            var ser = JsonConvert.SerializeObject(sprocs);
            var deser = JsonConvert.DeserializeObject<List<dynamic>>(ser);
 
            Console.Write(sprocs);
        }  
   

        public static DataTable GetData(){
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CAClientConnectionString"].ConnectionString);
            conn.Open();
            SqlCommand cmd;
            try
            { 
                cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spSprocMeta"; 
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        } 
    }
}
