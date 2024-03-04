using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.IAL.Internal.Models.APGMembership
{
    public  class CheckPasswordRequestViewModel
    {
        public string Password { get; set; }
        public int RequestSourceId { get; set; }
    }
}
