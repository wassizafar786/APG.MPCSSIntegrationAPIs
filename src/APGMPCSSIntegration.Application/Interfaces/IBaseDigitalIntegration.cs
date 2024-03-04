using System;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using System.Threading.Tasks;
using APG.MessageQueue.Contracts.Transactions;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;

namespace APGDigitalIntegration.Application.Interfaces
{
    public interface IBaseDigitalIntegration
    {
        public Task<string> GetPaymentGateway();
        public Task<BaseResponse<object>> CheckShadowLimitRequest<T>(T request, TransactionType transactionType);
        public Task AddTransactionMessage<T>(T request, TransactionType transactionType, string transactionIdentifier, string transactionStatus, ServiceResponse response);
    }
}
