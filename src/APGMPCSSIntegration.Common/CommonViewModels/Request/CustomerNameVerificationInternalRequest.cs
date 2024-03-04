using APGMPCSSIntegration.IAL.Internal.BaseRequests;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APGDigitalIntegration.Common.CommonViewModels.Request;

public class CustomerNameVerificationRequest : IRequestBase, ICheckMerchantWithTerminalBase
{
    public string MobileNumber { get; set; }
    public string Alias { get; set; }

    [Required] 
    public string UniqueNotificationId { get; set; }
    public long TerminalId { get; set; }
    public long MerchantId { get; set; }

    [Required] 
    public int RequestSource { get; set; }

    #region These  Fields Will be initialzed in CheckTerminalMerchant Filter Attribute.

    [BindNever, JsonIgnore] 
    public long MerchantRefId { get; set; }

    [BindNever, JsonIgnore]
    public long TerminalNodeId { get; set; }

    [BindNever, JsonIgnore]
    public int TerminalTypeId { get; set; }
    [JsonIgnore] 
    public long? MerchantBranchId { get; set; }

    [BindNever, JsonIgnore]
    public long BankId { get; set; }
    [JsonIgnore] 
    public long? AggregatorId { get; set; }
    [JsonIgnore]
    public int SettAccType { get; set; }

    #endregion
}

public class CustomerNameVerificationInternalResponse
{
    public string CustomerName { get; set; }
    public string CustomerType { get; set; }
}