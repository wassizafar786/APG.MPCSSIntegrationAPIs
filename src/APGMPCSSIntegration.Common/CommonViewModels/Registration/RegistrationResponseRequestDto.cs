using System;

namespace APGMPCSSIntegration.Common.CommonViewModels.Registration
{
    public class RegistrationResponseRequestDto
    {
        public MessageIdentificationDto MsgId { get; set; }
        public OriginalMessageIdentificationDto OrgnlMsgId { get; set; }
        public OriginalMessageStatusDto OrgnlMsgSts { get; set; }

    }

    public class MessageIdentificationDto
    {
        public string Id { get; set; }
        public string CreDtTm { get; set; }
    }

    public class OriginalMessageStatusDto
    {
        public string Sts { get; set; }
        public string RsnCd { get; set; }
        public string Nrtn { get; set; }
    }

    public class OriginalMessageIdentificationDto
    {
        public string Id { get; set; }
        public string MsgTp { get; set; }
    }
}

