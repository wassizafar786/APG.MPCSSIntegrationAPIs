using APG.MessageQueue.Mpcss.Interfaces;
using Microsoft.Extensions.Hosting;

namespace APG.MessageQueue.Mpcss.Services;

public class ReconnectionActiveMQService : BackgroundService
{
    private readonly HeartBeatStatus _heartbeatStatus;
    public readonly IActiveMQHostedService _activeMQHostedService;
    public ReconnectionActiveMQService(HeartBeatStatus heartbeatStatus, IActiveMQHostedService activeMQHostedService)
    {
        _heartbeatStatus = heartbeatStatus;
        _activeMQHostedService = activeMQHostedService;
    }
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        await Task.Yield();

        while (!token.IsCancellationRequested)
        {
            await Task.Delay((int)TimeSpan.FromMinutes(10).TotalMilliseconds, token);

            if (!_heartbeatStatus.ConnectionIsProcessing &&
                (!_heartbeatStatus.LastHeartBeatReceived.HasValue ||
                _heartbeatStatus.LastHeartBeatReceived.GetValueOrDefault().AddMinutes(15) < DateTime.UtcNow))
            {
                await _activeMQHostedService.CallExceuteAsync(token);
            }

        }
    }

}