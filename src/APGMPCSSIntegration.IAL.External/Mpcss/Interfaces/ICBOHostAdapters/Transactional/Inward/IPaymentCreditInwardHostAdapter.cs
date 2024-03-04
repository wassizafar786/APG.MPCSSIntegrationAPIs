using APGDigitalIntegration.Common.CommonViewModels.Request;

namespace APGDigitalIntegration.IAL.External.Mpcss.Interfaces.ICBOHostAdapters.Transactional.Inward
{ 
    public interface IPaymentCreditInwardHostAdapter
    {
        Task<BaseInternalResponse> Execute(CreditPaymentInternalRequest internalCreditPaymentInternalRequest, TransactionStatus transactionStatus, CancellationToken cancellationToken);
    }
}
