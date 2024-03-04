using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Constant
{
    public static class MerchantTerminalInfo
    {
        public const string TerminalTypeId = "TerminalTypeId";
        public const string AggregatorId = "AggregatorId";
        public const string MerchantRefId = "MerchantRefId";
        public const string SettAccType = "SettAccType";
        public const string BankId = "BankId";
        public const string TerminalNodeId = "TerminalNodeId";
    }

    public static class DigitalTransactionInfo
    {
        public const string MessageIdentificationCode = "MessageIdentificationCode";
        public const string TotalInterbankSettlementAmount = "TotalInterbankSettlementAmount";
        public const string GroupHeader = "GrpHdr";
        public const string OriginalTransactionId = "OriginalTransactionId";


    }
}
