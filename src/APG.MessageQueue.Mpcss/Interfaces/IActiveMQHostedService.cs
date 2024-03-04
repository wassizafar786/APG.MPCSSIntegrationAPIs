namespace APG.MessageQueue.Mpcss.Interfaces
{
    public interface IActiveMQHostedService
    {
        Task CallExceuteAsync(CancellationToken stoppingToken);
    }
}
