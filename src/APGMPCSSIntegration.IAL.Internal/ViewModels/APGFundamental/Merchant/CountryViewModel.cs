using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class CountryViewModel
    {
        [Key]
        public long IdN { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The ISO is Required")]
        [MinLength(2)]
        [MaxLength(2)]
        [DisplayName("ISO")]
        public string ISO { get; set; }

        [Required(ErrorMessage = "The Name is Required")]
        [MinLength(2)]
        [MaxLength(50)]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Name is Required")]
        [MinLength(2)]
        [MaxLength(5)]
        [DisplayName("Phone Code")]
        public string PhoneCode { get; set; }
    }
}
