using System.Text.Json;

namespace APG.MessageQueue.Contracts.Notifications;

public class SendPushNotification
{
    private static readonly JsonSerializerOptions SerializeOptions;

    static SendPushNotification()
    {
        SerializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }
    
    public SendPushNotification(string userId, string sessionId, string eventFrom, string hubMethod, object message)
    {
        this.UserId = userId;
        this.SessionId = sessionId;
        this.EventFrom = eventFrom;
        this.HubMethod = hubMethod;
        this.Message = JsonSerializer.Serialize(message, SerializeOptions);
    }

    public SendPushNotification()
    {
        
    }

    public string UserId { get; set; }
    public string SessionId { get; set; }
    public string EventFrom { get; set; }
    public string HubMethod { get; set; }
    public object Message { get; set; }
}