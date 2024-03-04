using APGDigitalIntegration.Common.CommonViewModels.Payment;
using APGDigitalIntegration.Common.CommonViewModels.Payment_New;

namespace APGMPCSSIntegration.Common.CommonViewModels.Payment
{
    public class PaymentRequestDto
    {
        public CustomerCreditTransferDto FIToFICstmrCdtTrf { get; set; }
        public CustomerDirectDebitTransferDto FIToFICstmrDrctDbt { get; set; }
        public PaymentStatusReportDto FIToFIPmtStsRpt { get; set; }
        public PaymentStatusRequestDto FIToFIPmtStsReq { get; set; }
    }

    public class CustomerCreditTransferDto
    {
        public GroupHeaderDto GrpHdr { get; set; }
        public CreditTransferTransactionDto CdtTrfTxInf { get; set; }
        public SupplementaryDataDto SplmtryData { get; set; }
    }

    public class CustomerDirectDebitTransferDto
    {
        public GroupHeaderDto GrpHdr { get; set; }
        public CustomerDebitTransactionDto DrctDbtTxInf { get; set; }
        public SupplementaryDataDto SplmtryData { get; set; }
    }

    public class PaymentStatusReportDto
    {
        public GroupHeaderDto GrpHdr { get; set; }
        public OriginalGroupStatusInformationDto OrgnlGrpInfAndSts { get; set; }
        public SupplementaryDataDto SplmtryData { get; set; }
    }

    public class PaymentStatusRequestDto
    {
        public GroupHeaderDto GrpHdr { get; set; }
        public OriginalGroupInformation OrgnlGrpInf { get; set; }
        public TransactionInformationDto TxInf { get; set; }
    }
    
}
