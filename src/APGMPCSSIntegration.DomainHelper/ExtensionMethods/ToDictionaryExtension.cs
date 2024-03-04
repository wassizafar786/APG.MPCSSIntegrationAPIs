using System.Net;
using System.Text;
namespace APGMPCSSIntegration.DomainHelper
{
    public static class ToDictionaryExtension
    {
        // public static Dictionary<string, string> ToDictionary(this IPrimitiveMap map)
        // {
        //     if (map == null)
        //         return (Dictionary<string, string>)null;
        //     Dictionary<string, string> dictionary = new Dictionary<string, string>();
        //     foreach (string key in (IEnumerable)map.Keys)
        //         dictionary.Add(key, map[key].ToString());
        //     return dictionary;
        // }

        public static string AsQueryString(this IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (!parameters.Any())
                return "";

            var builder = new StringBuilder("?");

            var separator = "";
            foreach (var kvp in parameters)
            {
                if (kvp.Value == null)
                    continue;

                builder.Append($"{separator}{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}");
                separator = "&";
            }

            return builder.ToString();
        }
    }
}