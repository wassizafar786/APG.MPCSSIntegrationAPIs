using System.Threading.Tasks;
using APGDigitalIntegration.Domain.Models;
using NetDevPack.Data;

namespace APGDigitalIntegration.Domain.Interfaces
{
    public interface IMerchantMPCSSTransactionRequestsRepository : IRepository<MerchantMPCSSTransactionRequest>
    {
        Task<MerchantMPCSSTransactionRequest> Add(MerchantMPCSSTransactionRequest requests);
        Task<MerchantMPCSSTransactionRequest> GetByMessageId(string messageId);
    }
}
