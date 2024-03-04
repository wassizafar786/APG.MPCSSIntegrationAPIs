using System;
using System.Collections.Generic;
using System.Text;
using APGDigitalIntegration.Common.CommonViewModels.Payment;

namespace APGMPCSSIntegration.Common.CommonViewModels.Payment
{
    public class OriginalGroupStatusInformationDto
    {
        public string OrgnlMsgId { get; set; }
        public string OrgnlMsgNmId { get; set; }
        public string GrpSts { get; set; }
        public StatusInformationDto StsRsnInf { get; set; }
    }

    public class StatusInformationDto
    {
        public ReasonInformationDto Rsn { get; set; }
    }

    public class ReasonInformationDto
    {
        public string Prtry { get; set; }
    }



    public class TransactionInformationDto
    {
        public string OrgnlEndToEndId { get; set; }
        public SupplementaryDataDto SplmtryData { get; set; }

    }

}
