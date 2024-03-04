using APG.MessageQueue.Contracts.Logs;
using APG.MessageQueue.Contracts.MerchantOrder;
using APG.MessageQueue.Contracts.Transactions;
using APG.MessageQueue.Interfaces;
using APGDigitalIntegration.BackgroundJobs.Helpers;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.Domain.Models;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Constant;
using APGDigitalIntegration.Infra.Data.Context;
using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace APGDigitalIntegration.BackgroundJobs.Services;

/// <summary>
/// Polls DigitalTransaction Table to detect any timed out transactions.
/// Marks the transactions as timed out
/// Reverts Merchant order status
/// Reverts ShadowBalance
/// Adds a row to TransactionTimeoutEnquiry Table
/// </summary>
public class TransactionTimeoutJob
{
    private readonly APGDigitalIntegrationContext _apgTransactionContext;
    private readonly ILoggingService _loggingService;
    private readonly IMapper _mapper;
    private readonly IAmountConverter _amountConverter;
    private readonly IMessageQueue _messageQueue;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IDigitalTransactionRepository _digitalTransactionRepository;
    private readonly TransactionTypesCacheService _transactionTypesCacheService;

    public TransactionTimeoutJob(APGDigitalIntegrationContext context, ILoggingService loggingService, IMapper mapper, IAmountConverter amountConverter, IMessageQueue messageQueue, IDateTimeProvider dateTimeProvider, IDigitalTransactionRepository digitalTransactionRepository, TransactionTypesCacheService transactionTypesCacheService)
    {
        _apgTransactionContext = context;
        _loggingService = loggingService;
        _mapper = mapper;
        _amountConverter = amountConverter;
        _messageQueue = messageQueue;
        _dateTimeProvider = dateTimeProvider;
        _digitalTransactionRepository = digitalTransactionRepository;
        _transactionTypesCacheService = transactionTypesCacheService;
    }

    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    public async Task Execute()
    {
        var dateTime = DateTimeOffset.UtcNow.AddSeconds(-1 * TimeoutJob.DetectionDelayMargin);
        // Find Unhandled Timeout Transactions
        var transactions = await _apgTransactionContext.Set<DigitalTransaction>()
            .Where(s => s.Status == DigitalTransactionStatus.Processing)
            .Where(s => s.MaxResponseDatetime < dateTime) // add margin
            .Where(s => s.RequestSourceId != (int)RequestSources.System)
            .ToListAsync();
        
        // Update Amounts
        foreach (var digitalTransaction in transactions)
            digitalTransaction.Amount = _amountConverter.ConvertToHigher(digitalTransaction.Amount, digitalTransaction.CurrencyId);
        
        if (transactions.Any() == false)
            return;

        // Remove Non financial transactions
        transactions.RemoveAll(transaction =>
        {
            var transactionType = _transactionTypesCacheService.Get(transaction.TransactionTypeId).GetAwaiter().GetResult();
            return transactionType.InstrumentId.IsNonFinancial();
        });
        
        // Revert ShadowBalance & Add Failed Timeout transaction & rollback Merchant Order.
        await RollbackTransactions(transactions);
        
        // await AddToDigitalTransactionTimeoutEnquiry(transactions);
            
        await _apgTransactionContext.SaveChangesAsync();
    }

