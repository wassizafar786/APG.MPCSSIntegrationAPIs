using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.DomainHelper.Filters;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGDigitalIntegration.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;

namespace APGDigitalIntegration.Infra.Data.Repository;

public class DigitalTransactionRepository : IDigitalTransactionRepository
{
    private readonly APGDigitalIntegrationContext _db;
    private readonly IAmountConverter _amountConverter;

    private readonly DbSet<DigitalTransaction> _dbSet;

    public DigitalTransactionRepository(APGDigitalIntegrationContext context, IAmountConverter amountConverter)
    {
        _db = context;
        _amountConverter = amountConverter;
        _dbSet = _db.Set<DigitalTransaction>();
    }

    public IUnitOfWork UnitOfWork => _db;

    public async Task<DigitalTransaction> GetById(long idn)
    {
        return await _dbSet.AsNoTracking()
                          .FirstOrDefaultAsync(x => x.IdN == idn);
        
        // return await DbSet.Include(s => s.TransactionType).Include(s => s.Currency).AsNoTracking().FirstOrDefaultAsync(x => x.IdN == idn);
    }

    public async Task<DigitalTransaction> GetByIdAsTracking(long idn)
    {
        return await _dbSet.FindAsync(idn);
    }

    public async Task<DigitalTransaction> GetByExternalTransactionId(string originalTransactionId)
    {
        var digitalTransaction =  await _dbSet.AsTracking().FirstOrDefaultAsync(x => x.ExternalTransactionId == originalTransactionId);
        digitalTransaction.Amount = _amountConverter.ConvertToHigher(digitalTransaction.Amount, digitalTransaction.CurrencyId);
        return digitalTransaction;
    }

    public async Task<DigitalTransaction> GetByTransactionId(Guid transactionId)
    {
        var digitalTransaction = await _dbSet.FirstOrDefaultAsync(s => s.Id == transactionId);

        if (digitalTransaction is null)
            throw new BusinessException("Transaction Not Found", "44");
        
        digitalTransaction.Amount = _amountConverter.ConvertToHigher(digitalTransaction.Amount, digitalTransaction.CurrencyId);
        return digitalTransaction;
    }
    

    public async Task<DigitalTransaction> GetTransaction(DigitalTransactionFilter transactionFilter)
    {
        var transactionTypeDbSet = this._dbSet.AsNoTracking();

        var transaction =  transactionFilter.TransactionIdentifier switch
        {
            DigitalTransactionIdentifier.DigitalTransactionId => await transactionTypeDbSet.SingleOrDefaultAsync(s => s.Id.ToString() == transactionFilter.IdentifierValue),
            DigitalTransactionIdentifier.DigitalTransactionIdN => await transactionTypeDbSet.SingleOrDefaultAsync(s => s.IdN.ToString() == transactionFilter.IdentifierValue),
            _ => throw new BusinessException($"Original Transaction Identifier {transactionFilter.TransactionIdentifier} Not supported in Digital Transactions", "01")
        };
        
        transaction.Amount = _amountConverter.ConvertToHigher(transaction.Amount, transaction.CurrencyId);
        return transaction;
    }

    public DigitalTransaction Add(DigitalTransaction transaction)
    {
        transaction.Amount = _amountConverter.ConvertToLower(transaction.Amount, transaction.CurrencyId);
        _dbSet.Add(transaction);
        return transaction;
    }

    public void Update(DigitalTransaction transaction)
    {
        transaction.Amount = _amountConverter.ConvertToLower(transaction.Amount, transaction.CurrencyId);
        _dbSet.Update(transaction);
    }

    public void Remove(DigitalTransaction transaction)
    {
        _dbSet.Remove(transaction);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
