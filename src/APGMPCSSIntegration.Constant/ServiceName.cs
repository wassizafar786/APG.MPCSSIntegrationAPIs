using APGMPCSSIntegration.Constant.Helpers;

namespace APGMPCSSIntegration.Constant
{
    public static class ServiceName
    {
        public static class Controllers
        {
            public const string DigitalTransaction = "DigitalTransaction";
            public const string DigitalOperation = "DigitalOperation";
            public const string Simulation = "Simulation";
            public const string QR = "QR";
            public const string HealthCheck = "HealthCheck";

        }

        public static class Fundamentals
        {
            public const string ServiceNameTerminal = "Terminal";
            public const string GetByParamKey = "GetByParamKey";
            public const string CheckShadowMerchantOrderNumberOfPayments = "CheckShadowMerchantOrderNumberOfPayments";
            public const string CheckWalletOrderNumberOfPayments = "CheckWalletOrderNumberOfPayments";
            public const string UpdateShadowMerchantOrderNumberOfPayments = "UpdateShadowMerchantOrderNumberOfPayments";
        }

        public static class Transactions
        {
            public const string CheckShadowBalanceLimit = "CheckShadowBalanceLimit";
            public const string RollbackDigitalTransaction = "RollbackDigitalTransaction";
        }       
        
        public static class Membership
        {
            public const string ServiceNameMembership = "Membership";

        }


    }

    public static class ServiceNameDigitalIntegration
    {
        public const string RecordRegistrationRequest = "RecordRegistrationRequest/{id}";
        public const string AccountRegistrationRequest = "AccountRegistrationRequest/{id}";
        public const string CreditPaymentRequest = "CreditPaymentRequest";
        public const string DebitPaymentRequest = "DebitPaymentRequest";
        public const string EnquiryRequest = "EnquiryRequest";
        public const string StatusReportRequest = "StatusReportRequest/{id}";
        public const string PaymentReturn = "PaymentReturn";
        public const string CustomerNameVerificationRequest = "CustomerNameVerificationRequest";
        public const string DefaultAccountVerificationRequest = "DefaultAccountVerificationRequest/{id}";

        public const string InwardCreditTransaction = "InwardCreditTransaction";
        public const string InwardCreditTransactionByWalletId = "InwardCreditTransactionByWalletId";
        public const string InwardDebitTransaction = "InwardDebitTransaction";
        public const string InwardPaymentReturnTransaction = "InwardPaymentReturnTransaction";
        public const string InwardPaymentEnquiry = "InwardPaymentEnquiry";

        public const string SendNoResponseMessage = "NoResponseMessageRequest";

        public const string ParseISOMessage = "ParseISOMessage";

        public const string ClearCash = "ClearCash";
        public const string Login = "login";
        public const string GenerateIsoQRCode = "GenerateIsoQRCode";
        public const string TestQrpayment = "TestQrpayment";
        public const string CheckDublicated = "CheckDublicated";
    }

    public static class Languages
    {
        public const string Arabic = "ar-EG";
        public const string English = "en-US";
    }

    public static class MicroServicesName
    {
        public const string APGMemberShip = "Membership";
        public const string APGFundamentals = "Fundamentals";
        public const string APGExecution = "Execution";
        public const string APGDigitalIntegration = "APGDigitalIntegration";
    } 
    
    public static class HealthCheckServicesName
    {
        public const string ActiveMq = "ActiveMq";
        public const string Redis = "Redis";
        public const string Db = "Db";
        public const string MassTransit = "MassTransit";
    }

    public static class ExceptionServiceSource
    {
        public const string APGExecution = "APGExecution";
        public const string APGFundamentals = "APGFundamentals";
        public const string APGTransaction = "APGTransaction";
        public const string APGMPCSSS = "APGMPCSSS";
    }

    public static class MicroServicesURL
    {
        static MicroServicesURL()
        {
            BaseTransactionsURL = new WriteOnce<string>();
            BaseFundamentalsURL = new WriteOnce<string>();
            BaseMembershipURL = new WriteOnce<string>();
            BaseAPGLogURL = new WriteOnce<string>();
        }

        public static WriteOnce<string> BaseFundamentalsURL { get; }
        public static WriteOnce<string> BaseMembershipURL { get; }
        public static WriteOnce<string> BaseTransactionsURL { get; }
        public static WriteOnce<string> BaseAPGLogURL { get; }
    }

    public static class ControllerNames
    {
        public static class Fundamentals
        {
            public const string Bank = "Bank";
            public const string Host = "Host";
            public const string MerchantBranch = "MerchantBranch";
            public const string Merchant = "Merchant";
            public const string Terminal = "Terminal";
            public const string TransactionRoute = "TransactionRoute";
            public const string MerchantOrder = "MerchantOrder";
            public const string MerchantRequest = "MerchantRequest";
            public const string MerchantRequestMessage = "MerchantRequestMessage";
            public const string Membership = "Membership";
            public const string MerchantData = "MerchantData";
            public const string ConfParameter = "ConfParameter";
            public const string BinConfiguration = "BinConfiguration";
            public const string ServiceNameQRCode = "QRCode";

        }

