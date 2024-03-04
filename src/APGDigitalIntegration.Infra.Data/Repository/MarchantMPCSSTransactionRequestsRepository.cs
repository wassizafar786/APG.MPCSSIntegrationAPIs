using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using APGDigitalIntegration.Domain.Models;

namespace APGDigitalIntegration.Infra.Data.Repository
{
    public class MerchantMPCSSTransactionRequestsRepository : IMerchantMPCSSTransactionRequestsRepository
    {
        public IUnitOfWork UnitOfWork => _db;

        private readonly APGDigitalIntegrationContext _db;
        private readonly DbSet<MerchantMPCSSTransactionRequest> _dbSet;
        
        public MerchantMPCSSTransactionRequestsRepository(APGDigitalIntegrationContext context)
        {
            _db = context;
            _dbSet = _db.Set<MerchantMPCSSTransactionRequest>();
        }
        
        
        public async Task<MerchantMPCSSTransactionRequest> Add(MerchantMPCSSTransactionRequest requests)
        {
            await _dbSet.AddAsync(requests);

            return requests;
        }
        
        public async Task<MerchantMPCSSTransactionRequest> GetByMessageId(string messageId)
        {
            return await _dbSet.AsTracking().FirstOrDefaultAsync(x => x.MessageId == messageId);
        }
        
        public void Dispose()
        {
            _db.Dispose();
        }

    }
}
