using APGMPCSSIntegration.IAL.Internal.Communicator;

namespace APGDigitalIntegration.IAL.Internal.Communicator
{
    public interface IApiCaller
    {
        /// <summary>
        /// Get Request with optional param
        /// </summary>
        /// <param name="baseURL">ex: http://localhost:15672/api/</param>
        /// <param name="controllerName">Bank</param>
        /// <param name="serviceName">GetById</param>
        /// <param name="queryParams">Dictionary of query Params.</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T">Response Body Data Model DataType</typeparam>
        /// <returns></returns>
        Task<BaseResponse<T>> GetAsJson<T>(string baseURL, string controllerName, string serviceName,
            Dictionary<string, string> queryParams = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Post request As Json body
        /// </summary>
        /// <param name="baseURL">ex: http://localhost:15672/api/</param>
        /// <param name="controllerName">ex: Bank</param>
        /// <param name="serviceName">ex: Add</param>
        /// <param name="inputViewModel">request body</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T">Response Body Data Model DataType</typeparam>
        /// <returns></returns>
        Task<BaseResponse<T>> PostAsJson<T>(string baseURL, string controllerName, string serviceName,
            object inputViewModel, CancellationToken cancellationToken = default);
    }
}