using System.Text.Json.Serialization;

namespace APG.MessageQueue.Contracts.Notifications;

public class DeleteMerchantNotificationToken
{
    [JsonIgnore]
    public long MerchantRefId { get; set; }
}