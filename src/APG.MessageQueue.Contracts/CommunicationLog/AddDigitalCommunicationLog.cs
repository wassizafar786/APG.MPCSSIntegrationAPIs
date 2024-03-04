namespace APG.MessageQueue.Contracts.CommunicationLog;


// Publisher: APGDigitalIntegration
// Consumer:  APGLogs
public class AddDigitalCommunicationLog
{
    public string MsgId { get; set; }
    public string CorrelationId { get; set; }
    public string OriginalExternalMsgId { get; set; } // Used in Inward requests. ex: qr
    public string InternalRequest { get; set; }
    public string InternalResponse { get; set; }
    public bool IsUpdate { get; set; }
    public DateTime RequestDatetime { get; set; }
    public string ServiceName { get; set; }
    public DateTimeOffset? InternalRequestTime { get; set; }
    public string ExternalRequest { get; set; }
    public DateTimeOffset? ExternalRequestTime { get; set; }
    public string ExternalResponse { get; set; }
    public DateTimeOffset? ExternalResponseTime { get; set; }
    public DateTimeOffset? InternalResponseTime { get; set; }
    public long? MerchantRefId { get; set; }
    public long? TerminalNodeId { get; set; }
    public long? BankId { get; set; }
    public int? TransactionTypeId { get; set; }
    public int? RequestTypeId { get; set; }
    public string ResponseCode { get; set; }
    public string ExceptionLogId { get; set; }
}