using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.BaseRequests;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace APGDigitalIntegration.Common.CommonViewModels.Request;

public class DebitPaymentInternalRequest : IRequestBase, ICheckMerchantWithTerminalBase
{
    public long TerminalId { get; set; }
    public long MerchantId { get; set; }
    public string OrderKey { get; set; }
    public int TransactionMethodId { get; set; }
    public string MobileNumber { get; set; }
    [MaxLength(35)]
    public string AliasName { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public string UniqueNotificationId { get; set; }
    
    [Required]
    public RequestSources RequestSource { get; set; }
    public int PaymentViewType { get; set; }
    public Guid Id { get; set; }
        
    #region These  Fields Will be initialzed in CheckTerminalMerchant Filter Attribute.

    [BindNever, JsonIgnore] 
    public long MerchantRefId { get; set; }

    [BindNever, JsonIgnore] 
    public long TerminalNodeId { get; set; }

    [BindNever, JsonIgnore] 
    public int TerminalTypeId { get; set; }

    [BindNever, JsonIgnore] 
    public long BankId { get; set; }
    
    [BindNever, JsonIgnore] 
    public long? AggregatorId { get; set; }
    
    [BindNever, JsonIgnore] 
    public int SettAccType { get; set; }
    
    [BindNever,JsonIgnore]
    public long? MerchantBranchId { get; set; }

    #endregion
}

public class CreditPaymentInternalRequest
{
    public long TerminalId { get; set; }
    public long MerchantId { get; set; }
    public long WalletOrderId { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public long BankId { get; set; }
    public string OriginalMessageId { get; set; }
    public string UniqueIdentificationId { get; set; }
    public DateTimeOffset CreatedDatetime { get; set; }
    public int RequestSource { get; set; }
}

public class MPCSSInwardCreditPaymentInternalRequest : CreditPaymentInternalRequest
{
    public string InstructingAgentBICFI { get; set; }
    public string OriginalSessionSequence { get; set; }
}

public class WalletPaymentInternalResponse
{
    public WalletPaymentInternalResponse() => HostData = new Dictionary<string, string>();
    public Guid TransactionId { get; set; }
    public DateTimeOffset TransactionTime{ get; set; }
    public string TransactionTypeDisplayName{ get; set; }
    public int TransactionTypeId{ get; set; }
    public Dictionary<string, string> HostData { get; set; }
}

public class BaseInternalResponse
{
    public BaseInternalResponse() => HostData = new Dictionary<string, string>();
    public string ExternalMessageId { get; set; }
    public bool IsSuccess { get; set; }
    public string ResponseCode { get; set; }
    public string ResponseMessage { get; set; }
    public string OriginalExternalMessageId { get; set; }
    public Dictionary<string, string> HostData { get; set; }
    public static BaseInternalResponse GetTimeoutResponse(string originalExternalMsgId)
    {
        return new BaseInternalResponse()
        {
            ResponseCode = ResponseCodes.SystemTimeout,
            IsSuccess = false,
            ResponseMessage = "Timeout - Detected By System",
            OriginalExternalMessageId = originalExternalMsgId,
            HostData = new Dictionary<string, string>(),
            ExternalMessageId = string.Empty
        };
    }
}


public class TransactionStatus
{
    public string ResponseCode { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public List<string> ErrorList { get; set; }
    

    public static TransactionStatus Success()
    {
        return new TransactionStatus()
        {
            IsSuccess = true,
            ResponseCode = ResponseCodes.Success,
            Message = null,
            ErrorList = new List<string>()
        };
    }
    
    public static TransactionStatus Failure(string responseCode, string rejectionReason, List<string> errorList = null)
    {
        return new TransactionStatus
        {
            IsSuccess = false,
            ResponseCode = responseCode,
            Message = rejectionReason,
            ErrorList = errorList ?? new List<string>()
        };
    }
    
    
}