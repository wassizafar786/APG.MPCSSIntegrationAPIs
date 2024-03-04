using System.Threading.Tasks;
using APGDigitalIntegration.Domain.Models;
using NetDevPack.Data;

namespace APGDigitalIntegration.Domain.Interfaces
{
    public interface IDigitalTransactionEnquiryRepository :IRepository<TransactionTimeoutEnquiry>
    {
        Task<TransactionTimeoutEnquiry> GetByDigitalTransactionId(long id);
        Task<TransactionTimeoutEnquiry> AddAsync(TransactionTimeoutEnquiry transactionTimeout);
        void Update(TransactionTimeoutEnquiry transactionTimeout);
        Task<TransactionTimeoutEnquiry> GetByExternalMessageId(string externalMessageId);
    }
}
