using System;
using System.ComponentModel.DataAnnotations;
using NetDevPack.Domain;

namespace APGDigitalIntegration.Domain.Models;

public class MerchantMPCSSTransactionRequest : Entity, IAggregateRoot
{
    [Key]
    public long IdN { get; set; }
    public long? QROrderId { get; set; }
    public string ParticipantPrefix { get; set; }
    public long SequenceId { get; set; }
    public string UniqueNotificationId { get; set; }
    public string MessageId { get; set; }
    public string TransactionType { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public int? ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string Status { get; set; }
    public int RequestSourceId { get; set; }
    public int? PaymentViewType { get; set; }
    public string Language { get; set; }

}