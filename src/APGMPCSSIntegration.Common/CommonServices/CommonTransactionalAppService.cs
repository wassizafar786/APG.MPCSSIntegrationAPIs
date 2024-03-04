using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Common.CommonViewModels.Payment;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonViewModels.Payment;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;
using APGDigitalIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.DomainHelper.ExtensionMethods;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;
using CategoryPurpose = APGMPCSSIntegration.Common.CommonViewModels.Payment.CategoryPurpose;
using ClearingSystem = APGMPCSSIntegration.Common.CommonViewModels.Payment.ClearingSystem;
using Creditor = APGMPCSSIntegration.Common.CommonViewModels.Payment.Creditor;
using CreditorAccount = APGMPCSSIntegration.Common.CommonViewModels.Payment.CreditorAccount;
using CreditorAgent = APGMPCSSIntegration.Common.CommonViewModels.Payment.CreditorAgent;
using Debtor = APGMPCSSIntegration.Common.CommonViewModels.Payment.Debtor;
using DebtorAccount = APGMPCSSIntegration.Common.CommonViewModels.Payment.DebtorAccount;
using DebtorAgent = APGMPCSSIntegration.Common.CommonViewModels.Payment.DebtorAgent;
using FinancialInstitutionIdentification = APGMPCSSIntegration.Common.CommonViewModels.Payment.FinancialInstitutionIdentification;
using Identification = APGMPCSSIntegration.Common.CommonViewModels.Payment.Identification;
using InstructedAgent = APGMPCSSIntegration.Common.CommonViewModels.Payment.InstructedAgent;
using InstructingAgent = APGMPCSSIntegration.Common.CommonViewModels.Payment.InstructingAgent;
using LocalInstrument = APGMPCSSIntegration.Common.CommonViewModels.Payment.LocalInstrument;
using OtherIdentification = APGMPCSSIntegration.Common.CommonViewModels.Payment.OtherIdentification;
using PaymentIdentification = APGMPCSSIntegration.Common.CommonViewModels.Payment.PaymentIdentification;
using PaymentTypeInformation = APGMPCSSIntegration.Common.CommonViewModels.Payment.PaymentTypeInformation;
using PrivateIdentification = APGMPCSSIntegration.Common.CommonViewModels.Payment.PrivateIdentification;
using SchemeName = APGMPCSSIntegration.Common.CommonViewModels.Payment.SchemeName;
using SettlementInformation = APGMPCSSIntegration.Common.CommonViewModels.Payment.SettlementInformation;

namespace APGDigitalIntegration.Common.CommonServices
{
    public class CommonTransactionalAppService : ICommonTransactionalAppService
    {
        private readonly ICurrencyApiService _currencyApiService;

        public CommonTransactionalAppService(ICurrencyApiService currencyApiService)
        {
            _currencyApiService = currencyApiService;
        }
        
