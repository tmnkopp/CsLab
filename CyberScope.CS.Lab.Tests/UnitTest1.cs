using CyberBalance.VB.Core;
using CyberBalance.VB.Web.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CyberScope.CS.Lab.Tests
{
    [TestClass]
    public class UnitTest1
    {
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

}
