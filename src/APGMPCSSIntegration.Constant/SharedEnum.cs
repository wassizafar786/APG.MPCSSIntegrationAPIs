using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace APGMPCSSIntegration.Constant
{
    public static class SharedEnums
    {
        public static string GetEnumDescription(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            if (field != null && Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }
            return nameof(enumValue);
        }
        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
        }
    }

    public enum CommunicationMessageType
    {
        ISO = 1,
        HTTP = 2,
        JSON = 3,
        XML = 4
    }

    public enum CommunicationMessageFormat
    {
        RecordRegistration = 1,
        AccountRegistration = 2,
        PaymentCreditDebit = 3,
        PaymentEnquiry = 4,
        PaymentReceipt = 5,
        PaymentStatusReport = 6,
        RecordVerification = 7,
        AccountVerification = 8
    }

    public enum TransactionType
    {
        Invalid = 0,
        
        Otp = 1,
        Purchase = 2,
        VoidPurchase = 3,
        Refund = 4,
        VoidRefund = 5,
        Authorize = 6,
        VoidAuthorize = 7,
        Completion = 8,
        VoidCompletion = 9,
        RefundCompletion = 10,
        CardInquiry = 11,
        
        P2BPull = 12,
        P2BPush = 13,
        P2BRefund = 14,
        B2BSend = 15,
        B2BReceive = 16,
        B2BRefund = 17,
        B2PSend = 18,
        WalletEnquiry = 19,
        
        CashIn = 20,
        CashOut = 21,
        
        AggregatorAdjustment = 997,
        AggregatorCommission = 998,
        AutomaticWithdrawal = 999,
        ManualWithdrawal = 1000,
    }
    
    [Flags]
    public enum TransactionInstruments
    {
        Debit = 1,
        Credit = 2,
        Financial = 4,
        DebitFinancial = 5,
        CreditFinancial = 6,
        NonFinancial = 8,
        DebitNonFinancial = 9,
        CreditNonFinancial = 10
    }

    public enum TransactionMethods
    {
        ManualPOS = 1,
        Magnetic = 2,
        Chip = 3,
        CardECommerce = 4,
        MobileNumber = 5,
        AliasName = 6,
        DigitalQR = 7,
        WalletECommerce = 8
    }

    public enum OmanNetActionCode
    {
        [XmlEnum("1")]
        Purchase = 1,
        [XmlEnum("2")]
        Refund = 2,
        [XmlEnum("3")]
        VoidPurchase = 3,
        [XmlEnum("4")]
        Authorization = 4,
        [XmlEnum("5")]
        Completion = 5,
        [XmlEnum("6")]
        VoidRefund = 6,
        [XmlEnum("7")]
        VoidCompletion = 7,
        [XmlEnum("8")]
        Inquiry = 8,
        [XmlEnum("9")]
        VoidAuthorization = 9,
        [XmlEnum("10")]
        TokenRegistration = 10,
        [XmlEnum("11")]
        TokenDeRegistration = 11
    }

    public enum OmanNetInstrument
    {
        [XmlEnum("D")]
        Debit = 'D'
    }

    public enum RequestProcessingType
    {
        TCPInitialize = 1,
        TCPAuthenticate = 2,
        TCPProcess = 3,
        HTTPRedirect = 4
    }


    public enum DynamicRolesEnum
    {
        SysAdmin = 1,
        BankUserAdmin = 2,
        BankMerchantAdmin = 3,
        BankBusinessUser = 4,
        BankFinancialAdmin = 5,
        BankAuditor = 6,
        BankHelpDesk = 7,
        BankAdminSupervisor = 8,
        MerchantAdmin = 9,
        MerchantBranchAdmin = 10,
        TerminalAdmin = 11,
        TerminalUser = 12
    }
    
    public enum TerminalTypes
    {
        POS = 1,
        mPOS = 2,
        WebTerminal = 3,
        WalletTerminal = 4
    }

    public enum RequestSources
    {
        NoThing = 0,
        Portal = 1,
        AmwalCheckout = 2,
        POS = 3,
        MPOS = 4,
        Webhook = 5,
        MerchantApp = 6,
        System = 7,
        MerchantAppSDK = 8,
        APGMemberShip = 9,
        APGFundamentals = 10,
        APGExecution = 11,
        APGTransaction = 12,
        APNotification = 13,
        APGDigitalIntegration = 14,
        Anonymous = 15

    }
  

    public enum LogSource
    {
        All = 0,
        MerchantApiOnly = 1,
        IgnorePortalOnly = 2
    }

    public enum InstrumentType
    {
        Debit = 1,
        Credit = 2
    }

    public enum CardBrandType
    {
        Visa = 1,
        Master = 2
    }

    public enum ConfigParam
    {
        SecureHashEnabled = 1,
        LoginTrialsNumber,
        IsOtpSimulation,
        VerificationCodeExpiryTime,
        VodafoneSMSAccountId,
        VodafoneSMSSenderName,
        VodafoneSMSPassword,
        UrlShortenerServiceURL,
        AMWALPayRoot,
        TwilioAccountSID,
        TwilioAuthToken,
        TwilioFromNumber,
        IvasServiceUrl,
        VodafoneServiceUrl,
        ActiveSMSApi,
        PushNotificationServiceURL,
        PushNotificationAuthorizationKey,
        IvasSMSEmail,
        IvasSMSpassword,
        UserPasswordExpiryDay,
        HistoricalPasswordCountToCheck,
        MultileSessionSameMerchant,
        SendOrderPaymentNotificationToMerchant,
        LogOutTimeOut,
        CodeTagQRHeight,
        CodeTagQRWidth,
        MaxNumberOfExportRecords,
        IsUseSignalR,
        FirstTimeLoginDormantDuration,
        TerminalIdsFrom,
        TerminalIdsTo,
        SendSMSWithResetPassword,
        SendEmailWithResetPassword,
        SendEmailSMTPHost,
        SendEmailSMTPPort,
        SendEmailsmtpPassword,
        SendAllEmailsToSingleAddress,
        ForcedEmailRecipient,
        FromAddress,
        MailFrom,
        SendMailWithChangePassword,
        SendSMSWithChangePassword,
        MerchantIdsFrom,
        MerchantIdsTo,
        SendEmailsmtpUserName,
        TransactionSMSTemplate,
        SendEmailWithSupportTicket,
        TransactionSMSTemplateVoid,
        SendSMSWithTransactionSale,
        SendSMSWithTransactionVoid,
        IsMDRExceptional,
        UserCreationSubjectEmailTemplate,
        UserCreationBodyEmailTemplate,
        UserResetPasswordSubjectEmailTemplate,
        UserResetPasswordBodyEmailTemplate,
        EnableMultiRefundTransactionPerBank,
        EnableRefundWithConvFees,
        CheckBalanceForBACC,
        UserCreationSendSMSMessage,
        UserNewPasswordSendSMSMessage,
        OTPSMSMessageTemplate,
        OTPEmailMessageTemplate,
        OTPVerificationMethod,
        OTPEmailSubject,
        ForceSendOrderNotificationToConsumer,
        SendOrderNotificationToConsumer,
        CashEnabled,
        OrderPaymentSubjectEmailTemplate,
        OrderPaymentBodyEmailTemplate,
        DefaultCurrencyCode,
        OrderRequestARSMSTemplate,
        OrderRequestENSMSTemplate,
        OrderPaymentARSMSTemplate,
        OrderPaymentENSMSTemplate,
        EnableSendTerminalNameMiGS,
        TakeMDRWithRefund,
        UserCreationBodyEmailTemplateBankOrTerminal,
        CalculateAcqFeesWithRefund,
        DormantUserLoginDuration,
        PasswordUserChangeDuration,
        OTPExpiredWithin,
        IsSimulation,
        TrialRangeInHour,
        EnableCheckAggregatorLimit,
        RunAutomaticSettlementFilesCreation,
        DTFPath,
        CreatePhysicalDTFSettlementFile,
        VerifyOTPLimit,
        ShadowBalanceAmountCalculationMethod,
        SmartBoxURL,
        OrderCreationEmailSubject,
        OrderCreationEmailBody,
        OrderCreationSMSBody,
        BaseTransactionsMicroServiceUrl,
        BaseFundamentalsMicroServiceUrl,
        BaseLogsMicroServiceUrl,
        BaseMembershipMicroServiceUrl,
        BaseExecutionMicroServiceUrl,
        SimulateOmanNetCardGateway,
        // ISOBICCode,
        QROrderExpiryDateTime_Min,
        ConvenienceFeeFixed,
        ConvenienceFeePercentage,
        TipOrConvenienceIndicator,
        DeleteExpiredQRAfterInDays,
        BaseSmartBoxApiGatewayUrl,
        SaleEmailReceiptTemplate,
        B2BReceiveEmailReceiptTemplate,
        P2BEmailReceiptTemplate,
        B2BSendEmailReceiptTemplate,
        MerchantOrderExpiryDateTime_Min,
        SimulateMPCSSTransaction,
        APGDigitalPaymentGateway,
        TransactionReceiptSubjectEmailTemplat,
        BasePortalApiGatewayUrl,
        BaseNotificationsMicroServiceUrl,
        ActiveProvider,
        UploadFileSizeLimit,
        QR_PayloadFormatIndicator,
        ShowParsedQRatCheckOut,
        MPCSSTransactionTimeoutInSeconds,
        MPCSSInwardTransactionInternalTimeout,
        BaseDigitalIntegrationMicroServiceUrl,
        BulkFileMaxRows,
        BulkFileMaxSizeInKB,
        BulkMerchantFileColumnsCount,
        BulkTerminalFileColumnsCount,
        MPCSSPSPRouteCode,
        MobileAccountSelector,
        SimulateMPCSSOperationStatus,
        OmanNetCardHostTransactionDomain,
        OmanNetCardHostInitializationAddress,
        OmanNetCardHostProcessAddress,
        OmanNetCardHostProcessAddressWithAuth,
        OmanNetCardHostInquiryDomain,
        OmanNetCardHostInquiryAddress,
        OmanNetCardResponseURL,
        OmanNetCardErrorURL,
        IsSimulatedWalletEnquirySuccess,
        IsSimulatedWalletRefundSuccess,
        SimulatedDigitalTransactionResponse,
        SystemTimezoneOffset,
        MPCSSVerificationTimeoutInSeconds,
        MPCSSOutwardTransactionInternalTimeout
    }

    public enum SimulatedDigitalTransactionResponse
    {
        None = 0,
        TimeoutResponseReceived = 1,
        NoResponseReceived = 2,
        LateSuccessResponseReceived = 3,
        LateFailureResponseReceived = 4,
        SuccessResponseReceived = 5,
        FailureResponseReceived = 6
    }

    public enum MerchantAccountTypeEnum
    {
        BACC = 1,
        SVA = 2,
        SettleByAggregator = 3
    }

    public enum ChannelTypeEnum
    {
        Card = 1,
        Wallet = 2,
        Cash = 3
    }
    public enum MPCSSStatus
    {
        [Description("Initiated")]
        Initiated = 1,
        [Description("Pending")]
        Pending = 2,
        [Description("Success")]
        Success = 3,
        [Description("Error")]
        Error = 4
    }

    public enum MPCSSRecordRequest
    {
        None = 0,
        [Description("RecordOpeningRequest")]
        RecordOpeningRequest = 1,
        [Description("RecordMaintenanceRequest")]
        RecordMaintenanceRequest = 2,
        [Description("RecordClosingRequest")]
        RecordClosingRequest = 3,
        [Description("AccountOpeningRequest")]
        AccountOpeningRequest = 4,
        [Description("AccountMaintenanceRequest")]
        AccountMaintenanceRequest = 5,
        [Description("AccountClosingRequest")]
        AccountClosingRequest = 6,
        PaymentOutwardCreditRequest = 7,
        PaymentOutwardDebitRequest = 8,
        PaymentInwardCreditRequest = 9,
        PaymentInwardDebitRequest = 10,
        PaymentStatusReport = 11,
        PaymentEnquiry = 12,
        [Description("CustomerNameVerificationRequest")]
        CustomerNameVerificationRequest = 13,
        DefaultAccountVerificationRequest = 14,
        PaymentReturnRequest = 15,
        RegistrationResponse = 16,
        [Description("DebitPaymentRequest")]
        DebitPaymentRequest = 17,
        [Description("QRPaymentRequest")]
        QRPaymentRequest = 18
    }

    public enum MPCSSCustomerType
    {
        [Description("MER")]
        Merchant,
        [Description("PER")]
        Person
    }
    
    

    public enum MPCSSAccountType
    {
        [Description("C")]
        MerchantId,
        [Description("M")]
        MobileNUmber
    }
    public enum MPCSSOperationStatus
    {
        Initiated = 1,
        Pending = 2,
        Success = 3,
        Error = 4
    }
    public enum MPCSSResponseStatus
    {
        [Description("Accepted")]
        ACPT,
        [Description("Rejected")]
        RJCT
    }
    public enum LogOperation
    {
        Insert,
        Update,
        Delete,
        Search
    }

    public enum MPCSSResponseReasonCode
    {
        [Description("Account is invalid or already exists.")]
        InvalidAccount = 1,
        AccountIsClosedOrBlocked = 2,
        DeceasedAccountHolder = 3,
        DormantAccount = 4,
        InsufficientFunds = 5,
        DuplicateTransaction = 6,

        [Description("Successfully processed.")]
        ProcessedSuccessfully = 1000,
        TechnicalError = 1001,
        ParsingError = 1002,
        DigitalSignatureOrSecurityError = 1003,

        [Description("Missing required information.")]
        InvalidIdFormat = 1004,
        
        SenderIsNotAllowedToSendMessageType = 1005,
        ReceiverIsNotAllowedToReceiveMessageType = 1006,
        PurposeIsNotAllowedForSending = 1007,
        InvalidReason = 1008,
        NoSessionAvailable = 1009,
        AutoReplied = 1010,
        DebitCapExceeded = 1016,
        CreditCapExceeded = 1017,
        LimitsExceeded = 1018,
        TransactionAmountOutOfRange = 1019,
        TransactionCountOutOfRange = 1020,
        UnregisteredPspPayment = 1100,

        [Description("Account already exists.")]
        AliasAlreadyUsed = 1101,
        MaximumNumberOfAccountsReached = 1102,
        CouldNotResolveDebtor = 1103,
        CouldNotResolveCreditor = 1104,
        ReplyTimeoutReached = 1105,
        PaymentAuthorizationFailed = 1106,
        RiskThresholdBreached = 1107
    }
    
    public enum TransactionIdentifier
    {
        None =0,
        TransactionIdN = 1,
        TransactionId = 2,
        SystemTraceNr = 3
    }
    
    public enum DigitalTransactionIdentifier
    {
        None = 0,
        DigitalTransactionId = 1,
        DigitalTransactionIdN = 2
    }
    
    public static class EnquiryOriginalTransactionStatus
    {
        public const string LateSuccess = "LateSuccess";
        public const string Failure = "Failure";
        public const string Unconfirmed = "Unconfirmed";
    }

    public static class TimeoutActions
    {
        public const string Refunded = "Refunded.";
        public const string FailedToRefund = "Refund Trial Exceed.";
        public const string FailedToEnquiry = "Enquiry Trial Exceed.";
        public const string NoActionTaken = "No Action Required.";
        public const string NoActionCouldBeTaken = "No Automatic Action Could Be Taken.";
        public const string EnquiryInitiated = "Enquiry #{0} Initiated.";
        public const string RefundInitiated = "Refund #{0} Initiated.";
        public const string PendingRefund = "Pending Refund.";
        public const string PendingEnquiry = "Pending Enquiry.";
    }

    public static class JobStates
    {

        public const string ReadyToProcess = "ReadyToProcess";
        public const string Running = "Running";
        public const string Complete = "Complete";
    }

    public static class TimeoutJob
    {
        public const int DetectionDelayMargin = 30;
    }
    
    public static class DigitalTransactionStatus
    {
        public const string ReadyToProcess = "ReadyToProcess";
        public const string Processing = "Processing";
        public const string Success = "Success";
        public const string Failure = "Failure";
        public const string Timeout = "Timeout";
        public const string SystemTimeout = "Timeout - Detected By System";
        public const string LateSuccess = "LateSuccess";
        public const string LateFailure = "LateFailure";
    }

}
