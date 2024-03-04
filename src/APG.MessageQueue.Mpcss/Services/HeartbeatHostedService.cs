using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APG.MessageQueue.Mpcss.Options;
using APGDigitalIntegration.Common.CommonViewModels.Heartbeat;
using APGDigitalIntegration.Constant;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Constant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace APG.MessageQueue.Mpcss.Services;

public class HeartbeatHostedService : BackgroundService
{
    private readonly IActiveMqMessageQueue _activeMqMessageQueue;
    private readonly IOptions<ActiveMqConfiguration> _options;
    private readonly bool _isSimulation;

    public HeartbeatHostedService(IActiveMqMessageQueue activeMqMessageQueue, IOptions<ActiveMqConfiguration> options, IServiceScopeFactory serviceScopeFactory)
    {
        _activeMqMessageQueue = activeMqMessageQueue;
        _options = options;

        this._isSimulation = serviceScopeFactory.CreateScope().ServiceProvider.GetService<IConfParamHelperService>().GetValue<bool>(ConfigParam.SimulateMPCSSTransaction).GetAwaiter().GetResult();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_options.Value.HeartBeat.IsEnabled == false)
            return Task.CompletedTask;
        
        Task.Factory.StartNew(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var message = new HeartBeatMessage();


                var queue = _isSimulation 
                    ? MPCSSQueues.HeartBeatResponseQueue 
                    : MPCSSQueues.HeartBeatRequestQueue;
                    
                await _activeMqMessageQueue.SendMessage(message, queue, ActiveMQMessageTypes.Text);

                await Task.Delay(_options.Value.HeartBeat.HeartBeatIntervalInSeconds * 1000, stoppingToken);
            }

        }, stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        return Task.CompletedTask;
    }
}