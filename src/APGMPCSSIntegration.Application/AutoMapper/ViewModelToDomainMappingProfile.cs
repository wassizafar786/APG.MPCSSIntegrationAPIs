using AutoMapper;
using APGDigitalIntegration.Domain.Commands;
using APGMPCSSIntegration.Application.ViewModels;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using System;
using APG.MessageQueue.Contracts.Digital_Transactions;
using APG.MessageQueue.Contracts.MerchantMPCSSOperations;
using APG.MessageQueue.Contracts.Transactions;
using APGDigitalIntegration.Application.WriteModels;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;
using APGDigitalIntegration.IAL.Internal.Services.APGFundamental;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.Application.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<CustomerViewModel, RegisterNewCustomerCommand>()
                .ConstructUsing(c => new RegisterNewCustomerCommand(c.Name, c.Email, c.BirthDate));
            CreateMap<CustomerViewModel, UpdateCustomerCommand>()
                .ConstructUsing(c => new UpdateCustomerCommand(c.Id, c.Name, c.Email, c.BirthDate));

            CreateMap<CreditDebitPaymentInputDto, DigitalTransactionViewModel>()
                .ConstructUsing(c => new DigitalTransactionViewModel
                {
                    Id = Guid.NewGuid(),
                    Amount = Convert.ToDecimal(c.InterBankSettlementAmount),
                    MerchantRefId = c.MerchantRefId,
                    TerminalNodeId = c.TerminalNodeId,
                    BankId = c.BankId
                });

            CreateMap<CreditTransferResponseDto, DigitalTransactionViewModel>()
                .ConstructUsing(c => new DigitalTransactionViewModel
                {
                    Id = Guid.NewGuid(),
                    Amount = Convert.ToDecimal(c.GrpHdr.TotalInterbankSettlementAmount),
                    MerchantRefId = c.MerchantRefId,
                    TerminalNodeId = c.TerminalNodeId,
                    BankId = c.BankId
                });

            CreateMap<DirectDebitTransferResponseDto, DigitalTransactionViewModel>()
                .ConstructUsing(c => new DigitalTransactionViewModel
                {
                    Id = Guid.NewGuid(),
                    Amount = Convert.ToDecimal(c.GrpHdr.TotalInterbankSettlementAmount),
                    MerchantRefId = c.MerchantRefId,
                    TerminalNodeId = c.TerminalNodeId,
                    BankId = c.BankId
                });

            CreateMap<CreditDebitPaymentInputDto, CheckShadowBalanceLimitReq>()
                .ConstructUsing(c => new CheckShadowBalanceLimitReq
                {
                    MerchantRefId = c.MerchantRefId,
                    TerminalNodeId = c.TerminalNodeId,
                    BankId = c.BankId,
                    Amount = Convert.ToDecimal(c.TotalInterbankSettlementAmount)

                });

            CreateMap<CreditTransferResponseDto, CheckShadowBalanceLimitReq>()
                .ConstructUsing(c => new CheckShadowBalanceLimitReq
                {
                    MerchantRefId = c.MerchantRefId,
                    TerminalNodeId = c.TerminalNodeId,
                    BankId = c.BankId,
                    Amount = Convert.ToDecimal(c.GrpHdr.TotalInterbankSettlementAmount)
                });

            CreateMap<DirectDebitTransferResponseDto, CheckShadowBalanceLimitReq>()
                .ConstructUsing(c => new CheckShadowBalanceLimitReq { });


            CreateMap<ReturnRequestInputDto, CheckShadowBalanceLimitReq>()
                .ConstructUsing(c => new CheckShadowBalanceLimitReq
                {
                    MerchantRefId = c.MerchantRefId,
                    TerminalNodeId = c.TerminalNodeId,
                    BankId = c.BankId,
                    Amount = Convert.ToDecimal(c.TotalInterbankSettlementAmount),
                    OriginalTransactionIdentifierValue = c.OriginalTransactionId,
                    OriginalTransactionIdentifierType =  TransactionIdentifier.TransactionId,
                });

            CreateMap<ReturnRequestInputDto, DigitalTransactionViewModel>()
                .ConstructUsing(c => new DigitalTransactionViewModel
                {
                    Id = Guid.NewGuid(),
                    Amount = Convert.ToDecimal(c.TotalInterbankSettlementAmount),
                    MerchantRefId = c.MerchantRefId,
                    TerminalNodeId = c.TerminalNodeId,
                    BankId = c.BankId

                });

            CreateMap<DigitalPaymentEnquiry, PaymentEnquiryRequest>().ReverseMap();
            CreateMap<DigitalPaymentRefund, RefundPaymentRequest>().ReverseMap();
            
            CreateMap<AddTransaction, TransactionViewModel>().ReverseMap(); 
            
            CreateMap<DigitalTransactionWriteModel, DigitalTransaction>().ReverseMap(); 
            CreateMap<DigitalTransaction, DigitalTransactionViewModel>().ReverseMap(); 
        }

    }
}
