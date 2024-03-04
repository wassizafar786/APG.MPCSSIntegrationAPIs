namespace APG.MessageQueue.Contracts.Merchant;

// Publisher: APGFundamentals
// Consumer:  APGTransactions

public class MerchantUpdated
{
    public MerchantUpdated(long merchantRefId, int settAccType, decimal maxBalanceLimit, long bankId)
    {
        MerchantRefId = merchantRefId;
        SettAccType = settAccType;
        MaxBalanceLimit = maxBalanceLimit;
        BankId = bankId;
    }

    public MerchantUpdated()
    {
        
    }
    public long MerchantRefId { get; set; }
    public int SettAccType { get; set; }
    public decimal MaxBalanceLimit { get; set; }
    public long BankId { get; set; }
}