namespace APG.MessageQueue.Contracts.Transactions;

public class TransactionReported
{
    public TransactionReported(Guid transactionId)
    {
        TransactionId = transactionId;
    }

    public TransactionReported()
    {
        
    }

    public Guid TransactionId { get; set; }
}