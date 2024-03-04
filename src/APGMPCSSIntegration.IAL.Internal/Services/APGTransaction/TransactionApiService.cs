using APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using System.Collections.Generic;
using System.Threading.Tasks;
using APGDigitalIntegration.Cache.Interfaces;
using APGDigitalIntegration.IAL.Internal.Communicator;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGMPCSSIntegration.IAL.Internal.CacheHelper;
using APGMPCSSIntegration.IAL.Internal.Viewmodel.APGFundamentals;
using APGDigitalIntegration.IAL.Internal.Viewmodel;

namespace APGDigitalIntegration.IAL.Internal.Services.APGTransaction
{
    public class TransactionApiService : ITransactionApiService
    {
        private readonly IApiCaller _apiCaller;
        private readonly ICacheService _cacheService;

        public TransactionApiService(IApiCaller apiCaller, ICacheService cacheService)
        {
            _apiCaller = apiCaller;
            _cacheService = cacheService;
        }
        
        public async Task<OriginalTransactionDetails> ValidateOriginalTransaction(ValidateOriginalTransactionRequest validateOriginalTransactionRequest, CancellationToken cancellationToken)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {nameof(validateOriginalTransactionRequest.IdentifierValue), validateOriginalTransactionRequest.IdentifierValue},
                {nameof(validateOriginalTransactionRequest.TransactionIdentifier), validateOriginalTransactionRequest.TransactionIdentifier.ToString()},
              };
            
            var originalTransaction =  await _apiCaller.GetAsJson<OriginalTransactionDetails>(
                    MicroServicesURL.BaseTransactionsURL,
                    ControllerNames.Transactions.Transaction,
                    ServiceNameCommon.ValidateOriginalTransaction,
                    queryParams, cancellationToken)
                .ConfigureAwait(false);
            
            if (originalTransaction.Success == false || originalTransaction.Data is null)
                throw new BusinessException(originalTransaction.ErrorList, originalTransaction.ResponseCode);

            return originalTransaction.Data;
        }

        public async Task<TransactionViewModel> GetTransactionById(Guid id)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {"Id", id.ToString()},
            };
            
            var response =  await _apiCaller.GetAsJson<TransactionViewModel>(
                    MicroServicesURL.BaseTransactionsURL,
                    ControllerNames.Transactions.Transaction,
                    ServiceNameCommon.GetTransactionById,
                    queryParams)
                .ConfigureAwait(false);

            if(response.Success == false)
                throw new BusinessException(response.ErrorList, response.ResponseCode);

            return response.Data;
        }


        public async Task<TransactionTypeCacheModel> GetTransactionType(int transactionTypeId)
        {
            var cacheKey = CacheKeysHelper.GetKey(new TransactionTypeCacheKeyModel(transactionTypeId));
            var transactionTypeCacheModel = await _cacheService.GetValueByKeyAsync<TransactionTypeCacheModel>(cacheKey).ConfigureAwait(false);

            if (transactionTypeCacheModel is not null)
                return transactionTypeCacheModel;
            
            var queryParams = new Dictionary<string, string>()
            {
                {"Id", transactionTypeId.ToString()},
            };
            
            var transactionTypeCacheModelResponse =  await _apiCaller.GetAsJson<TransactionTypeCacheModel>(
                    MicroServicesURL.BaseTransactionsURL,
                    ControllerNames.Transactions.Lookup,
                    ServiceNameCommon.GetTransactionTypeById,
                    queryParams)
                .ConfigureAwait(false);

            return transactionTypeCacheModelResponse.Data;
        }

        public async Task<ResolvePaymentReturnTransactionTypeResponse> ResolvePaymentReturnTransactionType(TransactionFilter transactionFilter)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {nameof(transactionFilter.IdentifierValue), transactionFilter.IdentifierValue},
                {nameof(transactionFilter.TransactionIdentifier), transactionFilter.TransactionIdentifier.ToString()}
            };
            var resolvedType = await _apiCaller.GetAsJson<ResolvePaymentReturnTransactionTypeResponse>(
                    MicroServicesURL.BaseTransactionsURL,
                    ControllerNames.Transactions.Transaction,
                    ServiceNameCommon.ResolvePaymentReturnTransactionType,
                    queryParams)
                .ConfigureAwait(false);

            if (resolvedType.Success == false || resolvedType.Data is null)
                throw new BusinessException(resolvedType.ErrorList, resolvedType.ResponseCode);

            return resolvedType.Data;
        }
    }
}