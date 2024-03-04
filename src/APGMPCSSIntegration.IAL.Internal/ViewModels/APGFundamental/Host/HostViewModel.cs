using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Host
{
    public class HostViewModel
    {
        [Key]
        public long IdN { get; set; }

        public Guid Id { get; set; }

        public int HostId { get; set; }

        public string HostName { get; set; }

        public string ServerIP1 { get; set; }

        public int ServerPort1 { get; set; }

        public string ServerIP2 { get; set; }

        public int? ServerPort2 { get; set; }

        public bool IsSSLEnabled { get; set; }

        public int ConnectionTimeout { get; set; }

        public int ReadTimeout { get; set; }

        public int WriteTimeout { get; set; }

        public string SSLCertFile { get; set; }

        public byte[] SSLCertContent { get; set; }

        public string SSLCertPassword { get; set; }

        public long? CommunicationMessageFormatId { get; set; }

        public long? CommunicationMessageTypeId { get; set; }

        public bool IsGateway { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public long? BankId { get; set; }

        public int? HostOrder { get; set; }
    }
}
