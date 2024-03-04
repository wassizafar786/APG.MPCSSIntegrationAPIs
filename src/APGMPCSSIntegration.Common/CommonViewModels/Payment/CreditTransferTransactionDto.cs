using System;
using System.Collections.Generic;
using System.Text;

namespace APGMPCSSIntegration.Common.CommonViewModels.Payment
{
    public class CreditTransferTransactionDto
    {
        public PaymentIdentification PmtId { get; set; }
        public string IntrBkSttlmAmt { get; set; }
        public string ChrgBr { get; set; }
        public Debtor Dbtr { get; set; }
        public DebtorAccount DbtrAcct { get; set; }
        public DebtorAgent DbtrAgt { get; set; }
        public CreditorAgent CdtrAgt { get; set; }
        public Creditor Cdtr { get; set; }

        public CreditorAccount CdtrAcct { get; set; }
    }
    public class Creditor
    {
        public string Nm { get; set; }
        public Identification Id { get; set; }
    }
    public class CreditorAgent
    {
        public FinancialInstitutionIdentification FinInstnId { get; set; }
    }
    public class CreditorAccount
    {
        public Identification Id { get; set; }
    }

    public class Debtor
    {
        public string Nm { get; set; }
        public Identification Id { get; set; }
    }
    public class DebtorAgent
    {
        public FinancialInstitutionIdentification FinInstnId { get; set; }
    }
    public class DebtorAccount
    {
        public Identification Id { get; set; }
    }

    public class PaymentIdentification
    {
        public string EndToEndId  { get; set; }
        public string TxId { get; set; }
    }
    public class OtherIdentification
    {
        public string Id { get; set; }
        public SchemeName SchmeNm { get; set; }

        public string Issr { get; set; }

    }
    public class Identification
    {
        public OtherIdentification Othr { get; set; }
        public PrivateIdentification PrvtId { get; set; }
    }
    
    public class PrivateIdentification
    {
        public OtherIdentification Othr { get; set; }
    }
    public class SchemeName
    {
        public string Prtry { get; set; }
    }
}
