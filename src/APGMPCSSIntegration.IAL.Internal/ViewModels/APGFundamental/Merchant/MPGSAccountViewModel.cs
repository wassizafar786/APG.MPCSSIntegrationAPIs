using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class MPGSAccountViewModel
    {
        [Key]
        public long IdN { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The UserName is Required")]
        [DisplayName("UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The Password is Required")]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The URL is Required")]
        [DisplayName("URL")]
        public string URL { get; set; }

        [Required(ErrorMessage = "The MerchantName is Required")]
        [DisplayName("MerchantName")]
        public string MerchantName { get; set; }

        [Required(ErrorMessage = "The MerchantRefId is Required")]
        [DisplayName("MerchantRefId")]
        public long MerchantRefId { get; set; }

        [DisplayName("IsDeleted")]
        public bool? IsDeleted { get; set; }
    }
}
