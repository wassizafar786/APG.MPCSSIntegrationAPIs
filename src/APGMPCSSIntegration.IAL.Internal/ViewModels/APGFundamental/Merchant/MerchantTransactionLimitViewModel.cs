using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.Merchant
{
    public class MerchantTransactionLimitViewModel
    {
        [Key]
        public long IdN { get; set; }
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The MinTxnLimit Id is Required")]
        [DisplayName("MinTxnLimit")]
        public decimal MinTxnLimit { get; set; }

        [Required(ErrorMessage = "The MaxTxnLimit is Required")]
        [DisplayName("MaxTxnLimit")]
        public decimal MaxTxnLimit { get; set; }

        [Required(ErrorMessage = "The MaxTxnCountPerDay is Required")]
        [DisplayName("MaxTxnCountPerDay")]
        public int MaxTxnCountPerDay { get; set; }

        [Required(ErrorMessage = "The TxnOperationHourFrom is Required")]
        [DisplayName("TxnOperationHourFrom")]
        public int TxnOperationHourFrom { get; set; }

        [Required(ErrorMessage = "The TxnOperationHourTo is Required")]
        [DisplayName("TxnOperationHourTo")]
        public int TxnOperationHourTo { get; set; }

        [Required(ErrorMessage = "The DailyLimitAmount is Required")]
        [DisplayName("DailyLimitAmount")]
        public decimal DailyLimitAmount { get; set; }

        [Required(ErrorMessage = "The MonthlyLimitAmount is Required")]
        [DisplayName("MonthlyLimitAmount")]
        public decimal MonthlyLimitAmount { get; set; }

        [Required(ErrorMessage = "The MaxLimitAggregator is Required")]
        [DisplayName("MaxLimitAggregator")]
        public decimal MaxLimitAggregator { get; set; }
    }
}
