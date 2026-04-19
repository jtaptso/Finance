namespace FinanceTracker.Domain.Entities;

public class Account
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AccountNumber { get; set; }
    public string? BankName { get; set; }
    public string Currency { get; set; } = "EUR";
    public decimal InitialBalance { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Transaction> Transactions { get; set; } = [];
    public ICollection<ImportHistory> ImportHistories { get; set; } = [];
}
