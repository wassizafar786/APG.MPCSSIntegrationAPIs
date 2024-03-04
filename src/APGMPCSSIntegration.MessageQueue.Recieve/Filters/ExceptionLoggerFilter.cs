using APG.MessageQueue.Contracts.Logs;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Domain.Interfaces;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Constant;
using MassTransit;

namespace APG.MessageQueue.Consumers.Filters;

public class ExceptionLoggerFilter<T>  : IFilter<ConsumeContext<T>>
    where T : class
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILoggingService _loggingService;
    private readonly IMPCSSCommunicationLogService _communicationLogService;

    public ExceptionLoggerFilter(IDateTimeProvider dateTimeProvider, ILoggingService loggingService, IMPCSSCommunicationLogService communicationLogService)
    {
        _dateTimeProvider = dateTimeProvider;
        _loggingService = loggingService;
        _communicationLogService = communicationLogService;
    }
    
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        try
        {
            _communicationLogService.SetInternalRequest(context.Message);
            await _communicationLogService.SetInternalRequestTime();
            await next.Send(context);
        }
        catch (Exception ex)
        {
            var exceptionLog = new AddExceptionLog()
            {
                Id = Guid.NewGuid(),
                Message = ex.Message,
                Source = ex.Source,
                ExceptionServiceSource = MicroServicesName.APGDigitalIntegration,
                StackTrace = ex.StackTrace,
                InnerException = ex.InnerException?.Message,
                DateTime = await _dateTimeProvider.SystemNow(),
                ExceptionType = ex.GetType().ToString(),
            };

            await _loggingService.LogException(exceptionLog);

            if (_communicationLogService.CommunicationLogEnabled)
                _communicationLogService.MPCSSCommunicationLogModel.ExceptionLogId = exceptionLog.Id.ToString();

            throw;
        }
        finally
        {
            if (_communicationLogService.CommunicationLogEnabled)
                await _communicationLogService.Log();
        }
    }

    public void Probe(ProbeContext context)
    {
        var scope = context.CreateFilterScope("exceptionLogger");
        scope.Add("filterName", "exceptionLogger");
        scope.Add( "serviceName", MicroServicesName.APGDigitalIntegration);
    }

}
