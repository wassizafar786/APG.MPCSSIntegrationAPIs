﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class MiGSAccountViewModel
    {
        [Key]
        public long IdN { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The MerchantIdIdentification is Required")]
        [DisplayName("MerchantIdIdentification")]
        public string MerchantId { get; set; }

        [Required(ErrorMessage = "The User is Required")]
        [DisplayName("User")]
        public string User { get; set; }

        [Required(ErrorMessage = "The Password is Required")]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The AccessCode is Required")]
        [DisplayName("AccessCode")]
        public string AccessCode { get; set; }

        [Required(ErrorMessage = "The SecretHashKey is Required")]
        [DisplayName("SecretHashKey")]
        public string SecretHashKey { get; set; }

        [Required(ErrorMessage = "The MerchantRefId is Required")]
        [DisplayName("MerchantRefId")]
        public long MerchantRefId { get; set; }

        [DisplayName("IsDeleted")]
        public bool? IsDeleted { get; set; }
    }
}
