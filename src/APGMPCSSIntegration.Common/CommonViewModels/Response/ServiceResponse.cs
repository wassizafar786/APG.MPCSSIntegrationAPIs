namespace APGDigitalIntegration.Common.CommonViewModels.Response;

public class ServiceResponse
{
    public string ResponseCode { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }

    public List<string> ErrorList { get; set; } = new();

    public ServiceResponse()
    {
    }

    public ServiceResponse(bool success, string responseCode, string message)
    {
        Success = success;
        ResponseCode = responseCode;
        Message = message;
    }

   
    public ServiceResponse(bool success, string responseCode, string message, object data ,List<string> errorList)
    {
        Success = success;
        ResponseCode = responseCode;
        Message = message;
        Data = data;
        ErrorList=errorList;
    }
}

public class ServiceResponse<T>
{
    public string ResponseCode { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    
    public List<string> ErrorList { get; set; } = new();
        
    public ServiceResponse()
    {
    }

    public ServiceResponse(bool success, string responseCode, string message)
    {
        Success = success;
        ResponseCode = responseCode;
        Message = message;
    }

    public ServiceResponse(bool success, string responseCode, string message, T data)
    {
        Success = success;
        ResponseCode = responseCode;
        Message = message;
        Data = data;
    }
}