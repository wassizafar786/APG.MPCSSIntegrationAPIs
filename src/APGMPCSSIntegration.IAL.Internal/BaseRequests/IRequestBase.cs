using System;
using System.Text.Json.Serialization;

namespace APGMPCSSIntegration.IAL.Internal.BaseRequests
{
    public interface IRequestBase : ITerminalIdBase, IMerchantIdBase
    {
    }

    //public interface IRequestDateTimeBase
    //{
    //    public DateTime RequestDateTime { get; set; }
    //}

    public interface ITerminalIdBase
    {
        public long TerminalId { get; set; }
    }

    public interface IMerchantIdBase
    {
        public long MerchantId { get; set; }
    }    
    
    public interface ICheckMerchantWithTerminalBase
    {
        [JsonIgnore]
        public long MerchantRefId { get; set; }
        
        [JsonIgnore]
        public long TerminalNodeId { get; set; }
        
        [JsonIgnore]
        public int TerminalTypeId { get; set; }

        [JsonIgnore]
        long BankId { get; set; }
        [JsonIgnore]
        long? AggregatorId { get; set; }
        [JsonIgnore]
        int SettAccType { get; set; }
        [JsonIgnore]
        long? MerchantBranchId { get; set; }

    }
}