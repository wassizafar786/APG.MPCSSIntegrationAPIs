using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel
{
    public class NoResponseMessageViewModel
    {
        public string MpcssEndToEndId { get; set; }
        public string OriginalMessageCreatedDateTime { get; set; }
        public string GroupMerchantId { get; set; }
        public string TerminalId { get; set; }
        public string Status { get; set; }
        public  string Amount { get; set; }
    }
}
