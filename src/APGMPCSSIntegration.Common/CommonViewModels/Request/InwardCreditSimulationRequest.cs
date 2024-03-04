using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Common.CommonViewModels.Request
{
    public class InwardCreditSimulationRequest
    {
        public string ISOMessage { get; set; }

        public decimal Amount { get; set; }

        public int Currency { get; set; }
    }

    public class QROrderSimulationRequest
    {
        public long WallerOrderId { get; set; }
        
        public string InstructingBICFI { get; set; }
    }
}
