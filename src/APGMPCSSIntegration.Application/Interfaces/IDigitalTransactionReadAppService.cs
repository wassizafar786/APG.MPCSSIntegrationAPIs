using System;
using System.Threading;
using System.Threading.Tasks;
using APGDigitalIntegration.Application.WriteModels;
using APGDigitalIntegration.DomainHelper.Filters;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;

namespace APGDigitalIntegration.Application.Interfaces;

public interface IDigitalTransactionReadAppService : IDisposable
{
    Task<DigitalTransactionViewModel> GetByTransactionId(Guid transactionId);
    Task<DigitalTransactionViewModel> GetByTransactionFilter(DigitalTransactionFilter digitalTransactionFilter);
}