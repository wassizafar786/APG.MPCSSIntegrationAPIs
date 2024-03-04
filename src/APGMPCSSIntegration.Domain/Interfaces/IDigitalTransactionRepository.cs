using System;
using System.Threading;
using System.Threading.Tasks;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.DomainHelper.Filters;
using NetDevPack.Data;

namespace APGDigitalIntegration.Domain.Interfaces;

public interface IDigitalTransactionRepository : IRepository<DigitalTransaction>
{
    Task<DigitalTransaction> GetById(long id);
    Task<DigitalTransaction> GetByIdAsTracking(long id);
    Task<DigitalTransaction> GetByExternalTransactionId(string originalTransactionId);
    DigitalTransaction Add(DigitalTransaction transaction);
    void Update(DigitalTransaction transaction);
    void Remove(DigitalTransaction transaction);
    Task<DigitalTransaction> GetTransaction(DigitalTransactionFilter transactionFilter);
    Task<DigitalTransaction> GetByTransactionId(Guid transactionId);
}