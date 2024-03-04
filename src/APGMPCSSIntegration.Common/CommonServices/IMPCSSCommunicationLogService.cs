using APG.MessageQueue.Contracts.CommunicationLog;
using APGMPCSSIntegration.Constant;
using Envelope = APGDigitalIntegration.Common.CommonViewModels.Common.Envelope;


namespace APGDigitalIntegration.Common.CommonServices
{
    public interface IMPCSSCommunicationLogService
    {
        public AddDigitalCommunicationLog MPCSSCommunicationLogModel { get; }
        void SetInternalRequest(object internalRequest);
        void SetInternalResponse(object internalResponse);
        void SetExternalResponse(Envelope externalResponse);
        void SetExternalResponseObj(object externalResponse);
        void SetExternalRequest(Envelope externalRequest);
        void SetRequestDatetime(DateTime requestDatetime);
        Task SetInternalRequestTime();
        Task SetInternalResponseTime();
        Task SetExternalRequestTime();
        Task SetExternalResponseTime();
        void SetMerchantRefId(long merchantRefId);
        void SetTerminalNodeId(long nodeId);
        bool CommunicationLogEnabled { get; set; }
        void SetExceptionId(string exceptionLogId);
        void MarkValidationsAsPassed();
        Task Log();
        void SetMsgId(string msgId);
         void SetCorrelationId(string correlationId);
        void SetTransactionTypeId(TransactionType transactionType);
        void SetRequstTypeId(int  mpcssRecordRequest);

    }
}
