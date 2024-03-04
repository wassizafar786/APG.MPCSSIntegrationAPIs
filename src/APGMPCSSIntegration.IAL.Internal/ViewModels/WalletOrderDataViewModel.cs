
using APGMPCSSIntegration.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction
{
    public class WalletOrderDataViewModel
    {
        public long IdN { get; set; }

        public int Currency { get; set; }
        public decimal Amount { get; set; }

        public long MerchantRefId { get; set; }

        public long NodeId { get; set; }
      
        public long MerchantId{ get; set; }
      
        public long TerminalId { get; set; }

        public string UniqueIdentificationId { get; set; }
        public string SessionId { get; set; }
        public RequestSources RequestSourceId { get; set; }
        public Guid Id { get; set; }
    }
}
