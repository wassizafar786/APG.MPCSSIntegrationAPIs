using System.Threading.Tasks;
using APGDigitalIntegration.IAL.Internal.Viewmodel;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGMPCSSIntegration.IAL.Internal.Communicator;

namespace APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction
{
    public interface ITransactionApiService
    {
        Task<OriginalTransactionDetails> ValidateOriginalTransaction(ValidateOriginalTransactionRequest validateOriginalTransactionRequest, CancellationToken cancellationToken = default);
        Task<TransactionTypeCacheModel> GetTransactionType(int transactionTypeId);
        Task<TransactionViewModel> GetTransactionById(Guid id);
        Task<ResolvePaymentReturnTransactionTypeResponse> ResolvePaymentReturnTransactionType(TransactionFilter transactionId);

    }
}