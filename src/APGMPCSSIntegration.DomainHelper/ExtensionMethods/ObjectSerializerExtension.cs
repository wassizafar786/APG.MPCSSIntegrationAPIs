using System.Text.Json;

namespace APGDigitalIntegration.DomainHelper
{
    public static  class ObjectSerializerExtension
    {
        public static string SerializeObject(this object obj)
        {
            if (obj is null)
                return string.Empty;

            return obj is string
                ? obj.ToString()
                : JsonSerializer.Serialize(obj);
        }
    }
}
