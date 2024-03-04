namespace APGMPCSSIntegration.IAL.Internal.Viewmodel.APGFundamentals.Bank;

public class BankViewModel
{

    public long IdN { get; set; }
        
    public Guid Id { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }
    
    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }
    public BankConfigurationViewModel BankConfiguration { get; set; }
}

public class BankConfigurationViewModel
{

    public long BankId { get; set; }

    public bool IsPOS { get; set; }
    
    public bool IsPOS_ICS { get; set; }
    
    public bool IsPOS_OmanNet { get; set; }

    public bool IsEcommerce { get; set; }

    public bool IsEcommerce_ICS { get; set; }
    
    public bool IsEcommerce_OmanNet { get; set; }

    public bool IsMPCSS { get; set; }
    public float TimeZoneOffset { get; set; }
    public string BankIdentifierCode { get; set; }

    public byte[] Logo { get; set; }

    public byte[] MonoLogo { get; set; }

    public string Logo_Base64 { get; set; }

    public bool IsDeleted { get; set; }
}
