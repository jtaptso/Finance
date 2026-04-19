using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Entities;

public class ImportHistory
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;
    public int TotalRows { get; set; }
    public int ImportedRows { get; set; }
    public int SkippedRows { get; set; }
    public DateOnly Month { get; set; }
    public ImportStatus Status { get; set; }
    public string? ErrorMessage { get; set; }

    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public ICollection<Transaction> Transactions { get; set; } = [];
}
