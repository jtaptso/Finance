using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Persistence.Configurations;

public class CategoryKeywordConfiguration : IEntityTypeConfiguration<CategoryKeyword>
{
    public void Configure(EntityTypeBuilder<CategoryKeyword> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(k => k.Keyword)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(k => k.Category)
            .WithMany(c => c.Keywords)
            .HasForeignKey(k => k.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(k => k.CategoryId);
    }
}
