using System;
using System.ComponentModel.DataAnnotations;
using NetDevPack.Domain;

namespace APGDigitalIntegration.Domain.Models;

public class DigitalTransaction : Entity, IAggregateRoot//////
{
    [Key]
    public long IdN { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public long? MerchantRefId { get; set; }
    public long? TerminalNodeId { get; set; }
    public long TerminalId { get; set; }
    public long MerchantId { get; set; }
    public long? BankId { get; set; }
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public DateTimeOffset CreatedDatetime { get; set; }
    public DateTimeOffset MaxResponseDatetime { get; set; }
    public DateTimeOffset? ResponseDatetime { get; set; }
    public string Status { get; set; }
    public long? AggregatorId { get; set; }
    public int MerchantAccountTypeId { get; set; }
    public string ResponseCode { get; set; }
    public int TransactionTypeId { get; set; }
    public string ExternalTransactionId { get; set; }
    public string OriginalExternalTransactionId { get; set; }
    public string SenderMobileNo { get; set; }
    public string ReceiverMobileNo { get; set; }
    
    public long? OriginalTransactionIdN { get; set; }
    public long? OriginalDigitalTransactionIdN { get; set; }
    public DigitalTransaction OriginalDigitalTransaction { get; set; }
    public string SenderName { get; set; }
    public string SenderAddress { get; set; }
    public string ReceiverName { get; set; }
    public string ReceiverAddress { get; set; }
    public string SenderReferenceNo { get; set; }
    public bool IsRefunded { get; set; }
    public string RefundReason { get; set; }
    public string RefundSource { get; set; }
    public string RefundCreatorId { get; set; }
    public int TransactionMethodId { get; set; }
    public int RequestSourceId { get; set; }
    public Guid OrderId { get; set; }
    public long MerchantBranchId { get; set; }

    [Timestamp] 
    public byte[] RowVersion { get; set; }

}