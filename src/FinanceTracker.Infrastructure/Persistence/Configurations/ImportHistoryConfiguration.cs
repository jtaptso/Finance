using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Persistence.Configurations;

public class ImportHistoryConfiguration : IEntityTypeConfiguration<ImportHistory>
{
    public void Configure(EntityTypeBuilder<ImportHistory> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(i => i.ErrorMessage)
            .HasMaxLength(1000);

        builder.HasOne(i => i.Account)
            .WithMany(a => a.ImportHistories)
            .HasForeignKey(i => i.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.AccountId);
        builder.HasIndex(i => i.ImportedAt);
    }
}
