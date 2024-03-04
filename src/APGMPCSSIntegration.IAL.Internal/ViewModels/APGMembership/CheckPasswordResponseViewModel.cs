using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.IAL.Internal.Models.APGMembership
{
    public class CheckPasswordResponseViewModel
    {
        public bool? IsCorrectPassword { get; set; }
        public bool? IsLockedOutUser { get; set; }
        public bool? IsInActiveUser { get; set; }
        public bool? IsDeletedUser { get; set; }
        public bool? IsExceededTrialLimit { get; set; }

    }
}
