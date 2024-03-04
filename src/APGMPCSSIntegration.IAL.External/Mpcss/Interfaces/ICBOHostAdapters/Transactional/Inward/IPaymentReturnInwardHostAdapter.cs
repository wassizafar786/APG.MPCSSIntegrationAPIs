using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APGDigitalIntegration.Common.CommonViewModels.Response;

namespace APGDigitalIntegration.IAL.External.Mpcss.Interfaces.ICBOHostAdapters
{
    public interface IPaymentReturnInwardHostAdapter
    {
        public Task<ServiceResponse> Execute(PaymentReturnResponseDto baseInternalRequest, MPCSSRecordRequest mpcssMessageType);
    }
}
