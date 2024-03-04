using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.DomainHelper.Filters;

public class DigitalTransactionFilter
{
    public DigitalTransactionFilter()
    {
        
    }
    public DigitalTransactionFilter(string identifierValue, DigitalTransactionIdentifier transactionIdentifier)
    {
        IdentifierValue = identifierValue;
        TransactionIdentifier = transactionIdentifier;
    }

    public string IdentifierValue { get; set; }
    public DigitalTransactionIdentifier TransactionIdentifier { get; set; }
}