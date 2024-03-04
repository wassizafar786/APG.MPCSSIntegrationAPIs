using APGDigitalIntegration.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace APGDigitalIntegration.Infra.Data.Mappings
{
    public class MerchantMPCSSTransactionRequestMap : IEntityTypeConfiguration<MerchantMPCSSTransactionRequest>
    {
        public void Configure(EntityTypeBuilder<MerchantMPCSSTransactionRequest> builder)
        {
            builder.Property(c => c.IdN)
                .IsRequired()
                .UseIdentityColumn()
                .HasColumnName("IdN");

            builder.Property(c => c.Id)
                .HasColumnName("Id")
                .HasDefaultValueSql("uuid_generate_v4()");

            builder.Property(c => c.UniqueNotificationId)
                .HasColumnType("varchar(100)")
                .HasColumnName("UniqueNotificationId");

            builder.Property(c => c.MessageId)
                .HasColumnType("varchar(80)")
                .HasColumnName("MessageId")
                .IsRequired();

            builder.Property(c => c.ParticipantPrefix)
                .HasColumnType("VARCHAR(4)");

           

            builder.Property(c => c.SequenceId)
              .HasDefaultValueSql("NEXTVAL('\"MPCSSMessageIdSequence\"')");

            builder.Property(s => s.MessageId)
            .HasComputedColumnSql("\"ParticipantPrefix\" || REPEAT('0', 14 - LENGTH(TRIM(TRAILING ' ' FROM \"SequenceId\"::TEXT))) || TRIM(TRAILING ' ' FROM \"SequenceId\"::TEXT)", stored: true)
            .HasColumnType("VARCHAR(18)");

            builder.Property(c => c.QROrderId)
                .HasColumnType("bigint")
                .HasColumnName("QROrderId");

            builder.Property(c => c.TransactionType)
                .HasColumnType("varchar(50)")
                .HasColumnName("TransactionType")
                .IsRequired();

            builder.Property(c => c.CreationDate)
               .HasColumnType("timestamptz")
               .HasColumnName("CreationDate")
               .IsRequired();

            builder.Property(c => c.ErrorMessage)
               .HasColumnType("varchar(1000)")
               .HasColumnName("ErrorMessage");

            builder.Property(c => c.ErrorCode)
               .HasColumnName("ErrorCode");

            builder.Property(c => c.Status)
              .HasColumnType("varchar(30)")
              .HasColumnName("Status");
        }
    }
}
