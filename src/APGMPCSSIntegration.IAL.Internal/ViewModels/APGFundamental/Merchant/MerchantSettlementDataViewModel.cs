using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class MerchantSettlementDataViewModel
    {
        [Key]
        public long IdN { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The SettAccType Id is Required")]
        [DisplayName("SettAccType")]
        public int SettAccType { get; set; }

        [DisplayName("SettBankAccName")]
        public string SettBankAccName { get; set; }

        [DisplayName("SettBankAccNumber")]
        public string SettBankAccNumber { get; set; }

        [Required(ErrorMessage = "The SettBankSwiftCode is Required")]
        [DisplayName("SettBankSwiftCode")]
        public string SettBankSwiftCode { get; set; }

        [DisplayName("SettCurrencyId")]
        public int? SettCurrencyId { get; set; }

        [DisplayName("SettCountryId")]
        public int? SettCountryId { get; set; }

        public ICollection<MerchantMDRViewModel> MerchantMDRs { get; set; }
    }
}
