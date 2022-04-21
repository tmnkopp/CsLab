using System.Collections.Generic;

namespace CyberScope.CS.Lab.Models
{
    public class DataRequest
    {
        public string SprocName { get; set; } 
        public Dictionary<string, string> PARMS { get; set; }
        public DataRequest()
        { 
            PARMS = new Dictionary<string, string>();
        }
    }  
}
