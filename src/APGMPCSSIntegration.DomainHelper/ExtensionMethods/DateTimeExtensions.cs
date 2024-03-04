namespace APGDigitalIntegration.DomainHelper.ExtensionMethods;

public static class DateTimeExtensions
{
    public static string ToISODateTime(this DateTime dateTime) => dateTime.ToString("yyyy-MM-ddTHH:mm:ss");
    public static string ToISODate(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd");
    
    public static string ToISODateTime(this DateTimeOffset dateTime) => dateTime.UtcDateTime.ToString("O");
    public static string ToISODate(this DateTimeOffset dateTime) => dateTime.UtcDateTime.ToString("yyyy-MM-dd");

}