using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class MerchantMDRViewModel
    {
        [Key]
        public long IdN { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The MDRType is Required")]
        [DisplayName("MDRType")]
        public int MDRType { get; set; }

        [Required(ErrorMessage = "The TerminalTypeId is Required")]
        [DisplayName("TerminalTypeId")]
        public long TerminalTypeId { get; set; }

        [Required(ErrorMessage = "The MerchantSettlementDataId is Required")]
        [DisplayName("MerchantSettlementDataId")]
        public long MerchantSettlementDataId { get; set; }

        [Required(ErrorMessage = "The MDRPercentage is Required")]
        [DisplayName("MDRPercentage")]
        public decimal MDRPercentage { get; set; }

        [Required(ErrorMessage = "The MinMDRLimit is Required")]
        [DisplayName("MinMDRLimit")]
        public decimal MinMDRLimit { get; set; }

        [Required(ErrorMessage = "The MaxMDRLimit is Required")]
        [DisplayName("MaxMDRLimit")]
        public decimal MaxMDRLimit { get; set; }

        [Required(ErrorMessage = "The MDRFlat is Required")]
        [DisplayName("MDRFlat")]
        public decimal MDRFlat { get; set; }
    }
}
