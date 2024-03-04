using APGMPCSSIntegration.IAL.Internal.BaseRequests;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace APGMPCSSIntegration.Common.CommonViewModels.Response
{
    public class SupplementaryDataResponseDto
    {
        public string PlcAndNm { get; set; }
        public EnvelopeResponseDto Envlp { get; set; }
    }
    public class EnvelopeResponseDto
    {
        [XmlElement("achSupplementaryData")]
        public ACHSupplementaryDataResponseDto achSupplementaryData { get; set; }
    }
    public class ACHSupplementaryDataResponseDto 
    {

        [XmlElement("batchSource")]
        public string BatchSource { get; set; }
        
        [XmlElement("consumerID")]
        public string ConsumerID { get; set; }
        
        [XmlElement("merchCategoryCd")]
        public string MerchantCategoryCode { get; set; }
        
        [XmlElement("grpMerchId")]
        public string GroupMerchantId { get; set; }
        
        [XmlElement("terminalId")]
        public string TerminalId { get; set; }
        
        [XmlElement("filler")]
        public string Filler { get; set; }
        
        [XmlElement("merchantName")]
        public string MerchantName { get; set; }
        
        [XmlElement("pntOfInitiateMethd")]
        public string PointOfInitiateMethd { get; set; }
        
        [XmlElement("MsgtipOrConvnceIndctrId")]
        public string MsgtipOrConvnceIndctrId { get; set; }
        
        [XmlElement("feePercentage")]
        public string FeePercentage { get; set; }
        
        [XmlElement("countryCd")]
        public string CountryCode { get; set; }
        
        [XmlElement("merchantCity")]
        public string MerchantCity { get; set; }
        
        [XmlElement("postCd")]
        public string PostCode { get; set; }
        
        [XmlElement("invoiceNumber")]
        public string InvoiceNumber { get; set; }
        
        [XmlElement("sessionSequence")]
        public string SessionSequence { get; set; }
        
        [XmlElement("receiverIdIssuingCountry")]
        public string ReceiverIdIssuingCountry { get; set; }
        
        [XmlElement("receiverIdType")]
        public string ReceiverIdType { get; set; }
        
        [XmlElement("receiverIdValue")]
        public string ReceiverIdValue { get; set; }
        
        [XmlElement("receiverName")]
        public string SupplementaryReceiverName { get; set; }
        
    }
}
