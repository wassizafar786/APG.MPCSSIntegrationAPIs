using System;
using System.Threading.Tasks;
using APG.MessageQueue.Contracts.CommunicationLog;
using APG.MessageQueue.Contracts.Logs;
using APG.MessageQueue.Contracts.SystemCommunicationLog;
using APGDigitalIntegration.DomainHelper.ViewModels;

namespace APGDigitalIntegration.Domain.Interfaces
{
    public interface ILoggingService
    {
        Task<Guid> HandleException(Exception ex);
        Task LogException(AddExceptionLog exceptionLogViewModel);
        Task LogMPCSSCommunicationLog(AddDigitalCommunicationLog mPcssCommunicationLogModel);
        Task LogSimulate(SimulateLogViewModel log);
        void SearchSimulateLog(SimulateLogViewModel log);
        Task LogException(Exception ex);
        Task AddSystemCommunicationLog(AddSystemCommunicationLog addSystemCommunicationLog);

    }
}