        public async Task<MqMessage> ConstructCreditTransactionXML(CreditDebitPaymentInputDto request)
        {
            var currencyCode = await _currencyApiService.GetCurrencyShortNameByCurrencyId(request.CurrencyId);
            string settlementDate = request.InterbankSettlementDate.ToString("yyyy-MM-dd");
            string date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

            string xmlRequest = string.Empty;

            CustomerCreditTransferDto CustomerCreditRequest = new CustomerCreditTransferDto();
            GroupHeaderDto GrpHdr = new GroupHeaderDto();
            GrpHdr.MsgId = request.MessageIdentificationCode;
            GrpHdr.CreDtTm = date;
            GrpHdr.NbOfTxs = request.NumberOfTransactions;
            GrpHdr.TtlIntrBkSttlmAmt = new ActiveAmountAndCurrency()
            {
                Amount = request.TotalInterbankSettlementAmount,
                Currency = currencyCode
            };
            GrpHdr.IntrBkSttlmDt = settlementDate;
            SettlementInformation SttlmInf = new SettlementInformation();
            SttlmInf.SttlmMtd = "CLRG";
            ClearingSystem ClrSys = new ClearingSystem();
            ClrSys.Prtry = "CBO";
            SttlmInf.ClrSys = ClrSys;
            GrpHdr.SttlmInf = SttlmInf;
            PaymentTypeInformation PmtTpInf = new PaymentTypeInformation();
            LocalInstrument LclInstrm = new LocalInstrument();
            LclInstrm.Cd = "TEL";
            PmtTpInf.LclInstrm = LclInstrm;
            CategoryPurpose CtgyPurp = new CategoryPurpose();
            CtgyPurp.Prtry = request.CategoryPurposeProprietary;
            PmtTpInf.CtgyPurp = CtgyPurp;
            GrpHdr.PmtTpInf = PmtTpInf;

            InstructingAgent InstgAgt = null;
            if (request.InstructingAgentBICFI != null)
            {
                InstgAgt = new InstructingAgent();
                FinancialInstitutionIdentification InstructingFinInstnId = new FinancialInstitutionIdentification();
                InstructingFinInstnId.BICFI = request.InstructingAgentBICFI;
                InstgAgt.FinInstnId = InstructingFinInstnId;
            }
            GrpHdr.InstgAgt = InstgAgt;
            InstructedAgent InstdAgt = new InstructedAgent();
            FinancialInstitutionIdentification InstructedFinInstnId = new FinancialInstitutionIdentification();
            InstructedFinInstnId.BICFI = "CBOMOMRUMPC";
            InstdAgt.FinInstnId = InstructedFinInstnId;
            GrpHdr.InstdAgt = InstdAgt;

            //Credit Transfer Data

            CreditTransferTransactionDto CdtTrfTxInf = new CreditTransferTransactionDto();
            PaymentIdentification PmtId = new PaymentIdentification();
            PmtId.EndToEndId = request.EndToEndId;
            PmtId.TxId = request.TrxnId;
            CdtTrfTxInf.PmtId = PmtId;
            CdtTrfTxInf.IntrBkSttlmAmt = request.InterBankSettlementAmount.ToString();
            CdtTrfTxInf.ChrgBr = "SLEV";

            Debtor Dbtr = new Debtor();
            Dbtr.Nm = request.SenderName;
            Identification DbtrId = null;
            if (request.SchemaProprietary != null || request.Issuer != null)
            {
                DbtrId = new Identification();
                PrivateIdentification PrvtId = new PrivateIdentification();
                OtherIdentification DbtrOthrId = new OtherIdentification();
                DbtrOthrId.Id = request.SenderId;
                SchemeName SchmeNm = null;
                if (request.SchemaProprietary != null)
                {
                    SchmeNm = new SchemeName();
                    SchmeNm.Prtry = request.SchemaProprietary;
                }
                DbtrOthrId.SchmeNm = SchmeNm;
                DbtrOthrId.Issr = request.Issuer;
                PrvtId.Othr = DbtrOthrId;
                DbtrId.PrvtId = PrvtId;
            }
            Dbtr.Id = DbtrId;
            CdtTrfTxInf.Dbtr = Dbtr;

            DebtorAccount DbtrAcct = new DebtorAccount();
            Identification DbtrAccId = new Identification();
            OtherIdentification DbtrOthr = new OtherIdentification();
            DbtrOthr.Id = request.SenderIdentification;
            DbtrAccId.Othr = DbtrOthr;
            DbtrAcct.Id = DbtrAccId;
            CdtTrfTxInf.DbtrAcct = DbtrAcct;

            DebtorAgent DbtrAgt = null;
            if (request.BICFI != null)
            {
                DbtrAgt = new DebtorAgent();
                FinancialInstitutionIdentification DbtrFinInstnId = new FinancialInstitutionIdentification();
                DbtrFinInstnId.BICFI = request.BICFI;
                DbtrAgt.FinInstnId = DbtrFinInstnId;
            }
            CdtTrfTxInf.DbtrAgt = DbtrAgt;

            CreditorAgent CdtrAgt = new CreditorAgent();
            FinancialInstitutionIdentification FinInstnId = new FinancialInstitutionIdentification();
            FinInstnId.BICFI = "CBOMOMRUMPC";
            CdtrAgt.FinInstnId = FinInstnId;
            CdtTrfTxInf.CdtrAgt = CdtrAgt;

            Creditor Cdtr = new Creditor();
            Cdtr.Nm = request.ReceiverName;
            Identification CdtrId = null;
            if (request.SchemaProprietary != null || request.Issuer != null)
            {
                CdtrId = new Identification();
                PrivateIdentification PrvtId = new PrivateIdentification();
                OtherIdentification CdtrOthrId = new OtherIdentification();
                CdtrOthrId.Id = request.ReceiverId;
                SchemeName SchmeNm = null;
                if (request.SchemaProprietary != null)
                {
                    SchmeNm = new SchemeName();
                    SchmeNm.Prtry = request.SchemaProprietary;
                }
                CdtrOthrId.SchmeNm = SchmeNm;
                CdtrOthrId.Issr = request.Issuer;
                PrvtId.Othr = CdtrOthrId;
                CdtrId.PrvtId = PrvtId;
            }
            Cdtr.Id = CdtrId;
            CdtTrfTxInf.Cdtr = Cdtr;


            CreditorAccount CdtrAcct = new CreditorAccount();
            Identification Id = new Identification();
            OtherIdentification Othr = new OtherIdentification();
            Othr.Id = request.ReceiverIdentification;
            Id.Othr = Othr;
            CdtrAcct.Id = Id;
            CdtTrfTxInf.CdtrAcct = CdtrAcct;

            SupplementaryDataDto SplmtryData = new SupplementaryDataDto();
            SplmtryData.PlcAndNm = "ACHSupplementaryData";
            Envelope Envlp = new Envelope();
            ACHSupplementaryData achSupplementaryData = new ACHSupplementaryData();
            achSupplementaryData.sessionSequence = request.SessionSequence;
            achSupplementaryData.batchSource = request.BatchSource;
            achSupplementaryData.consumerID = request.ConsumerID;
            achSupplementaryData.countryCd = request.CountryCd;
            achSupplementaryData.feePercentage = request.FeePercentage;
            achSupplementaryData.filler = request.Filler;
            achSupplementaryData.grpMerchId = request.GroupMerchantId;
            achSupplementaryData.invoiceNumber = request.InvoiceNumber;
            achSupplementaryData.terminalId = request.TerminalId.ToString();
            achSupplementaryData.merchantCity = request.MerchantCity;
            achSupplementaryData.merchantName = request.MerchantName;
            achSupplementaryData.merchCategoryCd = request.MerchantCategoryCode;
            achSupplementaryData.MsgtipOrConvnceIndctrId = request.TipOrConvnceIndicatorId;
            achSupplementaryData.pntOfInitiateMethd = request.PointOfInitiationMethod;
            achSupplementaryData.postCd = request.PostCode;
            Envlp.achSupplementaryData = achSupplementaryData;
            SplmtryData.Envlp = Envlp;
            CustomerCreditRequest.GrpHdr = GrpHdr;
            CustomerCreditRequest.CdtTrfTxInf = CdtTrfTxInf;
            CustomerCreditRequest.SplmtryData = SplmtryData;

            var paymentRequest = new PaymentRequestDto
            {
                FIToFICstmrCdtTrf = CustomerCreditRequest
            };
            xmlRequest = MpcssMethods.ConvertRequestToXMLString(paymentRequest);

            xmlRequest = xmlRequest.Replace("<TtlIntrBkSttlmAmt>", "<TtlIntrBkSttlmAmt Ccy=\"OMR\">");
            xmlRequest = xmlRequest.Replace("<IntrBkSttlmAmt>", "<IntrBkSttlmAmt Ccy=\"OMR\">");
            xmlRequest = MpcssMethods.BuildAchNs2Data(xmlRequest); /* Needs to be checked */
            MqMessage message = MessageBuilder.ConstructMqMessage(xmlRequest, request.MessageIdentificationCode, MPCSSMessageTypes.CREDIT_MESSAGE_TYPE, date);

            return message;

        }

