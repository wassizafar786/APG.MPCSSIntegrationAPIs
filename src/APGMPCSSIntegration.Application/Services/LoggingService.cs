using System;
using System.Threading;
using System.Threading.Tasks;
using APG.MessageQueue.Contracts.CommunicationLog;
using APG.MessageQueue.Contracts.Logs;
using APG.MessageQueue.Contracts.SystemCommunicationLog;
using APG.MessageQueue.Interfaces;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.DomainHelper;
using APGDigitalIntegration.DomainHelper.ViewModels;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Constant;

namespace APGDigitalIntegration.Application.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly IMessageQueue _messageQueue;
        private readonly IDateTimeProvider _dateTimeProvider;

        public LoggingService(IMessageQueue messageQueue, IDateTimeProvider dateTimeProvider)
        {
            _messageQueue = messageQueue;
            _dateTimeProvider = dateTimeProvider;
        }
        
        public async Task<Guid> HandleException(Exception ex)
        {
            var exceptionLogModel = new AddExceptionLog()
            {
                Id = Guid.NewGuid(),
                Message = ex.Message,
                Source = ex.Source,
                ExceptionServiceSource = MicroServicesName.APGDigitalIntegration,
                StackTrace = ex.StackTrace,
                InnerException = ex.InnerException?.Message,
                DateTime = DateTime.UtcNow,
                ExceptionType = ex.GetType().ToString()
            };
            await this.LogException(exceptionLogModel);
            return exceptionLogModel.Id;
        }
        public async Task LogException(AddExceptionLog exceptionLogViewModel)
        {
            await _messageQueue.PublishMessage(exceptionLogViewModel, CancellationToken.None);
        }
        public async Task LogMPCSSCommunicationLog(AddDigitalCommunicationLog mPcssCommunicationLogModel)
        {
            await _messageQueue.PublishMessage(mPcssCommunicationLogModel, CancellationToken.None);
        }

        public async Task LogSimulate(SimulateLogViewModel log)
        {
            await _messageQueue.PublishMessage(log, CancellationToken.None);
        }
        public void SearchSimulateLog(SimulateLogViewModel log)
        {
            _messageQueue.PublishMessage(log, CancellationToken.None);
        }
        
        public async Task LogException(Exception ex)
        {
            var exceptionLog = new AddExceptionLog()
            {
                Id = Guid.NewGuid(),
                Message = ex.Message,
                Source = ex.Source,
                ExceptionServiceSource = ExceptionServiceSource.APGMPCSSS,
                StackTrace = ex.StackTrace,
                InnerException = ex.InnerException?.Message,
                DateTime = await _dateTimeProvider.SystemNow(),
                ExceptionType = ex.GetType().ToString(),
            };

            await this.LogException(exceptionLog);
        }


        public async Task AddSystemCommunicationLog(AddSystemCommunicationLog systemCommunicationLog)
        {
            if (systemCommunicationLog == null)
                throw new ArgumentException($"{nameof(systemCommunicationLog)} Is Null");

            systemCommunicationLog.Request=systemCommunicationLog.Request.SerializeObject();
            systemCommunicationLog.Response=systemCommunicationLog.Response.SerializeObject();


            await _messageQueue.PublishMessage(systemCommunicationLog, CancellationToken.None)
                   .ConfigureAwait(false);

        }
    }
}