using APGMPCSSIntegration.Application.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGMPCSSIntegration.Application.Services.Messages
{
    public class GenericSuccessResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> ErrorList { get; set; }
        public T Data { get; set; }

        public static GenericSuccessResponse<T> GetSuccessResponse(T data)
        {
            return new GenericSuccessResponse<T>()
            {
                Data = data,
                Message = Resources.Success,
                Success = true,
                ErrorList = new List<string>()
            };
        }
        public static GenericSuccessResponse<T> GetFailureResponse(IEnumerable<string> errorList)
        {
            return new()
            {
                Data = default,
                Message = "Failure",
                Success = false,
                ErrorList = errorList.ToList()
            };
        }
        public static GenericSuccessResponse<T> GetFailureResponse(string error)
        {
            return new()
            {
                Data = default,
                Message = "Failure",
                Success = false,
                ErrorList = new List<string>()
                {
                    error
                }
            };
        }


    }

    public class GenericResponse
    {
        public bool Success { get; set; }
        public string ResponseCode { get; set; }
        public string Message { get; set; }
        public object data { get; set; }

        public List<string> ErrorList { get; set; } = new();
    }
}
