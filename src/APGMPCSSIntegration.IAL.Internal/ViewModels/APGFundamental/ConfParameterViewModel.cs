using System;

namespace APGMPCSSIntegration.IAL.Internal.Viewmodel.APGFundamentals
{
    public class ConfParameterViewModel
    {
        public long IdN { get; set; }
        
        public Guid Id { get; set; }

        public string ParamKey { get; set; }

        public string ParamValue { get; set; }

        public bool IsSystem { get; set; }

        public bool IsEncrypted { get; set; }

        public bool IsDeleted { get; set; }

        public long? BankId { get; set; }
    }
}
