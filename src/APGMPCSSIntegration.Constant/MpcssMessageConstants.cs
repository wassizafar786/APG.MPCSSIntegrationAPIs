using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.Constant;

public static class MPCSSQueues
{
    public static void Initialize(string participantShortName)
    {
        ParticipantShortName = participantShortName;

        OutwardPaymentQueue = $"mpc.{ParticipantShortName}.payment.outward";
        InwardPaymentQueue = $"mpc.{ParticipantShortName}.payment.inward";
        PaymentEnquiryRequestQueue = $"mpc.{ParticipantShortName}.paymentenquiry.outward";
        PaymentEnquiryResponseQueue = $"mpc.{ParticipantShortName}.paymentenquiry.inward";
        OutwardReplyQueue = $"mpc.{ParticipantShortName}.reply.outward";
        InwardReplyQueue = $"mpc.{ParticipantShortName}.reply.inward";
        RegistrationRequestQueue = $"mpc.{ParticipantShortName}.reg.outward";
        RegistrationResponseQueue = $"mpc.{ParticipantShortName}.reg.inward";
        RegistrationFileRequestQueue = $"mpc.{ParticipantShortName}.regfile.outward";
        RegistrationFileResponseQueue = $"mpc.{ParticipantShortName}.regfile.inward";
        HeartBeatRequestQueue = $"mpc.{ParticipantShortName}.heartbeat.outward";
        HeartBeatResponseQueue = $"mpc.{ParticipantShortName}.heartbeat.inward";
        ReportsQueue = $"mpc.{ParticipantShortName}.reports.inward";
        CustomerNameRequestQueue = $"mpc.{ParticipantShortName}.customername.outward";
        CustomerNameResponseQueue = $"mpc.{ParticipantShortName}.customername.inward";
        CheckDefaultRequestQueue = $"mpc.{ParticipantShortName}.checkdefault.outward";
        CheckDefaultResponseQueue = $"mpc.{ParticipantShortName}.checkdefault.inward";
        ReportsTestQueue = "mpc.mpct.reports.inward";
    }

    public static string ParticipantShortName { get; private set; }
    public static string OutwardPaymentQueue {get; private set;}
    public static string InwardPaymentQueue {get; private set;}
    public static string PaymentEnquiryRequestQueue {get; private set;}
    public static string PaymentEnquiryResponseQueue {get; private set;}
    public static string OutwardReplyQueue {get; private set;}
    public static string InwardReplyQueue {get; private set;}
    public static string RegistrationRequestQueue {get; private set;}
    public static string RegistrationResponseQueue {get; private set;}
    public static string RegistrationFileRequestQueue {get; private set;}
    public static string RegistrationFileResponseQueue {get; private set;}
    public static string HeartBeatRequestQueue {get; private set;}
    public static string HeartBeatResponseQueue {get; private set;}
    public static string ReportsQueue {get; private set;}
    public static string CustomerNameRequestQueue {get; private set;}
    public static string CustomerNameResponseQueue {get; private set;}
    public static string CheckDefaultRequestQueue {get; private set;}
    public static string CheckDefaultResponseQueue {get; private set;}
    public static string ReportsTestQueue {get; private set;}

}
public static class MpcssMessageConstants
{
    public const string HashAlgorithm = "SHA256";
    public const string PrivateKeyToken = "abcd";
    public const string PspFilePath = @"C:\mpcss\keys_bkplatest\";
    public const string mpcCertificateFile = "mpc.cer";
    public const string pspCertificateFile = "abcd.p12";
    public const string ReportFilePath = @"C:\mpcss\reports\";


    public static class CBO
    {
        public const string BICFI = "CBOMOMRUMPC";
        public const string CategoryPurpose = "1";
    }
        
    public static class Settlement
    {
        public const string SttlmMtd = "CLRG";
        public const string ChrgBr = "SLEV";
        public const string ClrSysPrtry = "CBO";
    }

