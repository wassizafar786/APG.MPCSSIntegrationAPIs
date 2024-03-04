namespace APG.MessageQueue.Contracts.SystemCommunicationLog;

// Publisher: ?
// Consumer:  APGLogs
public class AddSystemCommunicationLog
{
    public Guid Id { get; set; }
    public string CorrelationId { get; set; }
    public string ServiceName { get; set; }
    public string Request { get; set; }
    public int SourceId { get; set; }
    public string Response { get; set; }
    public DateTime? RequestTime { get; set; }
    public DateTime? ResponseTime { get; set; }
    public bool IsChild { get; set; }

}