        public static class Transactions
        {
            public const string ShadowBalance = "ShadowBalance";
            public const string Transaction = "Transaction";
            public const string DigitalTransaction = "DigitalTransaction";
            public const string MarchnatOrderRequest = "MarchnatOrderRequest";
            public const string Lookup = "Lookup";
        }

        public static class Log
        {
            public const string CommunicationLog = "CommunicationLog";
        }





    }

    public static class ServiceNameLookup
    {
        public const string ServiceName = "Lookup";

        public const string GetAllCommunicationMessageType = "GetAllCommunicationMessageType";
        public const string GetCommunicationMessageTypeById = "GetCommunicationMessageTypeById";
        public const string GetAllCommunicationMessageFormat = "GetAllCommunicationMessageFormat";
        public const string GetCommunicationMessageFormatById = "GetCommunicationMessageFormatById";
        public const string GetAllCommunicationMessageFormatByMessageTypeId = "GetAllCommunicationMessageFormatByMessageTypeId";
        public const string GetAllTerminalType = "GetAllTerminalType";
        public const string GetTerminalTypeById = "GetTerminalTypeById";
        public const string GetAllCountry = "GetAllCountry";
        public const string GetAllCountryState = "GetAllCountryState";
        public const string GetAllMerchantCategoryCode = "GetAllMerchantCategoryCode";
        public const string GetAllMerchantRequestType = "GetAllMerchantRequestType";
        public const string IsTerminalMerchantValid = "IsTerminalMerchantValid";
        public const string GetMerchantTerminalTransactionData = "GetMerchantTerminalTransactionData";
        public const string IsMPCSSTerminalMerchantValid = "IsMPCSSTerminalMerchantValid";
        public const string GetHostOrHostOrder = "GetHostOrHostOrder";
        public const string GetMerchantRefIdByMerchantId = "GetMerchantRefIdByMerchantIdAsync";
        public const string GetMPCSSAccountPaymentDataModel = "GetMPCSSAccountPaymentDataModel";
        public const string GetOmanNetCardAccountByMerchantRefId = "GetOmanNetCardAccountByMerchantRefId";
        public const string GetByTerminalId = "GetByTerminalId";
        public const string Notification = "Notification";
        public const string DIWalletPaymentResponse = "DIWalletPaymentResponse";
        public const string DICustomerNameVerificationResponse = "DICustomerNameVerificationResponse";
        public const string OrderIsPaidSuccessfully = "OrderIsPaidSuccessfully";
        public const string ParseQR = "ParseQR";
        public const string CheckReplayAttach = "CheckReplayAttach";
    }

    public static class ServiceNameCommon
    {
        public const string Add = "Add";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string GetById = "GetById";
        public const string GetByMerchantId = "GetByMerchantId";
        public const string GetAll = "GetAll";
        public const string GetAllPaged = "GetAllPaginated";
        public const string GetAllExported = "GetAllExported";
        public const string Login = "login";
        public const string GetCardBinByBinNumber = "GetCardBinByBinNumber";
        public const string GetByExternalReceiptNumber = "GetByExternalReceiptNumber";
        public const string GetByExternalTransactionId = "GetByExternalTransactionId";
        public const string GetByStan = "GetByStan";
        public const string UpdateDigitalTransactionStatus = "UpdateDigitalTransactionStatus";
        public const string GetByMessageId = "GetByMessageId";
        public const string ValidateOriginalTransaction = "ValidateOriginalTransaction";
        public const string GetOriginalTransaction = "GetOriginalTransaction";
        public const string GetTransactionTypeById = "GetTransactionTypeById";
        public const string GetTransactionById = "GetTransactionById";
        public const string GetByTransactionId = "GetByTransactionId";
        public const string GetByTransactionFilter = "GetByTransactionFilter";
        public const string ResolvePaymentReturnTransactionType = "ResolvePaymentReturnTransactionType";
        public const string CheckPassword = "CheckPassword";

    }

    public static class ServiceNameHealthCheck
    {
        public const string GetActiveMQLastHeartbeatRecived = "GetActiveMQLastHeartbeatRecived";
    }

    public static class ProcessingCodes
    {
        public const string Purchase = "000000";
        public const string VoidPurchase = "020000";
        public const string Refund = "200000";
        public const string VoidRefund = "220000";
        public const string Authorize = "200100";
        public const string VoidAuthorize = "200200";
        public const string Completion = "200300";
        public const string VoidCompletion = "200400";
        public const string RefundCompletion = "200500";
        public const string CardInquiry = "200600";

        public const string P2BPull = "300000";
        public const string P2BPush = "310000";
        public const string P2BRefund = "300100";
        public const string B2BSend = "320000";
        public const string B2BReceive = "300100";
        public const string B2BRefund = "300200";
        public const string B2PSend = "300300";
        public const string B2PRefund = "300400";
        public const string WalletEnquiry = "300500";
    }

    public static class TransactionStatus
    {
        public const string Processing = "Processing";
        public const string Success = "Success";
        public const string Failure = "Failure";
        public const string Timeout = "Timeout";
        public const string LateSuccess = "LateSuccess";
    }
}
