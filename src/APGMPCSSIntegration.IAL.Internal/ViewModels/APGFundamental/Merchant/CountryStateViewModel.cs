using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class CountryStateViewModel
    {
        [Key]
        public long IdN { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The Name is Required")]
        [MinLength(2)]
        [MaxLength(50)]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Country ID is Required")]
        [DisplayName("Country ID")]
        public long CountryId { get; set; }
    }
}
