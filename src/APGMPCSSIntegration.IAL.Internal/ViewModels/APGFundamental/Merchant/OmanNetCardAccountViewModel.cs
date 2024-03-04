using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class OmanNetCardAccountViewModel
    {
        [Key]
        public long IdN { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The MerchantIdIdentification is Required")]
        [DisplayName("MerchantIdIdentification")]
        public string MerchantId { get; set; }

        [Required(ErrorMessage = "The MerchantRefId is Required")]
        [DisplayName("MerchantRefId")]
        public long MerchantRefId { get; set; }

        [DisplayName("UserName")]
        public string UserName { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("IsDeleted")]
        public bool? IsDeleted { get; set; }
    }
}
