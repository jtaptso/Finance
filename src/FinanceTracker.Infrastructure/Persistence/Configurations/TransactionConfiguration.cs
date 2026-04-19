using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Date)
            .IsRequired();

        builder.Property(t => t.TransactionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.OriginalDescription)
            .HasMaxLength(500);

        builder.Property(t => t.Notes)
            .HasMaxLength(1000);

        builder.HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.ImportHistory)
            .WithMany(i => i.Transactions)
            .HasForeignKey(t => t.ImportHistoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(t => t.Date);
        builder.HasIndex(t => t.CategoryId);
        builder.HasIndex(t => t.AccountId);
        builder.HasIndex(t => t.ImportHistoryId);
        builder.HasIndex(t => new { t.Date, t.Amount, t.Description })
            .HasDatabaseName("IX_Transaction_Duplicate");
    }
}
