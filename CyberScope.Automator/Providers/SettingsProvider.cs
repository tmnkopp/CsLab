using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO; 
using System.Text.RegularExpressions;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace CyberScope.Automator
{
    public class ControlLocator{
        public string Locator { get; set; }
        public string Type { get; set; }
        public string Selector { get; set; }
        private string _ValueSetterTypes;
        public string ValueSetterTypes { get; set; } = ".*";
    }
    public class TestConfig{
        public DataCallTestConfig DataCall { get; set; }
        public UserMgmtTestConfig UserMgmt { get; set; } 
    }
    public class DataCallTestConfig
    {
        public string Tab { get; set; }
        public string Sections { get; set; } = ".*";
    }
    public class UserMgmtTestConfig
    {
        public string User { get; set; }
        public string Agency { get; set; }
    }

    public static class SettingsProvider
    { 
        #region PROPS 
        private static string RawConfig() {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            return File.ReadAllText($"{dirPath}\\AutomatorConfig.json"); 
        }
        public static TestConfig TestConfig
        { 
            get {
                dynamic json = JsonConvert.DeserializeObject(RawConfig());
                string json_section = JsonConvert.SerializeObject(json.TestConfig); 
                json_section = json_section.Replace("{AppSettings:AgencyUser}", ConfigurationManager.AppSettings.Get($"AgencyUser"));
                TestConfig config;
                config = JsonConvert.DeserializeObject<TestConfig>
                        (json_section);
                return config; 
            }  
        }
        public static List<string> ChromeOptions
        {
            get
            { 
                dynamic json = JsonConvert.DeserializeObject(RawConfig());
                var options = json.ChromeOptions; 
                return ((JArray)options).Select(i => (string)i).ToList();
            }
        }
        public static Dictionary<string, Dictionary<string, string>> InputDefaults
        {
            get {
                var json_config = RawConfig();
                dynamic obj = JsonConvert.DeserializeObject(json_config);
                string json_input_defaults = JsonConvert.SerializeObject(obj.InputDefaults);
                Dictionary<string, Dictionary<string, string>> config; 
                config = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>
                    (json_input_defaults);
                return config;
            }
        } 
        public static List<ControlLocator> ControlLocators
        { 
            get { 
                var local_settings = RawConfig();
                List<ControlLocator> locators = new List<ControlLocator>();
                dynamic json = JsonConvert.DeserializeObject(local_settings); 
                foreach (var item in json.ControlLocators)
                {
                    ControlLocator i = JsonConvert.DeserializeObject<ControlLocator>(JsonConvert.SerializeObject(item));
                    locators.Add(i); 
                } 
                return locators;
            }
        }
        #endregion 
    }
}