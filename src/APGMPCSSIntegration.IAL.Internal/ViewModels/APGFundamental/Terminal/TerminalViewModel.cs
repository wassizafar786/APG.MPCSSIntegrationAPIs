using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Terminal
{
    public class TerminalViewModel
    {
        [Key]
        public long NodeId { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The BankId is Required")]
        [DisplayName("BankId")]
        public long BankId { get; set; }

        [Required(ErrorMessage = "The TerminalId is Required")]
        [DisplayName("TerminalId")]
        public long TerminalId { get; set; }

        [DisplayName("IsUpdated")]
        public bool? IsUpdated { get; set; }

        [DisplayName("LastUpdateDate")]
        public DateTime? LastUpdateDate { get; set; }

        [DisplayName("IsUpdateNow")]
        public bool? IsUpdateNow { get; set; }

        [DisplayName("UpdateStartDate")]
        public DateTime? UpdateStartDate { get; set; }

        [DisplayName("UpdateTimeFrom")]
        public TimeSpan? UpdateTimeFrom { get; set; }

        [DisplayName("UpdateTimeTo")]
        public TimeSpan? UpdateTimeTo { get; set; }

        [Required(ErrorMessage = "The Status is Required")]
        [DisplayName("Status")]
        public int Status { get; set; }

        [Required(ErrorMessage = "The AllowRefund is Required")]
        [DisplayName("AllowRefund")]
        public bool AllowRefund { get; set; }

        [Required(ErrorMessage = "The DeployDate is Required")]
        [DisplayName("DeployDate")]
        public DateTime DeployDate { get; set; }

        [Required(ErrorMessage = "The Name is Required")]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("IsDeleted")]
        public bool? IsDeleted { get; set; }

        [Required(ErrorMessage = "The IsCard is Required")]
        [DisplayName("IsCard")]
        public bool IsCard { get; set; }

        [Required(ErrorMessage = "The IsOmanNetDigital is Required")]
        [DisplayName("IsOmanNetDigital")]
        public bool IsOmanNetDigital { get; set; }

        [DisplayName("Latitude")]
        public float? Latitude { get; set; }

        [DisplayName("Longtiude")]
        public float? Longtiude { get; set; }

        [Required(ErrorMessage = "The TerminalTypeId is Required")]
        [DisplayName("TerminalTypeId")]
        public long TerminalTypeId { get; set; }

        [DisplayName("ApplicationVersionId")]
        public long? ApplicationVersionId { get; set; }

        [DisplayName("ConfigurationProfileId")]
        public long? ConfigurationProfileId { get; set; }

        [DisplayName("TerminalTemplateId")]
        public long? TerminalTemplateId { get; set; }
    }
}
