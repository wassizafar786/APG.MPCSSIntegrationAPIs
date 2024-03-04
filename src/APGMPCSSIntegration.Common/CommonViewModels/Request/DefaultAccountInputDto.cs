using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace APGMPCSSIntegration.Common.CommonViewModels.Request
{
    public class DefaultAccountInputDto
    {
        [MaxLength(35, ErrorMessage = "Maximum field lenght is 35")]
        [Required]
        public string MessageIdentificationCode { get; set; }

        public string AccountType { get; set; }
        public string RegistrationCode { get; set; }
        public string MobileNumber { get; set; }
       
    }
}
