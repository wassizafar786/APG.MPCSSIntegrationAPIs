using APGDigitalIntegration.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APGDigitalIntegration.Infra.Data.Mappings
{
    public class DigitalTransactionMap : IEntityTypeConfiguration<DigitalTransaction>
    {
        public void Configure(EntityTypeBuilder<DigitalTransaction> builder)
        {
            builder.Property(c => c.IdN)
                .IsRequired()
                .UseIdentityColumn()
                .HasColumnName("IdN");

            builder.Property(c => c.Id)
                .HasColumnName("Id")
                .HasDefaultValueSql("uuid_generate_v4()");
                        
            builder.Property(c => c.OrderId)
                .HasColumnName("OrderId")
                .HasColumnType("VARCHAR(50)");
            
            builder.Property(c => c.From)
             .HasColumnType("varchar(25)");

            builder.Property(c => c.To)
          .HasColumnType("varchar(25)");

           builder.Property(c => c.MerchantRefId)
                .HasColumnType("bigint")
                .IsRequired();

            builder.Property(c => c.TerminalNodeId)
              .HasColumnType("bigint")
              .IsRequired();

            builder.Property(c => c.BankId)
            .HasColumnType("bigint")
            .IsRequired();

            builder.Property(c => c.Amount)
                .HasColumnType("decimal(18, 0)")
                .IsRequired();

            builder.Property(c => c.CurrencyId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.CreatedDatetime)
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.Property(c => c.Status)
                .HasColumnType("varchar(30)")
                .IsRequired();
            
            builder.Property(c => c.TransactionTypeId)
              .HasColumnType("int")
              .IsRequired();

            builder.Property(c => c.ExternalTransactionId)
                .HasColumnType("varchar(50)")
                .IsRequired();

            builder.Property(c => c.SenderMobileNo)
            .HasColumnType("varchar(250)");

            builder.Property(c => c.ReceiverMobileNo)
            .HasColumnType("varchar(25)");
                        
            builder
                .Property(p => p.RowVersion)
                .IsRowVersion();
            
            builder.ToTable("DigitalTransaction")
                .HasOne(x => x.OriginalDigitalTransaction)
                .WithMany()
                .HasForeignKey(s => s.OriginalDigitalTransactionIdN)
                .IsRequired(false);

            builder.Property(c => c.SenderName)
          .HasColumnType("varchar(25)");

            builder.Property(c => c.SenderAddress)
                .HasColumnType("varchar(250)");

            builder.Property(c => c.ReceiverName)
                .HasColumnType("varchar(25)");

            builder.Property(c => c.ReceiverAddress)
                .HasColumnType("varchar(250)");

            builder.Property(c => c.SenderReferenceNo)
                .HasColumnType("varchar(100)");

            builder.Property(c => c.IsRefunded)
                .HasColumnType("boolean")
                .HasDefaultValue(false); 

            builder.Property(c => c.RefundReason)
                .HasColumnType("varchar(100)");

            builder.Property(c => c.RefundSource)
                   .HasColumnType("varchar(100)");

            builder.Property(c => c.RefundCreatorId)
                    .HasColumnType("varchar(100)");
        }
    }
}
