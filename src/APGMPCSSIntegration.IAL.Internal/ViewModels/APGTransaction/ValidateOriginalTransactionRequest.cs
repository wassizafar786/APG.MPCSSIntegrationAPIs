using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;

public class ValidateOriginalTransactionRequest
{
    public ValidateOriginalTransactionRequest(string identifierValue, TransactionIdentifier transactionIdentifier)
    {
        IdentifierValue = identifierValue;
        TransactionIdentifier = transactionIdentifier;
    }

    public string IdentifierValue { get; set; }
    public TransactionIdentifier TransactionIdentifier { get; set; }
}

public class OriginalTransactionDetails
{
    public long TransactionIdN { get; init; }
    public Guid TransactionId { get; init; }
    
    public long HostId { get; init; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
}