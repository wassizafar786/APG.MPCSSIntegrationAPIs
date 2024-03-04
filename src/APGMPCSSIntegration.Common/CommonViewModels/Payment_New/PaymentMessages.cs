
using System.Xml.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Payment_New;



[XmlType("TtlIntrBkSttlmAmt")]
public class TotalInterBankSettlementAmount
{
    [XmlAttribute(AttributeName = "Ccy")]
    public string Currency { get; set; }

    [XmlText]
    public string Value { get; set; }
}

[XmlType("SttlmInf")]
public class SettlementInformation
{
    [XmlElement(ElementName = "SttlmMtd")]
    public string SettlementMethod { get; set; }

    [XmlElement(ElementName = "ClrSys")]
    public ClearingSystem ClearingSystem { get; set; }
}

[XmlType("ClrSys")]
public class ClearingSystem
{
    [XmlElement(ElementName = "Prtry")]
    public string Proprietary { get; set; }
}

[XmlType("PmtTpInf")]
public class PaymentTypeInformation
{
    [XmlElement(ElementName = "LclInstrm")]
    public LocalInstrument LocalInstrument { get; set; }

    [XmlElement(ElementName = "CtgyPurp")]
    public CategoryPurpose CategoryPurpose { get; set; }
}

[XmlType("LclInstrm")]
public class LocalInstrument
{
    [XmlElement(ElementName = "Cd")]
    public string Code { get; set; }
}

[XmlType("CtgyPurp")]
public class CategoryPurpose
{
    [XmlElement(ElementName = "Prtry")]
    public string Proprietary { get; set; }
}

[XmlType("InstgAgt")]
public class InstructingAgent
{
    [XmlElement(ElementName = "FinInstnId")]
    public FinancialInstitutionIdentification FinancialInstitutionIdentification { get; set; }
}

[XmlType("InstdAgt")]
public class InstructedAgent
{
    [XmlElement(ElementName = "FinInstnId")]
    public FinancialInstitutionIdentification FinancialInstitutionIdentification { get; set; }
}


[XmlType("FinInstnId")]
public class FinancialInstitutionIdentification
{
    [XmlElement(ElementName = "BICFI")]
    public string BICFI { get; set; }
}


[XmlType("CdtTrfTxInf")]
public class CreditTransferTransactionInformation
{
    [XmlElement(ElementName = "PmtId")]
    public PaymentIdentification PaymentIdentification { get; set; }

    [XmlElement(ElementName = "IntrBkSttlmAmt")]
    public InterBankSettlementAmount InterbankSettlementAmount { get; set; }

    [XmlElement(ElementName = "ChrgBr")]
    public string ChargeBearer { get; set; }

    [XmlElement(ElementName = "Dbtr")]
    public Debtor Debtor { get; set; }

    [XmlElement(ElementName = "DbtrAcct")]
    public DebtorAccount DebtorAccount { get; set; }

    [XmlElement(ElementName = "DbtrAgt")]
    public DebtorAgent DebtorAgent { get; set; }

    [XmlElement(ElementName = "CdtrAgt")]
    public CreditorAgent CreditorAgent { get; set; }

    [XmlElement(ElementName = "Cdtr")]
    public Creditor Creditor { get; set; }

    [XmlElement(ElementName = "CdtrAcct")]
    public CreditorAccount CreditorAccount { get; set; }
}


[XmlType("DrctDbtTxInf")]
public class DebitTransferTransactionInformation
{
    [XmlElement(ElementName = "PmtId")]
    public PaymentIdentification PaymentIdentification { get; set; }

    [XmlElement(ElementName = "IntrBkSttlmAmt")]
    public InterBankSettlementAmount InterbankSettlementAmount { get; set; }

    [XmlElement(ElementName = "ChrgBr")]
    public string ChargeBearer { get; set; }

    [XmlElement(ElementName = "Dbtr")]
    public Debtor Debtor { get; set; }

    [XmlElement(ElementName = "DbtrAcct")]
    public DebtorAccount DebtorAccount { get; set; }

    [XmlElement(ElementName = "DbtrAgt")]
    public DebtorAgent DebtorAgent { get; set; }

    [XmlElement(ElementName = "CdtrAgt")]
    public CreditorAgent CreditorAgent { get; set; }

    [XmlElement(ElementName = "Cdtr")]
    public Creditor Creditor { get; set; }

    [XmlElement(ElementName = "CdtrAcct")]
    public CreditorAccount CreditorAccount { get; set; }
}

[XmlType("PmtId")]
public class PaymentIdentification
{
    [XmlElement(ElementName = "EndToEndId")]
    public string EndToEndId { get; set; }

    [XmlElement(ElementName = "TxId")]
    public string TransactionId { get; set; }
}

[XmlType("IntrBkSttlmAmt")]
public class InterBankSettlementAmount
{
    [XmlAttribute(AttributeName = "Ccy")]
    public string Currency { get; set; }

    [XmlText]
    public string Value { get; set; }
}



[XmlType("Dbtr")]
public class Debtor
{
    public string Nm { get; set; }
    public Identification Id { get; set; }
}

public class Identification
{
    public OtherIdentification Othr { get; set; }
    public PrivateIdentification PrvtId { get; set; }
}

public class OtherIdentification
{
    public string Id { get; set; }
    
    [XmlElement(ElementName = "SchmeNm")]
    public SchemeName SchemeName { get; set; }

    [XmlElement(ElementName = "Issr")]
    public string Issuer { get; set; }
}

public class PrivateIdentification
{
    [XmlElement(ElementName = "Othr")]
    public OtherIdentification Other { get; set; }
}
public class SchemeName
{
    [XmlElement(ElementName = "Prtry")]
    public string Proprietary { get; set; }
}


[XmlType("DbtrAcct")]
public class DebtorAccount
{
    [XmlElement(ElementName = "Id")]
    public Id Id { get; set; }
}

public class Id
{
    [XmlElement(ElementName = "Othr")]
    public Other Other { get; set; }
}

[XmlType("Othr")]
public class Other
{
    [XmlElement(ElementName = "Id")]
    public string Id { get; set; }
}

[XmlType("DbtrAgt")]
public class DebtorAgent
{
    [XmlElement(ElementName = "FinInstnId")]
    public FinancialInstitutionIdentification FinancialInstitutionIdentification { get; set; }
}


[XmlType("Cdtr")]
public class Creditor
{
    public string Nm { get; set; }
    public Identification Id { get; set; }
}

[XmlType("CdtrAgt")]
public class CreditorAgent
{
    [XmlElement(ElementName = "FinInstnId")]
    public FinancialInstitutionIdentification FinancialInstitutionIdentification { get; set; }
}

[XmlType("CdtrAcct")]
public class CreditorAccount
{
    [XmlElement(ElementName = "Id")]
    public Id Id { get; set; }
}


[XmlType("OrgnlGrpInf")]
public class OriginalGroupInformation
{
    public string OrgnlMsgId { get; set; }
    public string OrgnlMsgNmId { get; set; }
    public string OrgnlCreDtTm { get; set; }
}


public class ActiveAmountAndCurrency
{
    [XmlText]
    public decimal Amount { get; set; }
        
    [XmlAttribute("Ccy")]
    public string Currency { get; set; }
}