    private async Task RollbackTransactions(List<DigitalTransaction> transactions)
    {
        var failedTransactions = new List<DigitalTransaction>();
        foreach (var digitalTransaction in transactions)
        {
            try
            {
                var isFinancial = IsFinancial(digitalTransaction);

                DateTimeOffset now;
                if (digitalTransaction.BankId.HasValue)
                    now = await _dateTimeProvider.NowByBankId(digitalTransaction.BankId.Value);
                else
                    now = await _dateTimeProvider.SystemNow();
                
                var transactionAddModel = CreateAddTransactionMessage(digitalTransaction, now);

                ForceTimeoutDigitalTransaction(digitalTransaction);

                if(isFinancial) 
                    AddTimeoutEnquiryRecord(digitalTransaction);

                // Outbox needed here.
                await _digitalTransactionRepository.UnitOfWork.Commit();

                if (digitalTransaction.OrderId != Guid.Empty && isFinancial) // direct integration order
                {
                    var msg = new UpdatePaymentLinkMerchantOrder(digitalTransaction.OrderId, ResponseCodes.SystemTimeout);
                    await _messageQueue.PublishMessage(msg, CancellationToken.None);
                }
                await _messageQueue.PublishMessage(transactionAddModel, CancellationToken.None);
            }
            catch (Exception ex)
            {
                var now = await _dateTimeProvider.SystemNow();
                failedTransactions.Add(digitalTransaction);
                var exceptionLogModel = new AddExceptionLog()
                {
                    Id = Guid.NewGuid(),
                    Message = ex.Message,
                    Source = ex.Source,
                    ExceptionServiceSource = MicroServicesName.APGDigitalIntegration,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    DateTime = now,
                    ExceptionType = ex.GetType().ToString()
                };
                await _loggingService.LogException(exceptionLogModel);
            }
        }

        foreach (var transaction in failedTransactions) // remove so that we can retry them later.
            transactions.Remove(transaction);
    }

    private bool IsFinancial(DigitalTransaction digitalTransaction)
    {
        var transactionType = _transactionTypesCacheService.Get(digitalTransaction.TransactionTypeId).GetAwaiter()
            .GetResult();
        return transactionType.InstrumentId.IsFinancial();
    }

    private void AddTimeoutEnquiryRecord(DigitalTransaction digitalTransaction)
    {
        var transactionTimeoutEnquiry = new TransactionTimeoutEnquiry(digitalTransaction.ExternalTransactionId, digitalTransaction.TransactionTypeId, digitalTransaction.IdN);
        _apgTransactionContext.Add(transactionTimeoutEnquiry);
    }

    private void ForceTimeoutDigitalTransaction(DigitalTransaction digitalTransaction)
    {
        digitalTransaction.Status = DigitalTransactionStatus.SystemTimeout;
        digitalTransaction.ResponseCode = ResponseCodes.SystemTimeout;
        
        _digitalTransactionRepository.Update(digitalTransaction);
    }

    private AddTransaction CreateAddTransactionMessage(DigitalTransaction digitalTransaction, DateTimeOffset now)
    {
        var transactionAddModel = new AddTransaction()
        {
            Id = digitalTransaction.Id,
            Amount = digitalTransaction.Amount,
            CurrencyId = digitalTransaction.CurrencyId,
            CardNumber = null,
            CVV2 = null,
            CardHolderName = null,
            MerchantRefId = digitalTransaction.MerchantRefId.GetValueOrDefault(),
            TerminalNodeId = digitalTransaction.TerminalNodeId.GetValueOrDefault(),
            TransactionTime = now,
            TransactionTypeId = digitalTransaction.TransactionTypeId,
            TransactionMethodId = digitalTransaction.TransactionMethodId,
            ChannelType = (int)ChannelTypeEnum.Wallet,
            ResponseCode = ResponseCodes.SystemTimeout,
            TerminalTypeId = (int)TerminalTypes.WalletTerminal,
            STAN = Guid.NewGuid().ToString(),
            BankId = digitalTransaction.BankId.GetValueOrDefault(),
            HostId = 0,
            MerchantAccountType = digitalTransaction.MerchantAccountTypeId,
            AggregatorId = digitalTransaction.AggregatorId,
            OrderId = digitalTransaction.OrderId,
            RequestSourceId = digitalTransaction.RequestSourceId,
            OriginalTransactionId = digitalTransaction.OriginalTransactionIdN
        };
        return transactionAddModel;
    }
    
}

        
internal static class TransactionTypeInstrumentExtensions
{
    public static bool IsFinancial(this int transactionType)
    {
        return ((TransactionInstruments)transactionType).HasFlag(TransactionInstruments.Financial);
    }    
    
    public static bool IsNonFinancial(this int transactionType)
    {
        return IsFinancial(transactionType) == false;
    }
}