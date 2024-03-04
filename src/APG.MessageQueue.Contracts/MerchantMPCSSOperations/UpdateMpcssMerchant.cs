namespace APG.MessageQueue.Contracts.MerchantMPCSSOperations;

public class UpdateMpcssMerchant
{
    public string MsgId { get; set; }
    
    public string OriginalMsgId { get; set; }
    
    public int RequestType { get; set; }
    
    public MpcssMerchantModel Request { get; set; }
    public string CorrelationId { get; set; }
}
public class MpcssMerchantModel
{
    public long MerchantMPCSSAccountRefId { get; set; }
    public string IdentCode { get; set; } // Record  // Account
    public string IdentNumber { get; set; } // Record // Account
    public string IdentCountryCode { get; set; } // Record // Account
    public string MerchantId { get; set; }  // Account
    public string MerchantName { get; set; } // Record
    public string ParticipantId { get; set; } // Record
    public string RegistrationCode { get; set; } // Account
    public string AccountCurrency { get; set; } // Account
    public string IsAccountBanked { get; set; } // Account
    public string AccountAlias { get; set; } // Account

    //Address
    public string POBox { get; set; } // Record
    public string PostalCode { get; set; } // Record
    public string StreetName { get; set; } // Record
    public string BuildingNumber { get; set; } // Record
    public string PhoneNumber { get; set; } // Record
    public string MobileNumber { get; set; } // Record
    public string CityName { get; set; } // Record
    public string TownName { get; set; } // Record
    public string GovernorateName { get; set; } // Record
    public string CountryCode { get; set; } // Record
    public string AdditionalInfo { get; set; } // Record
    public long BankId { get; set; }
    public long MerchantRefId { get; set; }
}