using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.DomainHelper.Filters;

public class TransactionFilter
{
    public TransactionFilter()
    {
        
    }
    public TransactionFilter(string identifierValue, TransactionIdentifier transactionIdentifier)
    {
        IdentifierValue = identifierValue;
        TransactionIdentifier = transactionIdentifier;
    }

    public string IdentifierValue { get; set; }
    public TransactionIdentifier TransactionIdentifier { get; set; }
}