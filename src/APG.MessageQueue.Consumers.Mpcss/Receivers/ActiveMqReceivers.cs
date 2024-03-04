using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.Util;
using APG.MessageQueue.Contracts.Logs;
using APG.MessageQueue.Mpcss.Options;
using APGDigitalIntegration.Common.CommonViewModels.Registeration_New;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Domain.Interfaces;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Constant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace APG.MessageQueue.Consumers.Mpcss.Receivers
{
    
    [Obsolete("Use MPCSSConsumers instead, this will be removed soon.")]
    public class ActiveMqReceivers : BackgroundService
    {
        #region Fields

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILoggingService _loggingService;
        private static IConnection _connection;
        private readonly string _activeMqUrl;
        private readonly string _password;

        #endregion

        #region Constructor

        [Obsolete("Use MPCSSConsumers instead, this will be removed soon.")]
        public ActiveMqReceivers(IOptions<ActiveMqConfiguration> rabbitMqOptions, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _loggingService = serviceScopeFactory.CreateScope().ServiceProvider.GetService<ILoggingService>();
            _activeMqUrl = rabbitMqOptions.Value.ActiveMqUrl;
            _password = rabbitMqOptions.Value.Password;

            InitializeRabbitMqListener();
        }

        #endregion

        #region Execute Async

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                stoppingToken.ThrowIfCancellationRequested();

                RegisterAsyncMessageHandler<IMessage>(new(MPCSSQueues.CheckDefaultResponseQueue, OnCheckDefaultMessageReceived));
                RegisterAsyncMessageHandler<IMessage>(new(MPCSSQueues.ReportsQueue, OnReportMessageReceived));
            }, stoppingToken);
            return Task.CompletedTask;
        }

        #endregion

        #region Register Async Message Handlers

        private void RegisterAsyncMessageHandler<T>(Tuple<string, Func<T, Task>> onNewMessageHandlerTuple)
        {
            var (queueName, onNewMessageHandler) = onNewMessageHandlerTuple;

            var session = _connection.CreateSession();
            var consumer = session.CreateConsumer(SessionUtil.GetDestination(session, queueName));

            _connection.Start();

            consumer.Listener += async (message) =>
            {
                try
                {
                    var msg = (T)message;
                    await onNewMessageHandler.Invoke(msg);
                }
                catch (Exception ex)
                {
                    var exceptionLog = PrepareExceptionLogModel(ex);
                    await _loggingService.LogException(exceptionLog);
                }
            };
        }

        #endregion

        #region Helpers

        private async Task OnCheckDefaultMessageReceived(IMessage receivedMsg)
        {
            try
            {
                var responseXml = ((ITextMessage)receivedMsg).Text;
                var envelope = MpcssMethods.BuildResponseEnvelope(responseXml);
                var isVerified = false;
                if (envelope != null)
                {
                    if (envelope.DigitalSignature != null && envelope.DigitalSignature.Length > 0)
                    {
                        var dateObject = DateTime.Parse(envelope.TransactionDate);
                        var dateString = dateObject.ToString("yyyy-MM-ddTHH:mm:ss");
                        var message = envelope.MessageContent + dateString;
                        isVerified = MpcssMethods.VerifyData(message, envelope.DigitalSignature);
                    }
                    if (responseXml != null && responseXml.Contains("<IsDefAcctRes>"))
                    {
                        var startIndex = responseXml.IndexOf("<IsDefAcctRes>");
                        var endIndex = responseXml.IndexOf("</Document>");
                        responseXml = responseXml.Substring(startIndex, endIndex - startIndex);
                    }
                }
                var response = (IsDefAcctReqResponseRoot)MpcssMethods.ConvertXMLResponseToString<IsDefAcctReqResponseRoot>(responseXml);
            }
            catch (Exception ex)
            {
                var exceptionLog = PrepareExceptionLogModel(ex);
                await _loggingService.LogException(exceptionLog);
            }

        }

        private async Task OnHeartBeatMessageReceived(IMessage receivedMsg)
        {
            try
            {
                var heartbeatResponse = new object();
                var responseXml = ((ITextMessage)receivedMsg).Text;
                var envelope = MpcssMethods.BuildResponseEnvelope(responseXml);
                var isVerified = false;
                if (envelope != null)
                {
                    if (envelope.DigitalSignature != null && envelope.DigitalSignature.Length > 0)
                    {
                        var dateObject = DateTime.Parse(envelope.TransactionDate);
                        var dateString = dateObject.ToString("yyyy-MM-ddTHH:mm:ss");
                        var message = envelope.MessageContent + dateString;
                        isVerified = MpcssMethods.VerifyData(message, envelope.DigitalSignature);
                    }
                }
                var response = (object)MpcssMethods.ConvertXMLResponseToString<object>(envelope.MessageContent);
            }
            catch (Exception ex)
            {
                var exceptionLog = PrepareExceptionLogModel(ex);
                await _loggingService.LogException(exceptionLog);
            }
        }
        
        private async Task OnReportMessageReceived(IMessage receivedMsg)
        {
            try
            {
                var messageId = receivedMsg.Properties["messageId"].ToString();
                var messageType = receivedMsg.Properties["messageType"].ToString();
                var digitalSignature = receivedMsg.Properties["digitalSignature"].ToString();
                var dateString = DateTime.UtcNow.ToString("yyyyMMddTHHmmss");
                var fileName = dateString + "_" + messageType + "_" + messageId + ".zip";
                var content = ((IBytesMessage)receivedMsg).Content;
                File.WriteAllBytes(MpcssMessageConstants.ReportFilePath + fileName, content);

            }
            catch (Exception ex)
            {
                var exceptionLog = PrepareExceptionLogModel(ex);
                await _loggingService.LogException(exceptionLog);
            }
        }

        private AddExceptionLog PrepareExceptionLogModel(Exception ex)
        {
            var exceptionLog = new AddExceptionLog
            {
                Id = Guid.NewGuid(),
                Message = ex.Message,
                Source = ex.Source,
                ExceptionServiceSource = MicroServicesName.APGDigitalIntegration,
                StackTrace = ex.StackTrace,
                InnerException = ex.InnerException?.Message,
                DateTime = DateTime.UtcNow,
                ExceptionType = ex.GetType().ToString(),
            };

            return exceptionLog;
        }

        #endregion

        #region Initialize Queue

        private void InitializeRabbitMqListener()
        {
            var uri = new Uri(_activeMqUrl);
            var factory = new ConnectionFactory(uri);
            factory.Password = _password;
            _connection = factory.CreateConnection();
        }

        #endregion
    }
}
