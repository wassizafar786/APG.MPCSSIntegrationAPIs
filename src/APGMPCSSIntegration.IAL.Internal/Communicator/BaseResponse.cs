using APGMPCSSIntegration.Constant;
using System.Collections.Generic;

namespace APGMPCSSIntegration.IAL.Internal.Communicator;

public class BaseResponse<T>
{
    public bool Success { get; set; }
    public string ResponseCode { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> ErrorList { get; set; } = new();

    public BaseResponse<T> AddError(string error)
    {
        if (ErrorList is not null)
            this.ErrorList.Add(error);
        else
            this.ErrorList = new List<string>() {error};

        return this;
    }
    public static BaseResponse<T> GetSuccessResponse(T data, string successMessage = ResponseMessages.Success)
    {
        return new BaseResponse<T>()
        {
            Success = true,
            Data = data,
            Message = successMessage,
            ResponseCode = ResponseCodes.Success
        };
    }
    
    // Get Failure Response
    public static BaseResponse<T> GetFailureResponse(string errorMessage, string responseMessage, string responseCode)
    {
        return new BaseResponse<T>()
        {
            Success = false,
            Message = responseMessage,
            ResponseCode = responseCode,
            ErrorList = new List<string>()
            {
                errorMessage
            },
            Data = default
        };
    }
}

public class BaseNotificationResponse<T> : BaseResponse<T>
{
    public string UniqueNotificationId { get; set; }
    
    public static BaseResponse<T> GetSuccessResponse(T data, string uniqueNotificationId,  string successMessage = ResponseMessages.Success)
    {
        return new BaseNotificationResponse<T>()
        {
            Success = true,
            Data = data,
            Message = successMessage,
            ResponseCode = ResponseCodes.Success,
            UniqueNotificationId = uniqueNotificationId
        };
    }
    
    // Get Failure Response
    public static BaseResponse<T> GetFailureResponse(string errorMessage, string responseMessage, string responseCode, string uniqueNotificationId)
    {
        return new BaseNotificationResponse<T>()
        {
            Success = false,
            Message = responseMessage,
            ResponseCode = responseCode,
            UniqueNotificationId = uniqueNotificationId,
            ErrorList = new List<string>()
            {
                errorMessage
            },
            Data = default
        };
    }
}