using CyberBalance.VB.Core;
using CyberBalance.CS.Core;

namespace CyberScope.CS
{
    public class DataRequestContext{
        public DataRequestContext(SprocRequest sprocRequest)
        {
            this.SprocRequest = sprocRequest;
        }
        public SprocRequest SprocRequest { get; private set; } 
        public CAuser CAUser { get; set; } 
        public URLParms UrlParams { get; set; } 
        public object DataValue { get; set; } 
    }
}
