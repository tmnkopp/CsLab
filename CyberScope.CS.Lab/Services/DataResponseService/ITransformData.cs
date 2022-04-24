using CyberBalance.VB.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CyberScope.CS.Lab.Services 
{
    public interface ITransformData
    {
        DataTable Transform(DataTransformContext context);
    }
    public struct DataTransformContext
    {
        public DataTransformContext(DataTable dataTable, CAuser _CAuser, URLParms  _UrlParams)
        {
            this._CAuser = _CAuser;
            this._UrlParams = _UrlParams;
            this.dataTable = dataTable;

        }
        public CAuser _CAuser { get; set; }
        public URLParms _UrlParams { get; set; }
        public DataTable dataTable { get; set; } 
    }
    public class UrlEncryption : ITransformData
    {
        public DataTable Transform(DataTransformContext context)
        { 
            var dt = context.dataTable;
            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    var dataVal = dr[col].ToString();
                    if (Regex.IsMatch(dataVal, $@"^~\/.+\.aspx\?.*"))
                    {
                        dataVal = System.Web.VirtualPathUtility.ToAbsolute(dataVal);
                        dataVal = context._UrlParams.EncryptURL(dataVal);
                    }
                    dr[col] = dataVal;
                }
            }
            return dt;
        }
    }
}
