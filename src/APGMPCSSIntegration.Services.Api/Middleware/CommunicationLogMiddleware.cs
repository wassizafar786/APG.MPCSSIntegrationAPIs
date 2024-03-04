using System.IO;
using System.Text;
using System.Threading.Tasks;
using APG.MessageQueue.Interfaces;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace APGDigitalIntegration.Services.Api.Middleware;

public class CommunicationLogMiddleware : IMiddleware
{
    #region Fields

    private readonly IMPCSSCommunicationLogService _communicationLogService;
    private readonly ILoggingService _loggingService;

    #endregion

    #region Constructor

    public CommunicationLogMiddleware(IMPCSSCommunicationLogService communicationLogService,
        ILoggingService loggingService
    )
    {
        _communicationLogService = communicationLogService;
        _loggingService = loggingService;
    }

    #endregion

    #region InvokeAsync

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var internalRequest = await this.GetRequest(context.Request);
        _communicationLogService.SetInternalRequest(internalRequest);
        _communicationLogService.SetRequestDatetime(System.DateTime.Now);
        await _communicationLogService.SetInternalRequestTime();

        var originalBodyStream = context.Response.Body;
        await using var responseBody = new MemoryStream();
            
        context.Response.Body = responseBody;

        await next(context);

        var response = await GetResponse(context.Response);
        await responseBody.CopyToAsync(originalBodyStream);

        if (_communicationLogService.CommunicationLogEnabled == false)
            return;

        // _communicationLogService.SetInternalResponse(response);
        await _loggingService.LogMPCSSCommunicationLog(_communicationLogService.MPCSSCommunicationLogModel);
    }

    #endregion

    private async Task<string> GetRequest(HttpRequest request)
    {
        request.EnableBuffering();

        using var reader = new StreamReader(request.Body, Encoding.UTF8, false, 1024, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return body;
    }

    private async Task<string> GetResponse(HttpResponse response)
    {
        //We need to read the response stream from the beginning...
        response.Body.Seek(0, SeekOrigin.Begin);

        //...and copy it into a string

        using var reader = new StreamReader(response.Body, Encoding.UTF8, false, 1024, leaveOpen: true);
        var body = await reader.ReadToEndAsync();

        //We need to reset the reader for the response so that the client can read it.
        response.Body.Seek(0, SeekOrigin.Begin);

        //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
        return body;
    }
}