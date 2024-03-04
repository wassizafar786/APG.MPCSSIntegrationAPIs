using APGDigitalIntegration.Domain.Models;
using APGDigitalIntegration.Infra.Data.Mappings;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using NetDevPack.Data;
using NetDevPack.Mediator;
using NetDevPack.Messaging;

namespace APGDigitalIntegration.Infra.Data.Context;

public sealed class APGDigitalIntegrationContext : DbContext, IUnitOfWork
{    
    // private readonly IMediatorHandler _mediatorHandler;
    public DbSet<DigitalTransaction> DigitalTransactions { get; set; }


    public APGDigitalIntegrationContext(DbContextOptions<APGDigitalIntegrationContext> options) : base(options)
    {
        // _mediatorHandler = mediatorHandler;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.Entity<TransactionTimeoutEnquiry>().Property(p => p.Id).HasDefaultValueSql("uuid_generate_v4()");
        modelBuilder.Entity<MerchantMPCSSTransactionRequest>().Property(p => p.Id).HasDefaultValueSql("uuid_generate_v4()");
        modelBuilder.Entity<DigitalTransaction>().Property(p => p.Id).HasDefaultValueSql("uuid_generate_v4()");

        modelBuilder.ApplyConfiguration(new DigitalTransactionEnquiryMap());
        modelBuilder.ApplyConfiguration(new MerchantMPCSSTransactionRequestMap());
        modelBuilder.ApplyConfiguration(new DigitalTransactionMap());

        modelBuilder.HasSequence<long>("MPCSSMessageIdSequence");
        modelBuilder.Ignore<ValidationResult>();
        modelBuilder.Ignore<Event>();

    }
    
    public async Task<bool> Commit()
    {
        // Dispatch Domain Events collection. 
        // Choices:
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
        // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
        // await _mediatorHandler.PublishDomainEvents(this);

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
        // performed through the DbContext will be committed
        var success = await SaveChangesAsync() > 0;

        return success;
    }


    public async Task<bool> Ping(CancellationToken cancellationToken)
    {
        try
        {
            await Database
                   .ExecuteSqlRawAsync("SELECT 1;", cancellationToken)
                   .ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}