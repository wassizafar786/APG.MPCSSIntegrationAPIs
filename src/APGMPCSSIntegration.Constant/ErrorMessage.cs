using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGMPCSSIntegration.Constant
{
    public static class ErrorMessage
    {
        public const string Success = "Success";
        public const string Error = "Error";
        public const string NoRequestFound = "No Request Found";
        public const string NoResponseFound = "No Response Found";
        public const string BusinessExceptionFailure = "APGEX:  ";
        public const string TechnicalExceptionFailure = "APGEX: @ExId an error has occured while processing the request , please try again later ";
        public const string MerchantTerminalError = "There is no Merchant Related to This Terminal";

    }

    public static class TechnicalErrorMessage
    {
        public const string TheRequestMustImplementISecureHashBase = "The Request Must Implement ISecureHashBase.";
    }

    public static class ResponseCodes
    {
        public const int SuccessCode = 0;
        public const string Success = "00";
        public const string TechnicalException = "01";
        public const string NoRequestFound = "02";
        public const string NoResponseFound = "03";
        public const string InvalidMerchantTerminal = "04";
        public const string InvalidTransactionType = "04";
        public const string Failure = "99";
        public const string Timeout = "39";
        public const string SystemTimeout = "42";
        public const string InvalidWalletOrder = "40";
        public const string IncorrectPassword = "102";
        public const string LockedUser = "103";
        public const string InActiveUser = "104";
        public const string ExceededTrialLimits = "105";
        public const string TimeoutSuccessResponseReceived = "5422"; // Detected by the background job.
        public const string TimeoutFailureResponseReceived = "5423"; // Detected by the background job.
        public const string DigitalTransactionHasAlreadyBeenProcessed = "5424"; // Detected by the background job.
    }
    
}