    public static class PaymentTypeInformation
    {
        public const string LclInstrm = "TEL";
    }
        
    public static class SupplementaryData
    {
        public const string PlcAndName = "ACHSupplementaryData";
        public const string SessionSequence = "0";
        public const string BatchSource = "2";
    }
        
    public static class ResponseStatus
    {
        public const string Accepted = "ACSP";
        public const string Rejected = "RJCT";
    }

    public static string GetMPCSSMessageTypeByTransactionType(TransactionType transactionType)
    {
        return transactionType switch
        {
            TransactionType.P2BPull => MPCSSMessageTypes.CREDIT_MESSAGE_TYPE,
            TransactionType.P2BPush => MPCSSMessageTypes.CREDIT_MESSAGE_TYPE,
            TransactionType.P2BRefund => MPCSSMessageTypes.PAYMENT_RETURN_MESSAGE_TYPE,
            TransactionType.WalletEnquiry => MPCSSMessageTypes.PAYMENT_ENQUIRY_MESSAGE_TYPE,
                
            _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
        };
    }
        
}

public static class MPCSSMessageTypes
{
    public const string CREDIT_MESSAGE_TYPE = "pacs.008.001.05";  
    public const string DEBIT_MESSAGE_TYPE = "pacs.003.001.05";
    public const string PAYMENT_STATUS_REPORT_MESSAGE_TYPE = "pacs.002.001.06";
    public const string PAYMENT_ENQUIRY_MESSAGE_TYPE = "pacs.028.001.01";
    public const string PAYMENT_RETURN_MESSAGE_TYPE = "pacs.004.001.05";
    public const string OPEN_CUSTOMER_MESSAGE_TYPE = "cstmrreg.01.01";
    public const string MAINTAIN_CUSTOMER_MESSAGE_TYPE = "cstmrreg.02.01";
    public const string CLOSE_CUSTOMER_MESSAGE_TYPE = "cstmrreg.03.01";
    public const string OPEN_ACCOUNT_MESSAGE_TYPE = "cstmrreg.06.01";
    public const string MAINTAIN_ACCOUNT_MESSAGE_TYPE = "cstmrreg.07.01";
    public const string CLOSE_ACCOUNT_MESSAGE_TYPE = "cstmrreg.08.01";
    public const string CUSTOMER_NAME_VERIFICATION_REQUEST_MESSAGE_TYPE = "cstmrreg.20.01";
    public const string CUSTOMER_NAME_VERIFICATION_RESPONSE_MESSAGE_TYPE = "cstmrreg.21.01";
    public const string DEFAULT_ACCOUNT_VERIFICATION_REQUEST_MESSAGE_TYPE = "cstmrreg.25.01";
    public const string DEFAULT_ACCOUNT_VERIFICATION_RESPONSE_MESSAGE_TYPE = "cstmrreg.26.01";
    public const string REGISTRATION_RESPONSE_MESSAGE_TYPE = "cstmrreg.10.01";
}
    

    
public class MPCSSAccountIdentificationTypes
{
    public static readonly MPCSSAccountIdentificationTypes MobileNumberIdentification = new("M");
    public static readonly MPCSSAccountIdentificationTypes AliasNameIdentification = new("A");
    public static readonly MPCSSAccountIdentificationTypes MerchantIdIdentification = new("C");
        
    public MPCSSAccountIdentificationTypes(string identifier)
    {
        Identifier = identifier;
    }

    public static MPCSSAccountIdentificationTypes GetByTransactionMethod(TransactionMethods transactionMethod)
    {
        return transactionMethod switch
        {
            TransactionMethods.AliasName => AliasNameIdentification,
            TransactionMethods.MobileNumber => MobileNumberIdentification,
                
            _ => throw new ArgumentOutOfRangeException(nameof(transactionMethod), transactionMethod, null)
        };
    }
    public string Identifier { get; }
}