using System.Collections.Generic;

namespace CyberScope.CS.Lab.Services
{
    public class DataRequest
    {
        public string SprocName { get; set; } 
        public Dictionary<string, string> PARMS { get; set; }
        public List<string> DataTransforms { get; set; }
        public DataRequest()
        { 
            PARMS = new Dictionary<string, string>();
            DataTransforms = new List<string>();
        }
    }  
}
