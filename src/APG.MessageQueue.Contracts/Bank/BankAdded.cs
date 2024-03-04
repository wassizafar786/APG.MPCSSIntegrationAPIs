namespace APG.MessageQueue.Contracts.Bank;


// Publisher: APGFundamentals 
// Consumer:  APGTransactions
public class BankAdded
{

    public BankAdded(long idn, Guid id, string name, string code, DateTimeOffset creationDate)
    {
        IdN = idn;
        Id = id;
        Name = name;
        Code = code;
        CreationDate = creationDate;
    }

    public BankAdded()
    {
        
    }

    public long IdN { get; set; }
    public Guid Id { get; }
    public string Name { get; }
    public string Code { get; }
    public DateTimeOffset CreationDate { get; }

}