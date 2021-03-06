using System;
namespace CyberScope.Automator
{
    [AttributeUsage(System.AttributeTargets.Class)]
    public class ConnectorMeta : Attribute
    {
        public string Selector { get; set; }
        public ConnectorMeta(string Selector)
        {
            this.Selector = Selector;
        }
    }
}