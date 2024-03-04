using System.Net.Http.Json;
using System.Text.Json;
using APG.MessageQueue.Contracts.SystemCommunicationLog;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.DomainHelper;
using APGDigitalIntegration.DomainHelper.Interfaces;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace APGDigitalIntegration.IAL.Internal.Communicator
{
    public class ApiCaller : IApiCaller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;
        private static readonly JsonSerializerOptions JsonSerializerOptions;

        static ApiCaller()
        {
            JsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }
        public ApiCaller(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor=httpContextAccessor;
            _serviceProvider=serviceProvider;
        }

        #region GetAsJson<T>

        /// <summary>
        /// Get Request with optional param
        /// </summary>
        /// <param name="baseURL">ex: http://localhost:15672/</param>
        /// <param name="controllerName">Bank</param>
        /// <param name="serviceName">GetById</param>
        /// <param name="queryParams">Dictionary of query Params.</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T">Response Body Data Model DataType</typeparam>
        /// <returns></returns>
        public async Task<BaseResponse<T>> GetAsJson<T>(string baseURL, string controllerName, string serviceName,
            Dictionary<string, string> queryParams = null, CancellationToken cancellationToken = default)
        {
            var requestTime = DateTime.Now;
            var baseResponse = new BaseResponse<T>();
            string correlationId = _httpContextAccessor?.HttpContext?.Request?.Headers?["CorrelationId"];
            //string token = _httpContextAccessor?.HttpContext?.Request?.Headers?["Authorization"];
            var queryString = queryParams?.AsQueryString() ?? string.Empty;
            try
            {
                var continueChecking = true;

                var serviceBaseURL = $"{baseURL}{controllerName}/";


                var clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

                var httpClient = _httpClientFactory.CreateClient();
                // var httpClient = new HttpClient(clientHandler);
                httpClient.BaseAddress = new Uri(serviceBaseURL);

                var jsonSerializerOptions = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                };

                #region Add Culture Info

                var cultureInfo = Thread.CurrentThread.CurrentCulture;
                httpClient.DefaultRequestHeaders.Add("Accept-Language", cultureInfo.Name);

                #endregion

                #region Add Token
                SetUserToken(httpClient);
                #endregion

                #region SystemToken
                SetSystemToken(httpClient);
                #endregion
                var response = await httpClient.GetAsync($"{serviceName}{queryString}", cancellationToken).ConfigureAwait(false);
                var responseBody = await response.Content.ReadFromJsonAsync<BaseResponse<T>>(jsonSerializerOptions).ConfigureAwait(false);

                #region On Response Failure

                if (response.IsSuccessStatusCode == false)
                {
                    continueChecking = false;
                    baseResponse= new BaseResponse<T>()
                    {
                        Data = default,
                        Message = responseBody?.Message ?? "Failure",
                        Success = false,
                        ErrorList = responseBody?.ErrorList ?? new List<string>()
                    };
                }

                #endregion

                #region On Response Success

                if (responseBody != null && continueChecking)
                {
                    continueChecking= false;
                    baseResponse = new BaseResponse<T>()
                    {
                        Message = responseBody.Message,
                        Success = responseBody.Success,
                        ErrorList = responseBody.ErrorList,
                        Data = responseBody.Data
                    };
                }

                #endregion

                #region On Response Body parsing Failure
                if (continueChecking)
                {
                    continueChecking=false;
                    baseResponse = new BaseResponse<T>()
                    {
                        Success = false,
                        Message = "Failure",
                        ErrorList = new List<string>() { "No Response Body Received" },
                        Data = default
                    };
                }

                #endregion


            }
            catch (Exception ex)
            {
                //TODO: Should log exception here.
                baseResponse= new BaseResponse<T>()
                {
                    Data = default,
                    Success = false,
                    Message = "Failure",
                    ErrorList = new List<string>() { ex.Message }
                };
            }

            #region Add System Log

            if (correlationId != null)
            {
                AddSystemCommunicationLog($"{serviceName}{queryString}", null, baseResponse.SerializeObject(), requestTime, correlationId);
            }

            #endregion
            return baseResponse;
        }

        #endregion

        #region PostAsJson<T>

        /// <summary>
        /// Post request As Json body
        /// </summary>
        /// <param name="baseURL">ex: http://localhost:15672/</param>
        /// <param name="controllerName">ex: Bank</param>
        /// <param name="serviceName">ex: Add</param>
        /// <param name="inputViewModel">request body</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T">Response Body Data Model DataType</typeparam>
        /// <returns></returns>
        public async Task<BaseResponse<T>> PostAsJson<T>(string baseURL, string controllerName, string serviceName,
            object inputViewModel, CancellationToken cancellationToken = default)
        {
            var requestTime = DateTime.Now;
            var customBaseResponse = new BaseResponse<T>();
            string correlationId = _httpContextAccessor?.HttpContext?.Request?.Headers?["CorrelationId"];

            var serviceBaseURL = $"{baseURL}{controllerName}/";

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(serviceBaseURL);

            #region Add Culture Info

            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            httpClient.DefaultRequestHeaders.Add("Accept-Language", cultureInfo.Name);

            #endregion

            #region Add CorrelationId 

            if (correlationId != null)
            {
                httpClient.DefaultRequestHeaders.Add("CorrelationId", correlationId);
            }

            #endregion

            #region Add Token
            SetUserToken(httpClient);
            #endregion

            #region SystemToken
            SetSystemToken(httpClient);
            #endregion

            var response = await httpClient.PostAsJsonAsync($"{serviceName}", inputViewModel, JsonSerializerOptions, cancellationToken)
                .ConfigureAwait(false);
            var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var res = DeserializeJsonFromStream<BaseResponse<T>>(responseStream);

                if (correlationId != null)
                {
                    AddSystemCommunicationLog($"{serviceName}", inputViewModel.SerializeObject(), res.SerializeObject(), requestTime, correlationId);
                }

                return res;
            }

            var responseBody = await StreamToStringAsync(responseStream).ConfigureAwait(false);

            if (TryParseJson<BaseResponse<T>>(responseBody, out var baseResponse))
            {
                if (correlationId != null)
                {
                    AddSystemCommunicationLog($"{serviceName}", inputViewModel.SerializeObject(), baseResponse.SerializeObject(), requestTime, correlationId);
                }
                return baseResponse;
            }

            // Unknown error has occured.
            throw new BadRequestException();
        }

        #endregion

        private static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false)
                return default;

            using var sr = new StreamReader(stream);
            using var jtr = new JsonTextReader(sr);

            var js = new JsonSerializer();
            var searchResult = js.Deserialize<T>(jtr);
            return searchResult;
        }

        private static async Task<string> StreamToStringAsync(Stream stream)
        {
            if (stream == null)
                return null;

            using var sr = new StreamReader(stream);
            var content = await sr.ReadToEndAsync();

            return content;
        }

        private static bool TryParseJson<T>(string data, out T result)
        {
            var success = true;
            var settings = new JsonSerializerSettings
            {
                Error = (_, args) =>
                {
                    success = false;
                    args.ErrorContext.Handled = true;
                },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(data, settings);
            return success;
        }



        async Task AddSystemCommunicationLog(string serviceName, string requestBody, string responseBody, DateTime requestTime, string correlationId)
        {

            var _loggingService = _serviceProvider.GetService<ILoggingService>();

            var systemCommunicationLogViewModel = new AddSystemCommunicationLog
            {
                Request = requestBody,
                CorrelationId = correlationId,
                Response = responseBody,
                RequestTime = requestTime,
                ResponseTime = DateTime.UtcNow,
                ServiceName = serviceName,
                SourceId = (int)RequestSources.APGDigitalIntegration,
                IsChild=true
            };

            await _loggingService.AddSystemCommunicationLog(systemCommunicationLogViewModel);
        }

        private void SetSystemToken(HttpClient httpClient)
        {
            string systemToken = _httpContextAccessor?.HttpContext?.Request?.Headers?[CommonConstant.SystemToken];

            if (!string.IsNullOrEmpty(systemToken))
            {
                httpClient.DefaultRequestHeaders.Add(CommonConstant.SystemToken, systemToken);
            }
            else
            {
                var _systemTokenService = _serviceProvider.GetService<ISystemTokenService>();

                httpClient.DefaultRequestHeaders.Add(CommonConstant.SystemToken, _systemTokenService.GenerateSystemToken());
            }
        }

        private void SetUserToken(HttpClient httpClient)
        {
            string token = _httpContextAccessor?.HttpContext?.Request?.Headers?["Authorization"];
            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", token);
            }
        }
    }
}