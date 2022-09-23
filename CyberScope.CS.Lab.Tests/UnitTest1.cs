using CyberBalance.VB.Core;
using CyberBalance.VB.Web.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CyberBalance.CS.Core;
using System.Linq;
using System.Data;
using System.Collections.Generic;

namespace CyberScope.CS.Lab.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Automator_Inits()
        {
            List<ITransformData> transforms = new List<ITransformData>();
            
            var sr = new SprocRequest();
            sr.SprocName = "cq_BOD2201Unremediated"; 
            sr.PARMS.Add("Generic1", "30579");
             
            var response = new SprocRequestService(sr).GetResponse(); 
            sr.PARMS.Clear(); 
        }  
    }

    public static class VirtualPathUtility{
        public static string ToAbsolute(string val){
            return val;
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
    }
}
