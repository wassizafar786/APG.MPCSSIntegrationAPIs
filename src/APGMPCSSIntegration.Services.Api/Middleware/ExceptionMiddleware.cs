using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using APG.MessageQueue.Contracts.Logs;
using APG.MessageQueue.Interfaces;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Domain.Interfaces;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace APGDigitalIntegration.Services.Api.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings;

        private readonly ILoggingService _loggingService;
        private readonly IMPCSSCommunicationLogService _communicationLogService;
        private readonly IStringLocalizer<ExceptionMiddleware> _localizer;
        private readonly IDateTimeProvider _dateTimeProvider;

        static ExceptionMiddleware()
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }


        public ExceptionMiddleware(ILoggingService loggingService,
            IMPCSSCommunicationLogService communicationLogService,
            IStringLocalizer<ExceptionMiddleware> localizer,
            IDateTimeProvider dateTimeProvider)
        {
            _loggingService = loggingService;
            _communicationLogService = communicationLogService;
            _localizer = localizer;
            _dateTimeProvider = dateTimeProvider;
        }
        
        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                
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

                var response = new BaseResponse<object>()
                {
                    Success = false,
                    Data = null
                };

                switch (ex)
                {
                    case BusinessException businessException:
                        response.Message = this.ConstructBusinessExceptionResponseMessage(exceptionLog);
                        response.ResponseCode = businessException.ResponseCode;
                        response.ErrorList = businessException.ErrorList;
                        break;
                    case BadRequestException:
                        //TODO: To be implemented.
                        break;
                    default:
                        response.Message = this.ConstructTechnicalExceptionResponseMessage(exceptionLog);
                        response.ErrorList.Add(response.Message);
                        response.ResponseCode = ResponseCodes.TechnicalException;
                        break;
                }
                
                if(_communicationLogService.CommunicationLogEnabled)
                    _communicationLogService.SetExceptionId(exceptionLog.Id.ToString());

                await context.Response.WriteAsync(JsonConvert.SerializeObject(response, Formatting.Indented, JsonSerializerSettings)).ConfigureAwait(false);
                await _loggingService.LogException(exceptionLog);
            }
        }

        private string ConstructBusinessExceptionResponseMessage(AddExceptionLog exceptionLog)
        {
            var exceptionIdString = exceptionLog.Id.ToString();
            var last12Digits = exceptionLog.Id.ToString().Substring(exceptionIdString.Length - 12);
            return $"{_localizer[ErrorMessage.BusinessExceptionFailure]} {last12Digits}";
        }

        private string ConstructTechnicalExceptionResponseMessage(AddExceptionLog exceptionLog)
        {
            var exceptionIdString = exceptionLog.Id.ToString();
            var last12Digits = exceptionLog.Id.ToString().Substring(exceptionIdString.Length - 12);
            return $"{_localizer[ErrorMessage.TechnicalExceptionFailure]}".Replace("@", last12Digits);
        }
    }
}