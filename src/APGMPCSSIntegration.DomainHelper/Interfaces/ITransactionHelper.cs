using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.DomainHelper.Interfaces
{
    public interface ITransactionHelper
    {
        TransactionType GetTransactionType(MPCSSRecordRequest messageTypeId);

        //TransactionType GetTransactionType(string processingCode);
    }
}
