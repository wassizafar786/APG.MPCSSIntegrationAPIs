using System.Collections.Generic;

namespace APGMPCSSIntegration.Services.Resources
{
    class JsonLocalization
    {
        public string Key { get; set; }
        public Dictionary<string, string> LocalizedValue = new Dictionary<string, string>();
    }
}