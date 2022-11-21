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
using System.Text.RegularExpressions;

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
            var srs = new SprocRequestService(r=>{
                r.SprocName = "spSprocMeta";
                r.PARMS.Add("MODE", "GET");
            });
            var dt = srs.GetDataTable(); 
            var ser = JsonConvert.SerializeObject(dt);
           
            Console.Write(ser);
        }  
    }
}
