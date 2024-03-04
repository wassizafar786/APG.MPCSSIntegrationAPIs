using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class MerchantDataViewModel
    {
        [Key]
        public long IdN { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The MerchantIdIdentification Id is Required")]
        [DisplayName("MerchantIdIdentification")]
        public long MerchantId { get; set; }

        [DisplayName("AcquirerMerchantId")]
        public long? AcquirerMerchantId { get; set; }

        [Required(ErrorMessage = "The MerchantName is Required")]
        [DisplayName("MerchantName")]
        public string MerchantName { get; set; }

        [Required(ErrorMessage = "The MerchantName_Ar is Required")]
        [DisplayName("MerchantName_Ar")]
        public string MerchantName_Ar { get; set; }

        [Required(ErrorMessage = "The Address1 is Required")]
        [DisplayName("Address1")]
        public string Address1 { get; set; }

        [DisplayName("Address2")]
        public string Address2 { get; set; }

        [DisplayName("Latitude")]
        public float? Latitude { get; set; }

        [DisplayName("Longtiude")]
        public float? Longtiude { get; set; }

        [DisplayName("ContactRefNo")]
        public string ContactRefNo { get; set; }

        [DisplayName("ContactPersonName")]
        public string ContactPersonName { get; set; }

        [DisplayName("ContactPersonMobile")]
        public string ContactPersonMobile { get; set; }

        [DisplayName("ContactPersonNationalId")]
        public string ContactPersonNationalId { get; set; }
        public string SecureHashKey { get; set; }
    }
}
