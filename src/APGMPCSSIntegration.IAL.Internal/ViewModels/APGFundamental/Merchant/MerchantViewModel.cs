using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class MerchantViewModel
    {
        [Key]
        public long MerchantRefId { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The BankId is Required")]
        [DisplayName("BankId")]
        public long BankId { get; set; }

        [DisplayName("ParentMerchantId")]
        public long? ParentMerchantId { get; set; }

        [Required(ErrorMessage = "The MerchantCategoryCodeId is Required")]
        [DisplayName("MerchantCategoryCodeId")]
        public long MerchantCategoryCodeId { get; set; }

        [Required(ErrorMessage = "The CountryId is Required")]
        [DisplayName("CountryId")]
        public long CountryId { get; set; }

        [Required(ErrorMessage = "The StateId is Required")]
        [DisplayName("StateId")]
        public long StateId { get; set; }

        public MerchantTransactionLimitViewModel MerchantTransactionLimit { get; set; }
        [Required(ErrorMessage = "The MerchantTransactionLimitId is Required")]
        [DisplayName("MerchantTransactionLimitId")]
        public long MerchantTransactionLimitId { get; set; }

        public MerchantDataViewModel MerchantData { get; set; }
        [Required(ErrorMessage = "The MerchantDataId is Required")]
        [DisplayName("MerchantDataId")]
        public long MerchantDataId { get; set; }

        public MerchantSettlementDataViewModel MerchantSettlement { get; set; }
        [Required(ErrorMessage = "The MerchantSettlementDataId is Required")]
        [DisplayName("MerchantSettlementDataId")]
        public long MerchantSettlementDataId { get; set; }

        [Required(ErrorMessage = "The MerchantCanRefund is Required")]
        [DisplayName("MerchantCanRefund")]
        public bool MerchantCanRefund { get; set; }

        [Required(ErrorMessage = "The IsAggregator is Required")]
        [DisplayName("IsAggregator")]
        public bool IsAggregator { get; set; }

        [DisplayName("AggregatorId")]
        public long? AggregatorId { get; set; }

        public ICollection<MiGSAccountViewModel> MiGSAccounts { get; set; }
        [Required(ErrorMessage = "The IsMigs is Required")]
        [DisplayName("IsMigs")]
        public bool IsMigs { get; set; }

        public ICollection<MPGSAccountViewModel> MPGSAccounts { get; set; }
        [Required(ErrorMessage = "The IsMPGS is Required")]
        [DisplayName("IsMPGS")]
        public bool IsMPGS { get; set; }

        public ICollection<OmanNetCardAccountViewModel> OmanNetCardAccounts { get; set; }
        [Required(ErrorMessage = "The IsOmanNet is Required")]
        [DisplayName("IsOmanNet")]
        public bool IsOmanNet { get; set; }

        [Required(ErrorMessage = "The IsOmanNetDigital is Required")]
        [DisplayName("IsOmanNetDigital")]
        public bool IsOmanNetDigital { get; set; }

        [DisplayName("IsMerchant3DS")]
        public bool IsMerchant3DS { get; set; }

        [Required(ErrorMessage = "The IsEcommerce is Required")]
        [DisplayName("IsEcommerce")]
        public bool IsEcommerce { get; set; }

        public ICollection<MerchantDocumentViewModel> MerchantDocuments { get; set; }
        [Required(ErrorMessage = "The IsDocument is Required")]
        [DisplayName("IsDocument")]
        public bool IsDocument { get; set; }

        [Required(ErrorMessage = "The IsActive is Required")]
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }

        [DisplayName("IsDeleted")]
        public bool? IsDeleted { get; set; }
    }
}
