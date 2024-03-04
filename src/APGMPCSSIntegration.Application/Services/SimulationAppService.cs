using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.IAL.Internal.Viewmodel.QR;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;
using System;
using System.Threading;
using System.Threading.Tasks;
using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APG.MessageQueue.Mpcss.Interfaces;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonViewModels.Common;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New.PaymentMesssages;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New.SupplementaryData;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.DomainHelper.ExtensionMethods;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;
using APGDigitalIntegration.IAL.Internal.Services.APGFundamental;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;

namespace APGDigitalIntegration.Application.Services
{
    public class SimulationAppService : ISimulationAppService
    {
        #region Fields

        private readonly IMpcssCommunicator _mpcssCommunicator;
        private readonly ICommonTransactionalAppService _commonTransactionalAppService;
        private readonly IMerchantOrderApiService _merchantOrderAppService;
        private readonly IQRCodeAppService _qrCodeAppService;
        private readonly ResponseMessageHandler _messageHandler = null;
        private readonly ICurrencyApiService _currencyApiService;
        private readonly IMPCSSMessageBuilder _mpcssMessageBuilder;
        private readonly IDateTimeProvider _dateTimeProvider;

        #endregion

        #region Constructor
        public SimulationAppService(IMpcssCommunicator mpcssCommunicator,
            ICommonTransactionalAppService commonTransactionalAppService,
            IMerchantOrderApiService merchantOrderAppService,
            IQRCodeAppService qrCodeAppService,
            ICurrencyApiService currencyApiService,
            IMPCSSMessageBuilder mpcssMessageBuilder,
            IDateTimeProvider dateTimeProvider)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _commonTransactionalAppService = commonTransactionalAppService;
            _merchantOrderAppService = merchantOrderAppService;
            _qrCodeAppService = qrCodeAppService;
            _messageHandler = new ResponseMessageHandler();
            _currencyApiService = currencyApiService;
            _mpcssMessageBuilder = mpcssMessageBuilder;
            _dateTimeProvider = dateTimeProvider;
        }

        #endregion


