using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Common.CommonViewModels.Response;

namespace APGDigitalIntegration.IAL.External.Interfaces.ICBOHosts
{
    public interface IRecordVerificationHostAdapter
    {
        public Task<ServiceResponse> Execute(CustomerNameVerificationRequest baseInternalRequest);
    }

}
