namespace APGMPCSSIntegration.Constant
{
    public class ResponseMessageHandler
    {

        public ResponseMessageHandler() { }
        public string GetMessage(PaymentFailureMessage code)
        {
            return code switch
            {

                PaymentFailureMessage.ListenerFailed => "Problem occured while starting Listeners",

                PaymentFailureMessage.RequestInitiationFailed => "Request Initiation Failed due to Exception",

                _ => throw new ArgumentOutOfRangeException(nameof(code), code, null),
            };
        }

        public string GetMessage(PaymentSuccessMessage code)
        {
            return code switch
            {

                PaymentSuccessMessage.ListenersStartedSuccessfully => "Listeners Started Successfully",

                PaymentSuccessMessage.ConnectionClosedSuccessfully => "Connections Closed Successfully",

                PaymentSuccessMessage.RequestSentSuccessfully => "Request Sent Successfully",

                PaymentSuccessMessage.ResponseReceived => "Response Received Successfully",


                _ => throw new ArgumentOutOfRangeException(nameof(code), code, null),
            };
        }

        public string MapToAPGResponseCode(PaymentRejectionReason code)
        {
            return code switch
            {
                PaymentRejectionReason.InvalidAccount => "101",
                PaymentRejectionReason.AccountClosedBlocked => "102",
                PaymentRejectionReason.DeceasedAccountHolder => "103",
                PaymentRejectionReason.DormantAccount => "104",
                PaymentRejectionReason.InsufficientFunds => "105",
                PaymentRejectionReason.DuplicateTransaction => "106",
                PaymentRejectionReason.ProcessedSuccessfully => "107",
                PaymentRejectionReason.TechnicalError => "108",
                PaymentRejectionReason.ParsingError => "109",
                PaymentRejectionReason.DigitalSignatureSecurityError => "110",
                PaymentRejectionReason.InvalidIDFormat => "111",
                PaymentRejectionReason.SenderNotAllowedToSendMessageType => "112",
                PaymentRejectionReason.ReceiverNotAllowedToReceiveMessageType => "113",
                PaymentRejectionReason.PurposeNotAllowedForSending => "114",
                PaymentRejectionReason.InvalidReason => "115",
                PaymentRejectionReason.NoSessionAvailable => "116",
                PaymentRejectionReason.AutoReplied => "117",
                PaymentRejectionReason.DebitCapExceeded => "118",
                PaymentRejectionReason.CreditCapExceeded => "119",
                PaymentRejectionReason.LimitsExceeded => "120",
                PaymentRejectionReason.TransactionAmountOutOfRange => "121",
                PaymentRejectionReason.UnregisteredPSP => "122",
                PaymentRejectionReason.AliasAlreadyUsed => "123",
                PaymentRejectionReason.MaximumNumberOfAccountsReached => "124",
                PaymentRejectionReason.CouldNotResolveDebtor => "125",
                PaymentRejectionReason.CouldNotResolveCreditor => "126",
                PaymentRejectionReason.ReplyTimeoutReached => "127",
                PaymentRejectionReason.PaymentAuthorizationFailed => "128",
                PaymentRejectionReason.RiskThresholdBreached => "129",

                _ => "130"
            };
    
        }

        public string GetMessage(PaymentRejectionReason code)
        {
            return code switch
            {
                
                PaymentRejectionReason.InvalidAccount => "InvalidAccount",
                PaymentRejectionReason.AccountClosedBlocked => "AccountClosedBlocked",
                PaymentRejectionReason.DeceasedAccountHolder => "DeceasedAccountHolder",
                PaymentRejectionReason.DormantAccount => "DormantAccount",
                PaymentRejectionReason.InsufficientFunds => "InsufficientFunds",
                PaymentRejectionReason.DuplicateTransaction => "DuplicateTransaction",
                PaymentRejectionReason.ProcessedSuccessfully => "ProcessedSuccessfully",
                PaymentRejectionReason.TechnicalError => "TechnicalError",
                PaymentRejectionReason.ParsingError => "ParsingError",
                PaymentRejectionReason.DigitalSignatureSecurityError => "DigitalSignatureSecurityError",
                PaymentRejectionReason.InvalidIDFormat => "InvalidIDFormat",
                PaymentRejectionReason.SenderNotAllowedToSendMessageType => "SenderNotAllowedToSendMessageType",
                PaymentRejectionReason.ReceiverNotAllowedToReceiveMessageType => "ReceiverNotAllowedToReceiveMessageType",
                PaymentRejectionReason.PurposeNotAllowedForSending => "PurposeNotAllowedForSending",
                PaymentRejectionReason.InvalidReason => "InvalidReason",
                PaymentRejectionReason.NoSessionAvailable => "NoSessionAvailable",
                PaymentRejectionReason.AutoReplied => "AutoReplied",
                PaymentRejectionReason.DebitCapExceeded => "DebitCapExceeded",
                PaymentRejectionReason.CreditCapExceeded => "CreditCapExceeded",
                PaymentRejectionReason.LimitsExceeded => "LimitsExceeded",
                PaymentRejectionReason.TransactionAmountOutOfRange => "TransactionAmountOutOfRange",
                PaymentRejectionReason.UnregisteredPSP => "UnregisteredPSP",
                PaymentRejectionReason.AliasAlreadyUsed => "AliasAlreadyUsed",
                PaymentRejectionReason.MaximumNumberOfAccountsReached => "MaximumNumberOfAccountsReached",
                PaymentRejectionReason.CouldNotResolveDebtor => "CouldNotResolveDebtor",
                PaymentRejectionReason.CouldNotResolveCreditor => "CouldNotResolveCreditor",
                PaymentRejectionReason.ReplyTimeoutReached => "ReplyTimeoutReached",
                PaymentRejectionReason.PaymentAuthorizationFailed => "PaymentAuthorizationFailed",
                PaymentRejectionReason.RiskThresholdBreached => "RiskThresholdBreached",

                _ => "Technical error"
            };
        }


    }
    
    public enum PaymentSuccessMessage
    {
        ListenersStartedSuccessfully = 2000,
        ConnectionClosedSuccessfully = 2001,
        RequestSentSuccessfully = 0,
        ResponseReceived = 2003,
    }

    public enum PaymentFailureMessage
    {
        ListenerFailed = 4000,
        RequestInitiationFailed = 4001
    }

    public enum PaymentRejectionReason
    {
        InvalidAccount = 1,
        AccountClosedBlocked = 2,
        DeceasedAccountHolder = 3,
        DormantAccount = 4,
        InsufficientFunds = 5,
        DuplicateTransaction = 6,
        ProcessedSuccessfully = 1000,
        TechnicalError = 1001,
        ParsingError = 1002,
        DigitalSignatureSecurityError = 1003,
        InvalidIDFormat = 1004,
        SenderNotAllowedToSendMessageType = 1005,
        ReceiverNotAllowedToReceiveMessageType = 1006,
        PurposeNotAllowedForSending = 1007,
        InvalidReason = 1008,
        NoSessionAvailable = 1009,
        AutoReplied = 1010,
        DebitCapExceeded = 1016,
        CreditCapExceeded = 1017,
        LimitsExceeded = 1018,
        TransactionAmountOutOfRange = 1019,
        TransactionCountOutOfRange = 1020,
        UnregisteredPSP = 1100,
        AliasAlreadyUsed = 1101,
        MaximumNumberOfAccountsReached = 1102,
        CouldNotResolveDebtor = 1103,
        CouldNotResolveCreditor = 1104,
        ReplyTimeoutReached = 1105,
        PaymentAuthorizationFailed = 1106,
        RiskThresholdBreached = 1107

    }
}
