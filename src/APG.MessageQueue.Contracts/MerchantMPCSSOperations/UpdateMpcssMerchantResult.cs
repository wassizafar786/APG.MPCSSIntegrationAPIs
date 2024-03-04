
namespace APG.MessageQueue.Contracts.MerchantMPCSSOperations;

public class MpcssMerchantUpdateResult
{
    public string MsgId { get; set; }
    public string OriginalMsgId { get; set; }
    public int RequestType { get; set; }
    public int Status { get; set; }
    public string ErrorCode { get; set; }   
    public string ErrorMessage { get; set; }
    public string CorrelationId { get; set; }
}