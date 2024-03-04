using System.Text.Encodings.Web;
using System.Text.Json;
using APG.MessageQueue.Contracts.CommunicationLog;
using APGDigitalIntegration.Domain.Interfaces;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Constant;
using Envelope = APGDigitalIntegration.Common.CommonViewModels.Common.Envelope;


namespace APGDigitalIntegration.Common.CommonServices
{
    public class MPCSSCommunicationLogService : IMPCSSCommunicationLogService
    {
        private readonly ILoggingService _loggingService;
        private readonly IDateTimeProvider _dateTimeProvider;
        public AddDigitalCommunicationLog MPCSSCommunicationLogModel { get; private set; }
        public bool CommunicationLogEnabled { get; set; }
        public bool ValidationsPassed { get; set; }

        private static JsonSerializerOptions SerializerOptions
        {
            get
            {
                return new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };
            }
        }

        #region Constructor
        public MPCSSCommunicationLogService(ILoggingService loggingService, IDateTimeProvider dateTimeProvider)
        {
            _loggingService = loggingService;
            _dateTimeProvider = dateTimeProvider;
            this.MPCSSCommunicationLogModel = new AddDigitalCommunicationLog
            {
                ServiceName = MicroServicesName.APGDigitalIntegration
            };
        }

        #endregion

        public void MarkValidationsAsPassed() => ValidationsPassed = true;
        public void SetMsgId(string msgId) => MPCSSCommunicationLogModel.MsgId = msgId;
        public void SetCorrelationId(string correlationId) => MPCSSCommunicationLogModel.CorrelationId = correlationId;
        public void SetTransactionTypeId(TransactionType transactionType) => MPCSSCommunicationLogModel.TransactionTypeId = (int)transactionType;
        public void SetRequstTypeId(int mpcssRecordRequest) => MPCSSCommunicationLogModel.RequestTypeId = mpcssRecordRequest;

        public void SetInternalRequest(object internalRequest)
        {
            this.MPCSSCommunicationLogModel.InternalRequest = this.SerializeObject(internalRequest);
        }

        public void SetInternalResponse(object internalResponse)
        {
            this.MPCSSCommunicationLogModel.InternalResponse = this.SerializeObject(internalResponse);
        }

        public void SetExternalResponse(Envelope externalResponse)
        {
            externalResponse.ContentData = externalResponse.Content.Value;
            this.MPCSSCommunicationLogModel.ExternalResponse = this.SerializeObject(externalResponse);
        }

        public void SetExternalResponseObj(object externalResponse)
        {
            this.MPCSSCommunicationLogModel.ExternalResponse = this.SerializeObject(externalResponse);
        }

        public void SetExternalRequest(Envelope externalRequest)
        {
            externalRequest.ContentData = externalRequest.Content.Value;
            this.MPCSSCommunicationLogModel.ExternalRequest = this.SerializeObject(externalRequest);
        }


        public void SetRequestDatetime(DateTime requestDatetime)
        {
            this.MPCSSCommunicationLogModel.RequestDatetime = requestDatetime;
        }

        public async Task SetInternalRequestTime()
        {
            this.MPCSSCommunicationLogModel.InternalRequestTime = await _dateTimeProvider.SystemNow();
        }

        public async Task SetInternalResponseTime()
        {
            this.MPCSSCommunicationLogModel.InternalResponseTime = await _dateTimeProvider.SystemNow();
        }

        public async Task SetExternalRequestTime()
        {
            this.MPCSSCommunicationLogModel.ExternalRequestTime = await _dateTimeProvider.SystemNow();
        }

        public async Task SetExternalResponseTime()
        {
            this.MPCSSCommunicationLogModel.ExternalResponseTime = await _dateTimeProvider.SystemNow(); ; ;
        }

        public void SetMerchantRefId(long merchantRefId)
        {
            this.MPCSSCommunicationLogModel.MerchantRefId = merchantRefId;
        }

        public void SetTerminalNodeId(long nodeId)
        {
            this.MPCSSCommunicationLogModel.TerminalNodeId = nodeId;
        }

        public void SetExceptionId(string exceptionLogId)
        {
            this.MPCSSCommunicationLogModel.ExceptionLogId = exceptionLogId;
        }

        private string SerializeObject(object obj)
        {
            if (obj is null)
                return string.Empty;

            return obj is string ? obj.ToString() : JsonSerializer.Serialize(obj, SerializerOptions);
        }


        public async Task Log()
        {
            await _loggingService.LogMPCSSCommunicationLog(MPCSSCommunicationLogModel).ConfigureAwait(false);
        }
    }
}
