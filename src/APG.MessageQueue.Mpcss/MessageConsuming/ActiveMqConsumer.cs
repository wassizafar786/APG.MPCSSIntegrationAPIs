using Apache.NMS;
using Apache.NMS.ActiveMQ;
using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APG.MessageQueue.Mpcss.MessageHandling;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.DomainHelper.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Spring.Messaging.Nms.Connections;
using Spring.Messaging.Nms.Core;
using Spring.Messaging.Nms.Listener;
using System.Text.Json;

namespace APG.MessageQueue.Mpcss.MessageConsuming;

public class ActiveMqConsumer<TMessageListener, TMessage> : IMessageListener, IInitializeConsumer, IDisposable
    where TMessageListener : IMessageHandlerBase<TMessage>
    where TMessage : class
{
    private IServiceScopeFactory _serviceScopeFactory;

    private SimpleMessageListenerContainer _messageListenerContainer;
    private IServiceScope _consumerScope;
    private ILoggingService _loggingService;


    public void InitializeConsumer(ActiveMQConsumerConfig activeMQConsumerConfig, CachingConnectionFactory connectionFactory)
    {
        this._serviceScopeFactory = activeMQConsumerConfig.ServiceScopeFactory;
        this._consumerScope = _serviceScopeFactory.CreateScope();
        this._loggingService = this._consumerScope.ServiceProvider.GetService<ILoggingService>();


        _messageListenerContainer = new SimpleMessageListenerContainer();
        _messageListenerContainer.ConnectionFactory = connectionFactory;
        _messageListenerContainer.DestinationName = activeMQConsumerConfig.DestinationQueue;
        _messageListenerContainer.PubSubDomain = false; // Set to true for topic-based messaging
        _messageListenerContainer.MessageListener = this;
        _messageListenerContainer.RecoveryInterval = TimeSpan.FromSeconds(activeMQConsumerConfig.ConnectionRecoveryIntervalInSeconds);
        _messageListenerContainer.MaxRecoveryTime = TimeSpan.MaxValue;
        _messageListenerContainer.SessionAcknowledgeMode = AcknowledgementMode.ClientAcknowledge;

        _messageListenerContainer.AfterPropertiesSet();
    }

    public async void OnMessage(IMessage message)
    {
        using var objectHandlerScope = _serviceScopeFactory.CreateScope();
        IMPCSSCommunicationLogService mpcssLoggerService = default;
        string payload = null;
        try
        {
            if (message is ITextMessage textMessage)
            {
                payload = textMessage.Text;
                var deserializedMessage = DeserializeXmlMessage(payload);
                if (deserializedMessage == default)
                    throw new InvalidCastException();

                var objectHandler = objectHandlerScope.ServiceProvider.GetService<TMessageListener>();
                mpcssLoggerService = objectHandlerScope.ServiceProvider.GetService<IMPCSSCommunicationLogService>();
                await objectHandler.ProcessMessage(deserializedMessage);

            }
            else if (message is IBytesMessage bytesMessage)
            {
                throw new NotImplementedException("Unsupported Message Type.");
            }

        }
        catch (Exception ex)
        {
            var exceptionId = await _loggingService.HandleException(ex);

            if (mpcssLoggerService is null)
                return;

            mpcssLoggerService.SetExceptionId(exceptionId.ToString());

            await SendMessageToQueueWithError(payload, ex, exceptionId, objectHandlerScope);

        }
        finally
        {
            if (mpcssLoggerService is not null &&
                  (mpcssLoggerService.MPCSSCommunicationLogModel.InternalRequest != null ||
                   mpcssLoggerService.MPCSSCommunicationLogModel.ExternalRequest != null ||
                   mpcssLoggerService.MPCSSCommunicationLogModel.InternalResponse != null ||
                   mpcssLoggerService.MPCSSCommunicationLogModel.ExternalResponse != null
                  ))
                await mpcssLoggerService.Log();

            await message.AcknowledgeAsync();
        }

    }

    private async Task SendMessageToQueueWithError(string payload, Exception ex, Guid exceptionId, IServiceScope objectHandlerScope)
    {
        try
        {
            var queueErrorName = $"{_messageListenerContainer.DestinationName}-error";
            var _activeMqMessageQueue = objectHandlerScope.ServiceProvider.GetService<IActiveMqMessageQueue>();
            await _activeMqMessageQueue.SendMessage(JsonConvert.SerializeObject(new
            {
                PayLoad = payload,
                ExceptionId = exceptionId,
                ExceptionError = ex
            }), queueErrorName, ActiveMQMessageTypes.Text);
        }
        catch (Exception)
        {
        }
    }

    private TMessage DeserializeXmlMessage(string payload) => XmlSerializationHelper.Deserialize<TMessage>(payload);


    public void Dispose()
    {
        _consumerScope.Dispose();
        _messageListenerContainer.Dispose();
    }
}