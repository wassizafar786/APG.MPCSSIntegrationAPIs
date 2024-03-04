using APG.MessageQueue.Contracts.Digital_Transactions;
using APG.MessageQueue.Interfaces;
using APGDigitalIntegration.Cache.Interfaces;
using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGTransaction;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.Internal.CacheHelper;
using APGDigitalIntegration.Infra.Data.Context;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Spring.Objects.Factory.Config;

namespace APGDigitalIntegration.BackgroundJobs.Services;

[DisableConcurrentExecution(timeoutInSeconds: 60)]
public class TransactionTimeoutEnquiryJob
{
    private static readonly Dictionary<int, TransactionTypeCacheModel> TransactionTypes;
    private readonly APGDigitalIntegrationContext _apgTransactionContext;
    private readonly IMessageQueue _messageQueue;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICacheService _cacheService;

    static TransactionTimeoutEnquiryJob()
    {
        TransactionTypes = new Dictionary<int, TransactionTypeCacheModel>();
    }
    
    public TransactionTimeoutEnquiryJob(APGDigitalIntegrationContext context, IMessageQueue messageQueue, IDateTimeProvider dateTimeProvider, ICacheService cacheService)
    {
        _apgTransactionContext = context;
        _messageQueue = messageQueue;
        _dateTimeProvider = dateTimeProvider;
        _cacheService = cacheService;
    }

    public async Task Execute()
    {
        var dateTime = DateTimeOffset.UtcNow;

        var toBeExecutedTransactions = await _apgTransactionContext
            .Set<TransactionTimeoutEnquiry>()
            .AsTracking()
            .Include(s => s.DigitalTransaction)
            .Where(s => s.NextExecutionTime <=dateTime)
            .Where(s => s.JobState == JobStates.ReadyToProcess || s.JobState == JobStates.Running)
            .ToListAsync();
            
        if (toBeExecutedTransactions.Any() == false)
            return;
            
        var sendEnquiryRequests = toBeExecutedTransactions.Where(s => s.TransactionStatus != EnquiryOriginalTransactionStatus.LateSuccess).ToList();
        sendEnquiryRequests.ForEach(s => s.ScheduleNextEnquiry());
        await SendEnquiryMessages(sendEnquiryRequests);
            
        var sendRefundRequests = toBeExecutedTransactions
            .Where(s => s.TransactionStatus == EnquiryOriginalTransactionStatus.LateSuccess)
            .Where(s => IsRefundTransaction(s).GetAwaiter().GetResult() == false)
            .ToList();
        
        sendRefundRequests.ForEach(s => s.ScheduleNextRefund());
        await SendRefundMessages(sendRefundRequests);
        
        await _apgTransactionContext.SaveChangesAsync();
    }

    private async Task<bool> IsRefundTransaction(TransactionTimeoutEnquiry transactionTimeoutEnquiry)
    {
        if (TransactionTypes.ContainsKey(transactionTimeoutEnquiry.TransactionTypeId))
            return TransactionTypes[transactionTimeoutEnquiry.TransactionTypeId].IsRefund;

        var cacheKey = CacheKeysHelper.GetKey(new TransactionTypeCacheKeyModel(transactionTimeoutEnquiry.TransactionTypeId));
        var transactionTypeCacheModel = await _cacheService.GetValueByKeyAsync<TransactionTypeCacheModel>(cacheKey);
        TransactionTypes.Add(transactionTimeoutEnquiry.TransactionTypeId, transactionTypeCacheModel);
        return transactionTypeCacheModel.IsRefund;
    }
    
    public async Task SendEnquiryMessages(List<TransactionTimeoutEnquiry> transactionTimeoutEnquiries)
    {
        var enquiryRequests = transactionTimeoutEnquiries.Select(s => new DigitalPaymentEnquiry()
        {
            RequestSource = (int)RequestSources.System,
            MerchantId = s.DigitalTransaction.MerchantId,
            TerminalId = s.DigitalTransaction.TerminalId,
            TransactionId = Guid.NewGuid(),
            TransactionIdentifierValue = s.DigitalTransaction.IdN.ToString(),
            DigitalTransactionIdentifierType = (int)DigitalTransactionIdentifier.DigitalTransactionIdN
        }).ToList();

        foreach (var paymentEnquiryInputDto in enquiryRequests)
            await _messageQueue.PublishMessage(paymentEnquiryInputDto, CancellationToken.None);
    }
        
    public async Task SendRefundMessages(List<TransactionTimeoutEnquiry> transactionTimeoutEnquiries)
    {
        var refundRequests = new List<DigitalPaymentRefund>();

        foreach (var transactionTimeoutEnquiry in transactionTimeoutEnquiries)
        {
            var now = await _dateTimeProvider.NowByBankId(transactionTimeoutEnquiry.DigitalTransaction.BankId!.Value);
            var refundRequest = new DigitalPaymentRefund()
            {
                RequestSource = (int)RequestSources.System,
                MerchantId = transactionTimeoutEnquiry.DigitalTransaction.MerchantId,
                TerminalId = transactionTimeoutEnquiry.DigitalTransaction.TerminalId,
                TransactionId = Guid.NewGuid(),
                ProcessingCode = ProcessingCodes.P2BRefund,
                TransactionIdentifierType = (int)TransactionIdentifier.TransactionId,
                TransactionIdentifierValue = transactionTimeoutEnquiry.DigitalTransaction.Id.ToString(),
                RequestDateTime = now.LocalDateTime
            };

            refundRequests.Add(refundRequest);
        }
        
        foreach (var refundRequest in refundRequests)
            await _messageQueue.PublishMessage(refundRequest, CancellationToken.None);
    }
}