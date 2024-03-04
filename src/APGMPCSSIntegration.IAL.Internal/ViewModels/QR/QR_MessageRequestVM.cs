using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.QR
{
    public class QR_MessageRequestVM
    {
        public int DataPresented { get; set; }
        public int DataProvider { get; set; }
        public string InitiationMethod { get; set; }

        public string MerchantIdentifierAliasName { get; set; }
        public string MerchantRef { get; set; }
        public string MobileNumber { get; set; }
        public string MerchantAliasName { get; set; }
        public string IdNumber { get; set; }
        public string NameOnID { get; set; }
        public string DateOfBirth { get; set; }
        public string IDExpiry { get; set; }

        public string TerminalId { get; set; }
        public string GroupMerchId { get; set; }
        public string ConsumerId { get; set; }
        public string InvoiceNo { get; set; }


        public string PayloadFormatIndicator { get; set; }
        public string MerchantTxnIdentifier { get; set; }

        public string BICCode { get; set; }

        public string MerchantIdentifier { get; set; }
        public string MerchantId { get; set; }

        public string PersonalIdentifier { get; set; }
        public string PersonalAliasName { get; set; }
        public string Filler { get; set; }
        public string MerchantCategoryCode { get; set; }
        public string TransactionCurrencyCode { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TipOrConvenienceIndicator { get; set; }
        public string ConvenienceFeeFixed { get; set; }
        public string ConvenienceFeePercentage { get; set; }
        public string CountryCode { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCity { get; set; }
        public string PostalCode { get; set; }
        public string AdditionalDataField { get; set; }
        public string CRC { get; set; }
        public string Reserved { get; set; }
    }
    
}