        public async Task<MqMessage> ConstructDebitTransactionXML(CreditDebitPaymentInputDto request)
        {
            var customerDirectDebitRequest = new CustomerDirectDebitTransferDto();
            var currencyShortName = await _currencyApiService.GetCurrencyShortNameByCurrencyId(request.CurrencyId);
            var grpHdr = new GroupHeaderDto
            {
                MsgId = request.MessageIdentificationCode,
                CreDtTm = DateTime.UtcNow.ToISODateTime(),
                NbOfTxs = "1",
                TtlIntrBkSttlmAmt = new ActiveAmountAndCurrency()
                {
                    Amount = request.TotalInterbankSettlementAmount,
                    Currency = currencyShortName
                    
                },
                IntrBkSttlmDt = DateTime.Today.ToISODate(),
                SttlmInf = new SettlementInformation
                {
                    SttlmMtd = MpcssMessageConstants.Settlement.SttlmMtd,
                    ClrSys = new ClearingSystem
                    {
                        Prtry = MpcssMessageConstants.Settlement.ClrSysPrtry
                    }
                },
                PmtTpInf = new PaymentTypeInformation()
                {
                    LclInstrm = new LocalInstrument
                    {
                        Cd = MpcssMessageConstants.PaymentTypeInformation.LclInstrm
                    },
                    CtgyPurp = new CategoryPurpose
                    {
                        Prtry = MpcssMessageConstants.CBO.CategoryPurpose // Move To Conf Parameter
                    }
                },
                InstgAgt = new InstructingAgent()
                {
                    FinInstnId = new FinancialInstitutionIdentification
                    {
                        BICFI = request.InstructingAgentBICFI
                    }
                },
                InstdAgt = new InstructedAgent
                {
                    FinInstnId = new FinancialInstitutionIdentification
                    {
                        BICFI = MpcssMessageConstants.CBO.BICFI
                    }
                }
            };
            
            //Debit Transfer Data
            var drctDbtTxInf = new CustomerDebitTransactionDto()
            {
                PmtId = new PaymentIdentification
                {
                    EndToEndId = request.EndToEndId,
                    TxId = request.TrxnId 
                },
                IntrBkSttlmAmt = new ActiveAmountAndCurrency()
                {
                    Amount = request.InterBankSettlementAmount,
                    Currency = currencyShortName
                    
                },
                ChrgBr = MpcssMessageConstants.Settlement.ChrgBr,
                
                Cdtr = new Creditor(),
                CdtrAcct = new CreditorAccount()
                {
                    Id = new Identification()
                    {
                        Othr = new OtherIdentification
                        {
                            Id = request.SenderIdentification 
                        }
                    }
                },
                CdtrAgt = new CreditorAgent()
                {
                    FinInstnId = new FinancialInstitutionIdentification()
                    {
                        BICFI = request.BICFI //TODO:ER9
                    }
                },
                
                Dbtr = new Debtor()
                {
                    // Nm = request.ReceiverName,
                    // Id = new Identification()
                    // {
                    //     PrvtId = new PrivateIdentification()
                    //     {
                    //         Othr = new OtherIdentification()
                    //         {
                    //             Id = request.ReceiverId,
                    //             SchmeNm = new SchemeName()
                    //             {
                    //                 Prtry = request.SchemaProprietary
                    //             },
                    //             Issr = request.Issuer
                    //         }
                    //     }
                    // }
                },
                
                DbtrAcct = new DebtorAccount()
                {
                    Id = new Identification()
                    {
                        Othr = new OtherIdentification()
                        {
                            Id = request.ReceiverIdentification // 
                        }
                    }
                },
                
                DbtrAgt = new DebtorAgent()
                {
                    FinInstnId = new FinancialInstitutionIdentification()
                    {
                        BICFI = MpcssMessageConstants.CBO.BICFI
                    }
                }
            };

            var splmtryData = new SupplementaryDataDto
            {
                PlcAndNm = MpcssMessageConstants.SupplementaryData.PlcAndName,
                Envlp = new Envelope
                {
                    achSupplementaryData = new ACHSupplementaryData //TODO: should i add all the options there??
                    {
                        sessionSequence = MpcssMessageConstants.SupplementaryData.SessionSequence,
                        batchSource = MpcssMessageConstants.SupplementaryData.BatchSource,
                        consumerID = request.ConsumerID,
                        countryCd = request.CountryCd,
                        feePercentage = request.FeePercentage,
                        filler = request.Filler,
                        grpMerchId = request.GroupMerchantId,
                        invoiceNumber = request.InvoiceNumber,
                        terminalId = request.TerminalId.ToString(),
                        merchantCity = request.MerchantCity,
                        merchantName = request.MerchantName,
                        merchCategoryCd = request.MerchantCategoryCode,
                        MsgtipOrConvnceIndctrId = request.TipOrConvnceIndicatorId,
                        pntOfInitiateMethd = request.PointOfInitiationMethod,
                        postCd = request.PostCode
                    }
                }
            };
            
            customerDirectDebitRequest.GrpHdr = grpHdr;
            customerDirectDebitRequest.DrctDbtTxInf = drctDbtTxInf;
            customerDirectDebitRequest.SplmtryData = splmtryData;

            var paymentRequestDto = new PaymentRequestDto
            {
                FIToFICstmrDrctDbt = customerDirectDebitRequest
            };
            var xmlRequest = MpcssMethods.ConvertRequestToXMLString(paymentRequestDto);

            xmlRequest = xmlRequest.Replace("<TtlIntrBkSttlmAmt>", "<TtlIntrBkSttlmAmt Ccy=\"OMR\">");
            xmlRequest = xmlRequest.Replace("<IntrBkSttlmAmt>", "<IntrBkSttlmAmt Ccy=\"OMR\">");
            xmlRequest = MpcssMethods.BuildAchNs2Data(xmlRequest);
            var message = MessageBuilder.ConstructMqMessage(xmlRequest, request.MessageIdentificationCode, MPCSSMessageTypes.DEBIT_MESSAGE_TYPE, grpHdr.CreDtTm);
            return message;

        }
        
