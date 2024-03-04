using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel;

public class ResolvePaymentReturnTransactionTypeResponse
{
    public bool IsPaymentReturnEnabled { get; set; }

    public TransactionType? ResolvedTransactionType { get; set; }
}