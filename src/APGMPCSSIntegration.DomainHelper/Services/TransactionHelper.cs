
using APGDigitalIntegration.DomainHelper.Interfaces;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using Microsoft.Extensions.Localization;

namespace APGDigitalIntegration.DomainHelper.Services
{
    public class TransactionHelper : ITransactionHelper
    {
        private readonly IStringLocalizer<SharedResource> _stringLocalizer;

        public TransactionHelper(IStringLocalizer<SharedResource> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        public TransactionType GetTransactionType(MPCSSRecordRequest messageTypeId)
        {
            return (messageTypeId) switch
            {
                (MPCSSRecordRequest.PaymentOutwardDebitRequest)  => TransactionType.P2BPull,
                (MPCSSRecordRequest.PaymentOutwardCreditRequest) => TransactionType.B2BSend,
                (MPCSSRecordRequest.PaymentInwardDebitRequest) => TransactionType.B2BReceive,
                (MPCSSRecordRequest.PaymentInwardCreditRequest) => TransactionType.P2BPush,
                (MPCSSRecordRequest.PaymentReturnRequest) => TransactionType.P2BRefund,                
                _ => throw new BusinessException(_stringLocalizer[ResponseMessages.InvalidTransactionType], ResponseCodes.InvalidTransactionType)
            
            };
        }


        //public TransactionType GetTransactionType(string processingCode)
        //{
        //    return processingCode switch
        //    {
        //        ProcessingCodes.P2BPull => TransactionType.P2BPull,
        //        ProcessingCodes.P2BPush => TransactionType.P2BPush,
        //        ProcessingCodes.P2BRefund => TransactionType.P2BRefund,
                
        //        _ => throw new ArgumentOutOfRangeException(nameof(processingCode), processingCode, null)
        //    };
        //}

    }
}
