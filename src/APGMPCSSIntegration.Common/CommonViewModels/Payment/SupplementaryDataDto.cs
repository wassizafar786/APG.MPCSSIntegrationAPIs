namespace APGDigitalIntegration.Common.CommonViewModels.Payment
{
    public class SupplementaryDataDto
    {
        public string PlcAndNm { get; set; }
        public Envelope Envlp { get; set; }
    }

    public class Envelope
    {
        public ACHSupplementaryData achSupplementaryData { get; set; }
    }
    public class ACHSupplementaryData
    {
        public string batchSource { get; set; }
        public string consumerID { get; set; }
        public string merchCategoryCd { get; set; }
        public string grpMerchId { get; set; }
        public string terminalId { get; set; }
        public string filler { get; set; }
        public string merchantName { get; set; }
        public string pntOfInitiateMethd { get; set; }
        public string MsgtipOrConvnceIndctrId { get; set; }
        public string feePercentage { get; set; }
        public string countryCd { get; set; }
        public string merchantCity { get; set; }
        public string postCd { get; set; }
        public string invoiceNumber { get; set; }
        public string sessionSequence { get; set; }
        public string receiverIdIssuingCountry { get; set; }
        public string receiverIdType { get; set; }
        public string receiverIdValue { get; set; }
        public string receiverName { get; set; }
    }
}
 