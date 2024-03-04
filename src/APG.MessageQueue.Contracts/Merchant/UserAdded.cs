namespace APG.MessageQueue.Contracts.Merchant;

public class UserAdded
{
    public UserAdded(int dynamicRoleId, string firstName, string lastName, string fullName, 
        string userName, string email, int notificationMethod, long bankId, string phoneNumber, long? terminalNodeId, 
        long? merchantRefId, long? merchantBranchId,DateTimeOffset expirationDate)
    {
        DynamicRoleId = dynamicRoleId;
        FirstName = firstName;
        LastName = lastName;
        FullName = fullName;
        UserName = userName;
        Email = email;
        NotificationMethod = notificationMethod;
        BankId = bankId;
        TerminalNodeId = terminalNodeId;
        MerchantRefId = merchantRefId;
        MerchantBranchId = merchantBranchId;
        PhoneNumber = phoneNumber;
        ExpirationDate = expirationDate;
    }

    public UserAdded()
    {
        
    }
    public int DynamicRoleId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get;set; }
    public int NotificationMethod { get; set; }
    public long BankId { get; set; }
    public long? TerminalNodeId { get; set; }
    public long? MerchantRefId { get; set; }
    public long? MerchantBranchId { get; set; }
    public string PhoneNumber { get; set; }
    public DateTimeOffset ExpirationDate { get; set; }
}

