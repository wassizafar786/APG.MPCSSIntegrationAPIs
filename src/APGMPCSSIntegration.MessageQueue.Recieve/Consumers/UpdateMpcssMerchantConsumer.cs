using APG.MessageQueue.Contracts.MerchantMPCSSOperations;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using AutoMapper;
using MassTransit;

namespace APG.MessageQueue.Consumers.Consumers;

public class UpdateMpcssMerchantConsumer : IConsumer<UpdateMpcssMerchant>
{
    private readonly IMerchantMPCSSOperationAppService _merchantMPCSSOperationAppService;
    private readonly IMapper _mapper;

    public UpdateMpcssMerchantConsumer(IMerchantMPCSSOperationAppService merchantMPCSSOperationAppService, IMapper mapper)
    {
        _merchantMPCSSOperationAppService = merchantMPCSSOperationAppService;
        _mapper = mapper;
    }
    
    public async Task Consume(ConsumeContext<UpdateMpcssMerchant> context)
    {
        // var msg = _mapper.Map<MPCSSOperation>(context.Message);
        await _merchantMPCSSOperationAppService.MerchantMPCSSOperationRequest(context.Message).ConfigureAwait(false);
    }
}