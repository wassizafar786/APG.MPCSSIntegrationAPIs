using APGMPCSSIntegration.IAL.Internal.Communicator;

namespace APGDigitalIntegration.IAL.Internal.Interfaces.APGLog
{
    public interface ICommunicationLogAppService
    {
        Task<BaseResponse<object>> CheckReplayAttack(DateTime requestDateTime, long terminalNodeId);
    }
}