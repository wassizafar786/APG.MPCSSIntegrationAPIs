namespace APG.MessageQueue.Contracts.CommunicationLog;


// Publisher: APGExecutions
// Consumer:  APGLogs
public class AddCommunicationLog
{
    public Guid Id { get; set; }
    
    public string ServiceName { get; set; }

    public object InternalRequest { get; set; }
        
    public object InternalResponse { get; set; }
        
    public object ExternalRequest { get; set; }
        
    public object ExternalResponse { get; set; }
        
    public DateTimeOffset? RequestDatetime { get; set; }
        
    public DateTimeOffset? InternalRequestTime { get; set; }
        
    public DateTimeOffset? InternalResponseTime { get; set; }

    public DateTimeOffset? ExternalRequestTime { get; set; }
        
    public DateTimeOffset? ExternalResponseTime { get; set; }
        
    public int? MessageTypeId { get; set; }

    public int? MessageFormatId { get; set; }

    public long? MerchantRefId { get; set; }

    public long? TerminalNodeId { get; set; }
        
    public string ExceptionLogId { get; set; }
}
