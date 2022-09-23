using System.Text.RegularExpressions;

namespace CyberScope.CS
{
    public interface ITransformData
    {
        object Transform(DataRequestContext context);
    }

    public class UrlEncrypter : ITransformData
    {
        public object Transform(DataRequestContext context)
        {  
            object dataVal = context.DataValue; 
            if (Regex.IsMatch(dataVal.ToString(), $@"^~\/.+\.aspx\?.*"))
            {
                dataVal = System.Web.VirtualPathUtility.ToAbsolute(dataVal.ToString());
                dataVal = context.UrlParams.EncryptURL(dataVal.ToString());
            }
            return dataVal; 
        }
    }
}
