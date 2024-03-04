using APGMPCSSIntegration.Constant;
using System.Threading.Tasks;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using APGMPCSSIntegration.IAL.Internal.Viewmodel.APGFundamentals;

namespace APGExecutions.IAL.Internal.Interfaces.APGFundamentals
{
    public interface IConfParamHelperService
    {
        Task<T> GetValue<T>(ConfigParam configParam, long? bankId = null);
        Task<BaseResponse<ConfParameterViewModel>> GetConfParam(ConfigParam paramName, long? bankId);
    }
}