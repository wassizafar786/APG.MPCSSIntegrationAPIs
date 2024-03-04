using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.ViewModels.APGFundamental.Terminal
{
    public class CheckTerminalMerchantRequest
    {
        [Required]
        public long MerchantId { get; set; }

        [Required]
        public long TerminalId { get; set; }
        public long? MerchantRefId { get; set; }

        public static CheckTerminalMerchantRequest Create( long merchantId, long terminalId)
        {
            return new CheckTerminalMerchantRequest()
            {
                MerchantId = merchantId,
                TerminalId = terminalId,
               
            };
        }
    }
}