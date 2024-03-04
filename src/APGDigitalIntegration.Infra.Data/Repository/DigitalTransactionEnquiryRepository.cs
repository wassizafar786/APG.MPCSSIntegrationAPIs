using APGDigitalIntegration.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.Domain.Models;

namespace APGDigitalIntegration.Infra.Data.Repository
{
    public class DigitalTransactionEnquiryRepository : IDigitalTransactionEnquiryRepository
    {
        private readonly APGDigitalIntegrationContext _db;
        private readonly DbSet<TransactionTimeoutEnquiry> _dbSet;
        
        public DigitalTransactionEnquiryRepository(APGDigitalIntegrationContext context)
        {
            _db = context;
            _dbSet = _db.Set<TransactionTimeoutEnquiry>();
        }

        public IUnitOfWork UnitOfWork => _db;

        public async Task<TransactionTimeoutEnquiry> GetByStatus(string status)
        {
            return await _dbSet.AsNoTracking().Where(x=> x.JobState == status).OrderByDescending(x=> x.IdN).FirstOrDefaultAsync();
        }
        public async Task<TransactionTimeoutEnquiry> GetByDigitalTransactionId(long digitalTransactionId)
        {
            return await _dbSet.AsTracking().Include(s => s.DigitalTransaction).FirstOrDefaultAsync(x => x.DigitalTransactionId == digitalTransactionId);
        }

        public async Task<TransactionTimeoutEnquiry> GetByExternalMessageId(string externalMessageId)
        {
            return await _dbSet.AsTracking().FirstOrDefaultAsync(x => x.OriginalMessageId == externalMessageId);
        }

        public async Task<TransactionTimeoutEnquiry> AddAsync(TransactionTimeoutEnquiry transactionTimeout)
        {
            await _dbSet.AddAsync(transactionTimeout);
            return transactionTimeout;
        }

        public void Update(TransactionTimeoutEnquiry transactionTimeout)
        {
            _dbSet.Update(transactionTimeout);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
