using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.Constant;

public static class NotificationHubs // Should be refactored into Notification Service.
{
    public static string GetNotificationEventHub(int requestSourceId)
    {
        return requestSourceId switch
        {
            (int)RequestSources.AmwalCheckout => NotificationHub.SmartBoxEventFrom,
            (int)RequestSources.Portal => NotificationHub.PortalEventFrom,
            _ => null
        };
    }
}