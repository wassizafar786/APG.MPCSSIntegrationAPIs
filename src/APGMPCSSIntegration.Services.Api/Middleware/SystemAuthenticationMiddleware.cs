using APGDigitalIntegration.Constant;
using APGDigitalIntegration.DomainHelper.Models;
using APGDigitalIntegration.DomainHelper.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Services.Api.Middleware;

public class SystemAuthenticationMiddleware : IMiddleware
{
    private readonly byte[] _secretKey;

    public SystemAuthenticationMiddleware(IOptions<SystemAuthenticationConfig> systemAuthConfig)
    {
        _secretKey = Convert.FromBase64String(systemAuthConfig.Value.SecretKeyBase64) ?? throw new ArgumentNullException("System Token Secret Key Not Provided");
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Headers[CommonConstant.SystemToken].ToString();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                string decrypedPayload = Jose.JWT.Decode(token, _secretKey);

                SystemTokenPayload model = JsonConvert.DeserializeObject<SystemTokenPayload>(decrypedPayload);

                if (model.IsExpiryDateValid())
                    await next(context);
                else
                    throw new Exception("Expired Token");
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 401;
                return;
            }
        }
        else
        {
            context.Response.StatusCode = 401;
            return;
        }
    }
}