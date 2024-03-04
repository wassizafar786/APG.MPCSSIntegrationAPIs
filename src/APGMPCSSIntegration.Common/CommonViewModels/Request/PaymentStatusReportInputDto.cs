using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace APGMPCSSIntegration.Common.CommonViewModels.Request
{
    public class PaymentStatusReportInputDto
    {
       // [MinLength(22, ErrorMessage = "Minimum field length is 22")]
        [MaxLength(35, ErrorMessage = "Maximum field lenght is 35")]
        [Required]
        public string MessageIdentificationCode { get; set; }
        
        //[RegularExpression(@"^[A-Z]{6,6}[A-Z2-9][A-NP-Z0-9]([A-Z0-9]{3,3}){0,1}+$", ErrorMessage = "Invalid Indentifier set")]
        public string InstructingAgentBICFI { get; set; }

        //[RegularExpression(@"^[A-Z]{6,6}[A-Z2-9][A-NP-Z0-9]([A-Z0-9]{3,3}){0,1}+$", ErrorMessage = "Invalid Indentifier set")]
        public string InstructedAgentBICFI { get; set; }

        [Required]
        [MaxLength(16, ErrorMessage = "Maximum field lenght is 16")]
        public string OriginalMessageId { get; set; }

        [Required]
        [MaxLength(35, ErrorMessage = "Maximum field lenght is 35")]
        public string OriginalMessageNameId { get; set; }
        public DateTime OriginalMessageCreatedDateTime { get; set; }

        [Required]
        public string GroupStatus { get; set; }

        [Required]
        [MaxLength(35, ErrorMessage = "Maximum field lenght is 35")]
        public string OriginalGroupStatusProprietary { get; set; }
        public string OriginalEndToEndId { get; set; }

        [MaxLength(100, ErrorMessage = "Maximum field lenght is 100")]
        public string AdditionalInformation { get; set; }

        [Required]
        [MaxLength(6, ErrorMessage = "Maximum field lenght is 6")]
        public string SessionSequence { get; set; }
        [Required]
        public string BatchSource { get; set; }
        public string ReceiverIdIssuingCountry { get; set; }
        public string ReceiverIdType { get; set; }
        public string ReceiverIdValue { get; set; }
        public string SupplementaryReceiverName { get; set; }
    }
}