        public async Task<ServiceResponse> InwardCreditTransaction(InwardCreditSimulationRequest inwardCreditSimulationRequest)
        {
            var qrMessage = await _qrCodeAppService.ParseQR_ISO2006(inwardCreditSimulationRequest.ISOMessage);

            if (qrMessage != null)
            {
                var request = ConstructCreditPaymentInputDto(qrMessage, inwardCreditSimulationRequest.Amount, inwardCreditSimulationRequest.Currency);
                // request.MessageIdentificationCode = await _merchantMPCSSTransactionRequestApiService.Add(new MerchantRequestOrderDTO
                // {
                //     QROrderId = request.MerchantOrderId, // change to order key later.
                //     UniqueNotificationId = _currentUserService.GetCurrentUserSessionId(),
                //     Status = MPCSSStatus.Initiated.GetEnumDescription(),
                //     TransactionType = MPCSSRecordRequest.QRPaymentRequest.GetEnumDescription()
                // });
                var mqMessage = await this.ConstructCreditTransactionXML(request); /****/

                Task.Run(() =>
                {
                    _mpcssCommunicator.SendMessage(mqMessage, MPCSSQueues.InwardPaymentQueue, ActiveMQMessageTypes.Text).ConfigureAwait(false);
                });


            }

            return new ServiceResponse(
            success: true,
            responseCode: ResponseCodes.Success,
            message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
        }

        public async Task<ServiceResponse> InwardCreditTransaction(QROrderSimulationRequest orderSimulationRequest)
        {
            var walletOrder = await _merchantOrderAppService.GetWalletOrderById(orderSimulationRequest.WallerOrderId);
         //   walletOrder.Amount = walletOrder.Amount * 1000;
            var instructingBICFI = string.IsNullOrWhiteSpace(orderSimulationRequest.InstructingBICFI)
                ? "HyperBICFI"
                : orderSimulationRequest.InstructingBICFI;

            var model = new CreditDebitPaymentInputDto
            {
                MessageIdentificationCode = "AMPL07092020002",
                NumberOfTransactions = "1",
                TotalInterbankSettlementAmount = walletOrder.Amount,
                InterbankSettlementDate = DateTime.UtcNow,
                CategoryPurposeProprietary = "1",
                BICFI = instructingBICFI,
                EndToEndId = "AMPL07092020002",
                TrxnId = "AMPL07092020002",
                CurrencyId = walletOrder.Currency,
                InterBankSettlementAmount = walletOrder.Amount,
                SenderName = "azeem",
                SenderId = "AMPL001M0096895062929",
                ReceiverName = "azeem",
                ReceiverId = "XXXXXXXM0096892222222",
                Issuer = "XXXXXXXM0096892222222",
                SchemaProprietary = "string",
                InstructingAgentBICFI = instructingBICFI,
                SenderIdentification = "string",
                ReceiverIdentification = "string",
                SessionSequence = "20",
                BatchSource = "string",
                // model.MerchantCategoryCode = qrMessage.MerchantCategoryCode;
                GroupMerchantId = walletOrder.MerchantId.ToString(),
                // model.Filler = qrMessage.Filler; 
                // model.MerchantName = qrMessage.MerchantName;
                // model.PointOfInitiationMethod = qrMessage.InitiationMethod;
                // model.TipOrConvnceIndicatorId = qrMessage.TipOrConvenienceIndicator;
                // model.FeePercentage = qrMessage.ConvenienceFeePercentage;
                // model.CountryCd = qrMessage.CountryCode;
                // model.MerchantCity = qrMessage.MerchantCity;
                // model.PostCode = qrMessage.PostalCode;
                InvoiceNumber = walletOrder.IdN.ToString(),
                ConsumerID = walletOrder.SessionId,
                TerminalId = walletOrder.TerminalId
            };

            var mqMessage = await this.ConstructCreditTransactionXML(model);

            Task.Run(() =>
            {
                Thread.Sleep(2000);
                _mpcssCommunicator.SendMessage(mqMessage, MPCSSQueues.InwardPaymentQueue, ActiveMQMessageTypes.Text).ConfigureAwait(false);
            });

            return new ServiceResponse(
                success: true,
                responseCode: ResponseCodes.Success,
                message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
        }


        public async Task<Envelope> ConstructCreditTransactionXML(CreditDebitPaymentInputDto request)
        {
            var currencyCode = await _currencyApiService.GetCurrencyShortNameByCurrencyId(request.CurrencyId);
            var settlementDate = request.InterbankSettlementDate.ToString("yyyy-MM-dd");
            var date = await _dateTimeProvider.SystemNow();

            var CustomerCreditRequest = new MPCSSPaymentCreditRequest();
            var GrpHdr = new GroupHeader();
            GrpHdr.MsgId = request.MessageIdentificationCode;
            GrpHdr.CreatedDateTime = date.ToISODateTime();
            GrpHdr.NumberOfTranasctions = request.NumberOfTransactions;
            GrpHdr.TotalInterbankSettlementAmount = new TotalInterBankSettlementAmount()
            {
                Value = request.TotalInterbankSettlementAmount.ToString(),
                Currency = currencyCode
            };
            GrpHdr.InterbankSettlementDate = settlementDate;
            var SttlmInf = new SettlementInformation();
            SttlmInf.SettlementMethod = "CLRG";
            var ClrSys = new ClearingSystem();
            ClrSys.Proprietary = "CBO";
            SttlmInf.ClearingSystem = ClrSys;
            GrpHdr.SettlementInformation = SttlmInf;
            var PmtTpInf = new PaymentTypeInformation();
            var LclInstrm = new LocalInstrument();
            LclInstrm.Code = "TEL";
            PmtTpInf.LocalInstrument = LclInstrm;
            var CtgyPurp = new CategoryPurpose();
            CtgyPurp.Proprietary = request.CategoryPurposeProprietary;
            PmtTpInf.CategoryPurpose = CtgyPurp;
            GrpHdr.PaymentTypeInformation = PmtTpInf;

            InstructingAgent instructingAgent = null;
            if (request.InstructingAgentBICFI != null)
            {
                instructingAgent = new InstructingAgent();
                var InstructingFinInstnId = new FinancialInstitutionIdentification();
                InstructingFinInstnId.BICFI = request.InstructingAgentBICFI;
                instructingAgent.FinancialInstitutionIdentification = InstructingFinInstnId;
            }
            GrpHdr.InstructingAgent = instructingAgent;
            var InstdAgt = new InstructedAgent();
            var InstructedFinInstnId = new FinancialInstitutionIdentification();
            InstructedFinInstnId.BICFI = "CBOMOMRUMPC";
            InstdAgt.FinancialInstitutionIdentification = InstructedFinInstnId;
            GrpHdr.InstructedAgent = InstdAgt;

            //Credit Transfer Data

            var CdtTrfTxInf = new CreditTransferTransactionInformation();
            var PmtId = new PaymentIdentification();
            PmtId.EndToEndId = request.EndToEndId;
            PmtId.TransactionId = request.TrxnId;
            CdtTrfTxInf.PaymentIdentification = PmtId;
            CdtTrfTxInf.InterbankSettlementAmount = new InterBankSettlementAmount()
            {
                Value = request.InterBankSettlementAmount.ToString(),
                Currency = currencyCode
            };
            CdtTrfTxInf.ChargeBearer = "SLEV";

            var Dbtr = new Debtor();
            Dbtr.Nm = request.SenderName;
            Identification DbtrId = null;
            if (request.SchemaProprietary != null || request.Issuer != null)
            {
                DbtrId = new Identification();
                var PrvtId = new PrivateIdentification();
                var DbtrOthrId = new OtherIdentification();
                DbtrOthrId.Id = request.SenderId;
                SchemeName SchmeNm = null;
                if (request.SchemaProprietary != null)
                {
                    SchmeNm = new SchemeName();
                    SchmeNm.Proprietary = request.SchemaProprietary;
                }
                DbtrOthrId.SchemeName = SchmeNm;
                DbtrOthrId.Issuer = request.Issuer;
                PrvtId.Other = DbtrOthrId;
                DbtrId.PrvtId = PrvtId;
            }
            Dbtr.Id = DbtrId;
            CdtTrfTxInf.Debtor = Dbtr;

            var DbtrAcct = new DebtorAccount();
            var DbtrAccId = new Id();
            var DbtrOthr = new Other();
            DbtrOthr.Id = request.SenderIdentification;
            DbtrAccId.Other = DbtrOthr;
            DbtrAcct.Id = DbtrAccId;
            CdtTrfTxInf.DebtorAccount = DbtrAcct;

            DebtorAgent DbtrAgt = null;
            if (request.BICFI != null)
            {
                DbtrAgt = new DebtorAgent();
                var DbtrFinInstnId = new FinancialInstitutionIdentification();
                DbtrFinInstnId.BICFI = request.BICFI;
                DbtrAgt.FinancialInstitutionIdentification = DbtrFinInstnId;
            }
            CdtTrfTxInf.DebtorAgent = DbtrAgt;

            var CdtrAgt = new CreditorAgent();
            var FinInstnId = new FinancialInstitutionIdentification();
            FinInstnId.BICFI = "CBOMOMRUMPC";
            CdtrAgt.FinancialInstitutionIdentification = FinInstnId;
            CdtTrfTxInf.CreditorAgent = CdtrAgt;

            var Cdtr = new Creditor();
            Cdtr.Nm = request.ReceiverName;
            Identification CdtrId = null;
            if (request.SchemaProprietary != null || request.Issuer != null)
            {
                CdtrId = new Identification();
                var PrvtId = new PrivateIdentification();
                var CdtrOthrId = new OtherIdentification();
                CdtrOthrId.Id = request.ReceiverId;
                SchemeName SchmeNm = null;
                if (request.SchemaProprietary != null)
                {
                    SchmeNm = new SchemeName();
                    SchmeNm.Proprietary = request.SchemaProprietary;
                }
                CdtrOthrId.SchemeName = SchmeNm;
                CdtrOthrId.Issuer = request.Issuer;
                PrvtId.Other = CdtrOthrId;
                CdtrId.PrvtId = PrvtId;
            }
            Cdtr.Id = CdtrId;
            CdtTrfTxInf.Creditor = Cdtr;


            var CdtrAcct = new CreditorAccount();
            var Id = new Id();
            var Othr = new Other();
            Othr.Id = request.ReceiverIdentification;
            Id.Other = Othr;
            CdtrAcct.Id = Id;
            CdtTrfTxInf.CreditorAccount = CdtrAcct;

            var SplmtryData = new SupplementaryData();
            SplmtryData.PlcAndNm = "ACHSupplementaryData";
            var Envlp = new Envlp();
            var achSupplementaryData = new AchSupplementaryData();
            achSupplementaryData.SessionSequence = request.SessionSequence;
            achSupplementaryData.BatchSource = request.BatchSource;
            achSupplementaryData.ConsumerId = request.ConsumerID;
            achSupplementaryData.CountryCd = request.CountryCd;
            achSupplementaryData.FeePercentage = request.FeePercentage;
            achSupplementaryData.Filler = request.Filler;
            achSupplementaryData.GroupMerchantId = request.GroupMerchantId;
            achSupplementaryData.InvoiceNumber = request.InvoiceNumber;
            achSupplementaryData.TerminalId = request.TerminalId.ToString();
            achSupplementaryData.MerchantCity = request.MerchantCity;
            achSupplementaryData.MerchantName = request.MerchantName;
            achSupplementaryData.MerchCategoryCd = request.MerchantCategoryCode;
            achSupplementaryData.MsgtipOrConvnceIndctrId = request.TipOrConvnceIndicatorId;
            achSupplementaryData.PointOfInitiateMethod = request.PointOfInitiationMethod;
            achSupplementaryData.PostCd = request.PostCode;
            Envlp.achSupplementaryData = achSupplementaryData;
            SplmtryData.Envlp = Envlp;
            CustomerCreditRequest.GroupHeader = GrpHdr;
            CustomerCreditRequest.CreditTransferTransactionInformation = CdtTrfTxInf;
            CustomerCreditRequest.SupplementaryData = SplmtryData;

            var paymentRequest = new MPCSSPaymentCreditRequestRoot
            {
                MPCSSPaymentCreditRequest = CustomerCreditRequest
            };

            return _mpcssMessageBuilder.ConvertToExternalRequest(paymentRequest, paymentRequest.MPCSSPaymentCreditRequest.GroupHeader.CreatedDateTime,
                CustomerCreditRequest.GroupHeader.MsgId, MPCSSMessageTypes.CREDIT_MESSAGE_TYPE, true);
        }


        public async Task<ServiceResponse> InwardDebitTransaction(CreditDebitPaymentInputDto request)
        {
            var mqMessage = await _commonTransactionalAppService.ConstructDebitTransactionXML(request);

            Task.Run(() =>
            {
                _mpcssCommunicator.SendMessage(mqMessage, MPCSSQueues.InwardPaymentQueue, ActiveMQMessageTypes.Text).ConfigureAwait(false);
            });

            return new ServiceResponse(
            success: true,
            responseCode: ResponseCodes.Success,
            message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
        }


        public async Task<ServiceResponse> InwardPaymentEnquiry(PaymentEnquiryInputDto request)
        {
            var mqMessage = _commonTransactionalAppService.ConstructPaymentEnquiryXML(request);

            await _mpcssCommunicator.SendMessage(mqMessage, MPCSSQueues.PaymentEnquiryResponseQueue, ActiveMQMessageTypes.Text).ConfigureAwait(false);

            return new ServiceResponse(
            success: true,
            responseCode: ResponseCodes.Success,
            message: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));
        }

        #region Private Methods
        private CreditDebitPaymentInputDto ConstructCreditPaymentInputDto(QR_MessageRequestVM qrMessage, decimal amount, int currency)
        {
            CreditDebitPaymentInputDto model = new CreditDebitPaymentInputDto();
            model.MessageIdentificationCode = "AMPL07092020002";
            model.NumberOfTransactions = "1";
            model.TotalInterbankSettlementAmount = qrMessage.TransactionAmount > 0 ? qrMessage.TransactionAmount : amount;
            model.InterbankSettlementDate = DateTime.UtcNow;
            model.CategoryPurposeProprietary = "1";
            model.BICFI = qrMessage.BICCode;
            model.EndToEndId = "AMPL07092020002";
            model.TrxnId = "AMPL07092020002";
            model.CurrencyId = qrMessage.TransactionCurrencyCode != null ? int.Parse(qrMessage.TransactionCurrencyCode) : currency;
            model.InterBankSettlementAmount = qrMessage.TransactionAmount > 0 ? qrMessage.TransactionAmount : amount;
            model.SenderName = "azeem";
            model.SenderId = "AMPL001M0096895062929";
            model.ReceiverName = "azeem";
            model.ReceiverId = "XXXXXXXM0096892222222";
            model.Issuer = "XXXXXXXM0096892222222";
            model.SchemaProprietary = "string";
            model.InstructingAgentBICFI = "string";
            model.SenderIdentification = "string";
            model.ReceiverIdentification = "string";
            model.SessionSequence = "string";
            model.BatchSource = "string";
            model.ConsumerID = "string";
            model.MerchantCategoryCode = qrMessage.MerchantCategoryCode;
            model.GroupMerchantId = qrMessage.MerchantIdentifier;
            model.Filler = qrMessage.Filler;
            model.MerchantName = qrMessage.MerchantName;
            model.PointOfInitiationMethod = qrMessage.InitiationMethod;
            model.TipOrConvnceIndicatorId = qrMessage.TipOrConvenienceIndicator;
            model.FeePercentage = qrMessage.ConvenienceFeePercentage;
            model.CountryCd = qrMessage.CountryCode;
            model.MerchantCity = qrMessage.MerchantCity;
            model.PostCode = qrMessage.PostalCode;
            model.InvoiceNumber = qrMessage.InvoiceNo;
            model.ConsumerID = qrMessage.ConsumerId;

            if (long.TryParse(qrMessage.TerminalId, out long terminalId))
            {
                model.TerminalId = terminalId;
            }

            model.GroupMerchantId = qrMessage.GroupMerchId;

            return model;
        }

        #endregion

    }
}
