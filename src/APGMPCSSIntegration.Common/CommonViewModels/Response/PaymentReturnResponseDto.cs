using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace APGMPCSSIntegration.Common.CommonViewModels.Response
{
    [XmlRoot("PmtRtr")]
    public class PaymentReturnResponseDto
    {
        public GroupHeaderResponseDto GrpHdr { get; set; }
        public ReturnOrginalGroupInformationDto OrgnlGrpInfAndSts { get; set; }
        public RefundTransactionInformationResponseDto TxInf { get; set; }
        public SupplementaryDataResponseDto SplmtryData { get; set; }
    }
    public class ReturnOrginalGroupInformationDto
    {
        [XmlElement("OrgnlMsgId")]
        public string OriginalMessageId { get; set; }
        [XmlElement("OrgnlMsgNmId")]
        public string OriginalMessageNameId { get; set; }
    }

    public class RefundTransactionInformationResponseDto
    {
        [XmlElement("RtrId")]
        public string ReturnIdentification { get; set; }
        [XmlElement("OrgnlTxId")]
        public string OriginalTransactionId { get; set; }
        [XmlElement("RtrdIntrBkSttlmAmt")]
        public decimal ReturnedInterBankSettlementAmount { get; set; }
        public ReturnReasonInformationDto RtrRsnInf { get; set; }
    }

    public class ReturnReasonInformationDto
    {
        public ReturnReasonDto Rsn { get; set; }
        [XmlElement("AddtlInf")]
        public string AdditionalInformation { get; set; }
    }

    public class ReturnReasonDto
    {
        [XmlElement("Prtry")]
        public string ReasonProprietary { get; set; }

    }
}
