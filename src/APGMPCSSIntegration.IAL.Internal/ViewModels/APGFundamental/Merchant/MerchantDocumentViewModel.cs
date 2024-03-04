using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class MerchantDocumentViewModel
    {
        [Key]
        public long IdN { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The FilePath is Required")]
        [DisplayName("FilePath")]
        public string FilePath { get; set; }

        [Required(ErrorMessage = "The FileName is Required")]
        [DisplayName("FileName")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "The CreationDate is Required")]
        [DisplayName("CreationDate")]
        public DateTime CreationDate { get; set; }

        [DisplayName("UpdateDate")]
        public DateTime? UpdateDate { get; set; }

        [Required(ErrorMessage = "The MerchantRefId is Required")]
        [DisplayName("MerchantRefId")]
        public long MerchantRefId { get; set; }

        [DisplayName("IsDeleted")]
        public bool? IsDeleted { get; set; }
    }
}
