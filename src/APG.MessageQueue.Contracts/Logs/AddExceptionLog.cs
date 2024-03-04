namespace APG.MessageQueue.Contracts.Logs;

// Publisher: All Tiers
// Consumer:  APGLogs
public class AddExceptionLog
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public string Source { get; set; }
    public string StackTrace { get; set; }
    public string InnerException { get; set; }
    public DateTimeOffset? DateTime { get; set; }
    public string ExceptionType { get; set; }// apgExecution, fundamentals, etc.. should be: Exception Source and should be an enum
    public string ExceptionServiceSource { get; set; }

}


