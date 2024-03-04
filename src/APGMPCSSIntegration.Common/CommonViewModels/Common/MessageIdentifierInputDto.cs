using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace APGMPCSSIntegration.Common.CommonViewModels.Common
{
    public class MessageIdentifierInputDto
    {
        /*Common Parameters*/
        [MaxLength(35, ErrorMessage = "Maximum field lenght is 35")]
        [Required]
        public string MessageIdentificationCode { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [MaxLength(100, ErrorMessage = "Maximum field lenght is 100")]
        public string AdditionalInformation { get; set; }
    }
}
