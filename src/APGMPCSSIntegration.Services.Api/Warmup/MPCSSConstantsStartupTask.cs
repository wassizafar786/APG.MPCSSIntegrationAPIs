using System.Threading;
using System.Threading.Tasks;
using APGDigitalIntegration.Constant;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGMPCSSIntegration.Constant;

namespace APGMPCSSIntegration.Services.Api.Warmup;

public class MPCSSConstantsStartupTask : IStartupTask
{
    private readonly IConfParamHelperService _confParamHelperService;

    public MPCSSConstantsStartupTask(IConfParamHelperService confParamHelperService)
    {
        _confParamHelperService = confParamHelperService;
    }
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var mpcssParticipentShortName = await _confParamHelperService.GetValue<string>(ConfigParam.MPCSSPSPRouteCode);
        MPCSSQueues.Initialize(mpcssParticipentShortName);
    }
}