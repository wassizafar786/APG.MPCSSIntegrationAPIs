using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class MerchantBranchViewModel
    {
        [Key]
        public long IdN { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The Code is Required")]
        [DisplayName("Code")]
        public string Code { get; set; }

        [Required(ErrorMessage = "The Name is Required")]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Address is Required")]
        [DisplayName("Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "The CanRefund is Required")]
        [DisplayName("CanRefund")]
        public bool CanRefund { get; set; }

        [DisplayName("IsDeleted")]
        public bool? IsDeleted { get; set; }

        [DisplayName("DeleteDate")]
        public DateTime? DeleteDate { get; set; }

        [Required(ErrorMessage = "The InsertionDate is Required")]
        [DisplayName("InsertionDate")]
        public DateTime InsertionDate { get; set; }

        [DisplayName("UpdateDate")]
        public DateTime? UpdateDate { get; set; }

        [Required(ErrorMessage = "The MerchantRefId is Required")]
        [DisplayName("MerchantRefId")]
        public long MerchantRefId { get; set; }
        [Required(ErrorMessage = "The IsActive is Required")]
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
    }
}
