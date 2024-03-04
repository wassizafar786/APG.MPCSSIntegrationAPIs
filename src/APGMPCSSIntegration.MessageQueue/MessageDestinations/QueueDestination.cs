namespace APG.MessageQueue.MessageDestinations;

public class MessageDestination : IMessageDestination
{
    public static MessageDestination ToQueue (string queue)
    {
        return new MessageDestination()
        {
            Queue = queue
        };
    }

    public static MessageDestination ToExchangeAndQueue (string exchange, string queue)
    {
        return new MessageDestination()
        {
            Exchange = exchange,
            Queue = queue
        };
    }
    
    private string Queue { get; init; }
    private string Exchange { get; init; }
        
    public string ToUri()
    {
        return string.IsNullOrWhiteSpace(Exchange) 
            ? $"queue:{Queue}" 
            : $"exchange:{Exchange}?bind=true&queue={Queue}";
    }
}

