using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace APGDigitalIntegration.DomainHelper.Services;

public static class XmlSerializationHelper
{
    public static string ToXml<T>(this T message) where T : class
    {
        return Serialize(message);
    }
    
    public static string Serialize<T>(T message)
    {
        var serializer = new XmlSerializer(typeof(T));

        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            Indent = false
        };

        var stringWriter = new StringWriter();

        using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
        {
            serializer.Serialize(xmlWriter, message);
        }

        var xml = stringWriter.ToString();
        stringWriter.Close();
            
        return xml;
    }

    public static T Deserialize<T>(string content)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(T));

            using var reader = new StringReader(content);
            var serializedContent = (T)serializer.Deserialize(reader);

            return serializedContent;
        }
        catch (Exception ex)
        {
            return default;
        }
    }
    
    public static object Deserialize(string content, Type type)
    {
        try
        {
            var serializer = new XmlSerializer(type);

            using var reader = new StringReader(content);
            var serializedContent = serializer.Deserialize(reader);

            return serializedContent;
        }
        catch (Exception ex)
        {
            return default;
        }
    }
}