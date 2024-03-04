using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Application.WriteModels;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.DomainHelper.Filters;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using AutoMapper;
using NetDevPack.Mediator;

namespace APGDigitalIntegration.Application.Services;

public class DigitalTransactionReadReadAppService : IDigitalTransactionReadAppService
{
    private readonly IMapper _mapper;
    private readonly IDigitalTransactionRepository _digitalTransactionRepository;

    public DigitalTransactionReadReadAppService(IMapper mapper, IDigitalTransactionRepository digitalTransactionRepository)
    {
        _mapper = mapper;
        _digitalTransactionRepository = digitalTransactionRepository;
    }

    public async Task<DigitalTransactionViewModel> GetByTransactionId(Guid transactionId)
    {
        var response = await _digitalTransactionRepository.GetByTransactionId(transactionId);
        var digitalTransaction = _mapper.Map<DigitalTransactionViewModel>(response);

        return digitalTransaction;
    }
    
    public async Task<DigitalTransactionViewModel> GetByTransactionFilter(DigitalTransactionFilter digitalTransactionFilter)
    {
        var response = await _digitalTransactionRepository.GetTransaction(digitalTransactionFilter);
        var digitalTransaction = _mapper.Map<DigitalTransactionViewModel>(response);

        return digitalTransaction;
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }


}