        public MqMessage ConstructPaymentEnquiryXML(PaymentEnquiryInputDto request)
        {
            var date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
            var xmlRequest = string.Empty;
            var PaymentStatusRequest = new PaymentStatusRequestDto();
            var GrpHdr = new GroupHeaderDto();
            GrpHdr.MsgId = request.MessageIdentificationCode;
            GrpHdr.CreDtTm = date;
            InstructingAgent InstgAgt = null;
            if (request.InstructingAgentBICFI != null)
            {
                InstgAgt = new InstructingAgent();
                FinancialInstitutionIdentification InstructingFinInstnId = new FinancialInstitutionIdentification();
                InstructingFinInstnId.BICFI = request.InstructingAgentBICFI;
                InstgAgt.FinInstnId = InstructingFinInstnId;
            }
            GrpHdr.InstgAgt = InstgAgt;

            var OrgnlGrpInf = new OriginalGroupInformation();
            OrgnlGrpInf.OrgnlMsgId = request.OriginalMessageId;
            OrgnlGrpInf.OrgnlMsgNmId = request.OriginalMessageNameId;
            OrgnlGrpInf.OrgnlCreDtTm = request.OriginalMessageCreatedDateTime;

            TransactionInformationDto TxInf = new TransactionInformationDto();
            TxInf.OrgnlEndToEndId = request.OriginalEndToEndId;
            SupplementaryDataDto SplmtryData = new SupplementaryDataDto();
            SplmtryData.PlcAndNm = "ACHSupplementaryData";
            Envelope Envlp = new Envelope();
            ACHSupplementaryData achSupplementaryData = new ACHSupplementaryData();
            achSupplementaryData.grpMerchId = request.GroupMerchantId;
            achSupplementaryData.terminalId = request.TerminalId.ToString();
            Envlp.achSupplementaryData = achSupplementaryData;
            SplmtryData.Envlp = Envlp;
            TxInf.SplmtryData = SplmtryData;
            PaymentStatusRequest.GrpHdr = GrpHdr;
            PaymentStatusRequest.OrgnlGrpInf = OrgnlGrpInf;
            PaymentStatusRequest.TxInf = TxInf;

            var paymentRequestDto = new PaymentRequestDto
            {
                FIToFIPmtStsReq = PaymentStatusRequest
            };
            xmlRequest = MpcssMethods.ConvertRequestToXMLString(paymentRequestDto);
            xmlRequest = MpcssMethods.BuildAchNs2Data(xmlRequest);
            MqMessage message = MessageBuilder.ConstructMqMessage(xmlRequest, request.MessageIdentificationCode, MPCSSMessageTypes.PAYMENT_ENQUIRY_MESSAGE_TYPE, date);
            return message;
        }

    }
}
