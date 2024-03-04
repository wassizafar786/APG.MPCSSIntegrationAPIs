using APGMPCSSIntegration.Common.CommonViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace APGMPCSSIntegration.Common.CommonViewModels.Request
{
    public class RegistrationAccountInputDto
    {
        /*Identification reference of the message at the PSP*/
        public MessageIdentifierInputDto MessageIdentifier { get; set; }

        /*Customer Information*/
        public string CustomerId { get; set; } /*Eg: Passport Number of customer*/

        public string IdentificationTypeCode { get; set; }

        public string IdIssuingCountryCode { get; set; }


        /*Customer Account Information */
        [Required]
        public string MobileOrMerchantId { get; set; }

        [MaxLength(1, ErrorMessage = "Maximum field lenght is 1")]
        [Required]
        public string AccountType { get; set; }  /*M: mobile number C: Merchant Id*/
        
        [MaxLength(10, ErrorMessage = "Maximum field lenght is 10")]
        [Required]
        public string RegistrationCode { get; set; }
        public string IsAccountBanked { get; set; }

        [MaxLength(18, ErrorMessage = "Maximum field lenght is 18")]
        public string AccountCurrency { get; set; }
        public string AccountAlias { get; set; }
        public string IsDefaultAccount { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string AdditionalInfo { get; set; }
        public string BankId { get; set; }
        public string ParticipantId { get; set; }
        
        public string MPCSSPSPRouteCode { get; set; }
    }
}
