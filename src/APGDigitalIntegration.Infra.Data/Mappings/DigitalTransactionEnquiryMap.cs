using APGDigitalIntegration.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APGDigitalIntegration.Infra.Data.Mappings
{
    public class DigitalTransactionEnquiryMap : IEntityTypeConfiguration<TransactionTimeoutEnquiry>
    {
        public void Configure(EntityTypeBuilder<TransactionTimeoutEnquiry> builder)
        {
            builder.Property(c => c.IdN)
                .IsRequired()
                .UseIdentityColumn()
                .HasColumnName("IdN");

            builder.Property(c => c.Id)
                .IsRequired()
                .HasColumnName("Id")
                .HasDefaultValueSql("uuid_generate_v4()");

            builder.Property(c => c.JobState)
                .HasColumnType("varchar(15)")
                .IsRequired();

            builder.Property(c => c.CreatedDateTime)
                .HasColumnType("timestamptz");
            
            builder.Property(c => c.NextExecutionTime)
                .HasColumnType("timestamptz")
                .IsRequired();

            builder.Property(c => c.UpdatedDateTime)
                .HasColumnType("timestamptz");
        }

    }
}
