
using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel;

public class TransactionFilter
{
    public TransactionFilter(string identifierValue, TransactionIdentifier transactionIdentifier)
    {
        IdentifierValue = identifierValue;
        TransactionIdentifier = transactionIdentifier;
    }

    public string IdentifierValue { get; set; }
    public TransactionIdentifier TransactionIdentifier { get; set; }
}