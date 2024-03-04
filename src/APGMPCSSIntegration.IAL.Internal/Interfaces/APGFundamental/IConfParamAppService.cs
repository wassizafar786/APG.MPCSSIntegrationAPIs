using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using APGMPCSSIntegration.IAL.Internal.Viewmodel.APGFundamentals;
using System.Threading.Tasks;

namespace APGMPCSSIntegration.IAL.Internal.Interfaces.APGFundamentals
{
    public interface IConfParamAppService
    {
        Task<BaseResponse<ConfParameterViewModel>> GetConfParam(ConfigParam paramName, long? bankId);
    }
}