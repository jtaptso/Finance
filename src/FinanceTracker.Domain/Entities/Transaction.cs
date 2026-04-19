using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public string Description { get; set; } = string.Empty;
    public string? OriginalDescription { get; set; }
    public bool IsRecurring { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid? ImportHistoryId { get; set; }
    public ImportHistory? ImportHistory { get; set; }
}
