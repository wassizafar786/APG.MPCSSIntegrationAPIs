using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction
{
    public class DigitalTransactionEnquiryViewModel
    {
        public Guid Id { get; set; }
        public long IdN { get; set; }
        public string MpcssEndToEndTransctionId { get; set; }
        public string MpcssTransactionDetails { get; set; }
        public DateTime NextExecutionTime { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
