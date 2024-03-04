using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGMPCSSIntegration.Constant
{
    public static class QRConstants
    {
    }

    public static class QR_Tag
    {
        public const string PayloadFormatIndicator = "00";
        public const string InitiationMethod = "01";
        public const string MerchantId = "08";
        public const string MerchantTxnIdentifier = "26";
        public const string Reserved = "00";
        public const string MerchantIdentifier = "01";
        public const string MerchantAliasName = "02";
        public const string MerchantRef = "03";
        public const string PersonalIdentifier = "27";
        public const string MobileNumber = "01";
        public const string PersonalAliasName = "02";
        public const string IdNumber = "03";
        public const string NameOnID = "04";
        public const string DateOfBirth = "05";
        public const string IDExpiry = "06";
        public const string Filler = "28";
        public const string MerchantCategoryCode = "52";
        public const string TransactionCurrencyCode = "53";
        public const string TransactionAmount = "54";
        public const string TipOrConvenienceIndicator = "55";
        public const string ConvenienceFeeFixed = "56";
        public const string ConvenienceFeePercentage = "57";
        public const string CountryCode = "58";
        public const string MerchantName = "59";
        public const string MerchantCity = "60";
        public const string PostalCode = "61";
        public const string AdditionalDataField = "62";
        public const string TerminalId = "00";
        public const string GroupMerchId = "01";
        public const string ConsumerId = "02";
        public const string AdditionalDataField_Filler = "03";
        public const string InvoiceNo = "04";
        public const string CRC = "63";
    }

    public static class QR_TagName
    {
        public const string PayloadFormatIndicator = "PayloadFormatIndicator";
        public const string InitiationMethod = "InitiationMethod";
        public const string MerchantId = "MerchantIdIdentification";
        public const string MerchantTxnIdentifier = "MerchantTxnIdentifier";
        public const string Reserved = "Reserved";
        public const string MerchantIdentifier = "MerchantIdentifier";
        public const string MerchantAliasName = "MerchantAliasName";
        public const string MerchantRef = "MerchantRef";
        public const string PersonalIdentifier = "PersonalIdentifier";
        public const string MobileNumber = "MobileNumberIdentification";
        public const string PersonalAliasName = "PersonalAliasName";
        public const string IdNumber = "IdNumber";
        public const string NameOnID = "NameOnID";
        public const string DateOfBirth = "DateOfBirth";
        public const string IDExpiry = "IDExpiry";
        public const string Filler = "Filler";
        public const string MerchantCategoryCode = "MerchantCategoryCode";
        public const string TransactionCurrencyCode = "TransactionCurrencyCode";
        public const string TransactionAmount = "TransactionAmount";
        public const string TipOrConvenienceIndicator = "TipOrConvenienceIndicator";
        public const string ConvenienceFeeFixed = "ConvenienceFeeFixed";
        public const string ConvenienceFeePercentage = "ConvenienceFeePercentage";
        public const string CountryCode = "CountryCode";
        public const string MerchantName = "MerchantName";
        public const string MerchantCity = "MerchantCity";
        public const string PostalCode = "PostalCode";
        public const string AdditionalDataField = "AdditionalDataField";
        public const string TerminalId = "TerminalId";
        public const string GroupMerchId = "GroupMerchId";
        public const string ConsumerId = "ConsumerId";
        public const string InvoiceNo = "InvoiceNo";
        public const string CRC = "CRC";
    }

    public static class QR_TagLength 
    {
        public const string CRC = "04";
    }


    public enum QR_DataTypeEnum
    {
        N = 0,
        ANS = 1,
        AN = 2
    }
    public enum QR_UsageEnum
    {
        Mandatory = 0,
        Optional = 1,
        Conditional = 2
    }

    public enum QR_DataPresented_By_Merchant
    {
        QRMobilePayments = 1,
        NFCMobilePayments = 2
    }

    public enum QR_DataProvider
    {
        Static = 1,
        Dynamic = 2,
        Dynamic_ecommerce = 3
    }

}
