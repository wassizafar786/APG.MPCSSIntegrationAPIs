using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGMPCSSIntegration.Constant
{
    public static class ResponseMessages
    {
        public const string InvalidTerminalId = "InvalidTerminalId";
        public const string ResponseFailure = "Failure";
        public const string Success = "Success";
        public const string InvalidRole = "InvalidRole";
        public const string IsDeleted = "IsDeleted";
        public const string MerchantNotActive = "MerchantNotActive";
        public const string BusinessExceptionFailure = "BusinessExceptionFailure";
        public const string TechnicalExceptionFailure = "TechnicalExceptionFailure";
        public const string PaymentAddedSuccessfully = "paymentAddedSuccessfully";
        public const string DontHonor = "DontHonor";
        public const string InvalidPAN = "Invalid PAN";
        public const string InvalidTransactionType = "InvalidTransactionType";
        public const string InvalidTransactionMethod = "InvalidTransactionMethod";
        public const string NoHostOrderReceived = "NoHostOrderReceived";
        public const string NoMerchantsFoundForThisTerminal = "NoMerchantsFoundForThisTerminal";
        public const string CardNumberIsRequired = "CardNumberIsRequired";
        public const string CardNumberLength = "CardNumberLength";
        public const string EmailNotSent = "OperationWasSuccessEmailNotSent";
        public const string InvalidValidatioBuilderParameters = "InvalidValidatioBuilderParameters";
        public const string InvalidWalletOrder = "InvalidWalletOrder";
        public const string InvalidIsCorrectPasswordValue = "InvalidIsCorrectPasswordValue";
        public const string ServiceUnavailablePleaseTryAgainLater = "ServiceUnavailablePleaseTryAgainLater";


    }

    public static class TechnicalErrorMessages
    {
        public const string InvalidSmtpHost = "SMTP Host is not configured";
        public const string InvalidSmtpPort = "SMTP Port is not configured";
        public const string InvalidSmtpUserName = "SMTP UserName is not configured";
        public const string InvalidSmtpPassword = "SMTP Password is not configured";
        public const string InvalidSmtpFrom = "SMTP From is not configured";
        public const string EmailToIsRequired = "Email To is required";
        public const string EmailSubjectIsRequired = "Email Subject is required";
        public const string EmailBodyIsRequired = "Email Body is required";
        
        public const string TheRequestMustImplementISecureHashBase = "The request must implement ISecureHashBase";

        public const string FundamentalsMicroServiceURLIsNotConfigured = "Fundamentals MicroService URL is not configured.";
        public const string TransactionsMicroServiceURLIsNotConfigured = "Transactions MicroService URL is not configured.";
        public const string MembershipMicroServiceURLIsNotConfigured = "Membership MicroService URL is not configured.";
        public const string LogsMicroServiceURLIsNotConfigured = "Logs MicroService URL is not configured.";
        
        public const string InvalidUrl = "One or more of the MicroServices URL is not set";
        public const string IncorrectPassword = "Incorrect Password";
        public const string LockedUser = "Locked User";
        public const string InActiveUser = "InActive User";
        public const string ExceededTrialLimits = "Exceeded Trial Limits";

    }
}
