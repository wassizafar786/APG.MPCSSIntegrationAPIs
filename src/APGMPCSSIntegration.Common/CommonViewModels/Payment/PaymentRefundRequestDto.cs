using System;
using System.Collections.Generic;
using System.Text;
using APGDigitalIntegration.Common.CommonViewModels.Response;

namespace APGMPCSSIntegration.Common.CommonViewModels.Payment
{
    public class PaymentRefundRequestDto
    {
        public string OrgnlMsgId { get; set; }
        public string OrgnlMsgNmId { get; set; }
    }


    public class ReturnReasonInformationDto
    {
        public ReturnReasonDto Rsn { get; set; }
        public string AddtlInf { get; set; }
    }

    public class ReturnReasonDto
    {
        public string Prtry { get; set; }
        
    }
}
