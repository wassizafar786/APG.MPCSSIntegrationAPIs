using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class MerchantCategoryCodeViewModel
    {
        [Key]
        public long IdN { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The Category Code is Required")]
        [MinLength(2)]
        [MaxLength(10)]
        [DisplayName("CategoryCode")]
        public string CategoryCode { get; set; }

        [Required(ErrorMessage = "The Category Name is Required")]
        [MinLength(2)]
        [MaxLength(150)]
        [DisplayName("Category Name")]
        public string CategoryName { get; set; }
    }
}
