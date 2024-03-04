using System;
using System.ComponentModel.DataAnnotations;
using APGMPCSSIntegration.Constant;
using NetDevPack.Domain;

namespace APGDigitalIntegration.Domain.Models
{
    public class TransactionTimeoutEnquiry : Entity, IAggregateRoot
    {
        public TransactionTimeoutEnquiry(string originalMessageId, int transactionTypeId, long digitalTransactionId)
        {
            OriginalMessageId = originalMessageId;
            TransactionTypeId = transactionTypeId;
            
            CreatedDateTime = UpdatedDateTime = DateTimeOffset.UtcNow;
            NextExecutionTime = CreatedDateTime.AddMinutes(1);
            JobState = JobStates.ReadyToProcess;
            TransactionStatus = null;
            Id = Guid.NewGuid();
            NumberOfRuns = 0;
            DigitalTransactionId = digitalTransactionId;
        }

        private TransactionTimeoutEnquiry()
        {
            
        }

        [Key]
        public int IdN { get; set; }
        public string JobState { get; set; }
        public string TransactionStatus { get; set; }
        public string ActionTaken { get; set; }
        public string OriginalMessageId { get; set; }
        public int TransactionTypeId { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public DateTimeOffset NextExecutionTime { get; set; }
        public DateTimeOffset UpdatedDateTime { get; set; }
        public int NumberOfRuns { get; set; }
        
        public DigitalTransaction DigitalTransaction { get; set; }
        
        public long DigitalTransactionId { get; set; }

        public void ConfirmFailure()
        {
            TransactionStatus = EnquiryOriginalTransactionStatus.Failure;
            ActionTaken = TimeoutActions.NoActionTaken;
            JobState = JobStates.Complete;
        }
        
        public void ConfirmRefunded()
        {
            ActionTaken = TimeoutActions.Refunded;
            JobState = JobStates.Complete;
        }

        public void ScheduleForRefund()
        {
            TransactionStatus = EnquiryOriginalTransactionStatus.LateSuccess;
            NextExecutionTime = DateTimeOffset.UtcNow;
            ActionTaken = TimeoutActions.PendingRefund;
            NumberOfRuns = 0;
        }

        private void ScheduleNextRun()
        {
            NextExecutionTime = DateTimeOffset.UtcNow.AddMinutes(7);
            JobState = JobStates.Running;
            NumberOfRuns++;
        }

        public void ScheduleNextEnquiry()
        {
            if (NumberOfRuns < 3)
            {
                ScheduleNextRun();
                ActionTaken = string.Format(TimeoutActions.EnquiryInitiated, NumberOfRuns);
                return;
            }

            JobState = JobStates.Complete;
            ActionTaken = TimeoutActions.FailedToEnquiry;
            TransactionStatus = EnquiryOriginalTransactionStatus.Unconfirmed;
        }

        public void ScheduleNextRefund()
        {
            if (NumberOfRuns < 3)
            {
                ScheduleNextRun();
                ActionTaken = string.Format(TimeoutActions.RefundInitiated, NumberOfRuns);
                return;
            }

            JobState = JobStates.Complete;
            ActionTaken = TimeoutActions.FailedToRefund;
        }

        public void Complete()
        {
            JobState = JobStates.Complete;
            ActionTaken = TimeoutActions.NoActionCouldBeTaken;
        }
    }
}
