using APGMPCSSIntegration.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.CardBin
{
    public class CardBinConfigurationViewModel
    {
        
            public long IdN { get; set; }
            public string Bin { get; set; }
            public CardBrandType Brand { get; set; }
            public InstrumentType Instrument { get; set; }
            public bool IsDeleted { get; set; }
            public long BankId { get; set; }
        
    }
}
