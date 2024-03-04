using APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant;
using APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Terminal;
using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;

public class WalletOrderModel
{
    public long IdN { get; set; }
    public int Currency { get; set; }
    public decimal Amount { get; set; }
    public long MerchantRefId { get; set; }
        
    public long NodeId { get; set; }
    
    public long MerchantId { get; set; }
    public MerchantViewModel Merchant { get; set; }
    public TerminalViewModel Terminal { get; set; }
    public string SessionId { get; set; }
    public RequestSources RequestSourceId { get; set; }
}