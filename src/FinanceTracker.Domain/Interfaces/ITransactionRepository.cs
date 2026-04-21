using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Transaction>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Transaction>> GetByMonthAsync(int year, int month, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Transaction>> GetByImportHistoryIdAsync(Guid importHistoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Transaction>> GetByCategoryAndDateRangeAsync(Guid categoryId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Transaction>> GetByDateRangeAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(DateOnly date, decimal amount, string description, CancellationToken cancellationToken = default);
    void Add(Transaction transaction);
    void AddRange(IEnumerable<Transaction> transactions);
    void Update(Transaction transaction);
    void Remove(Transaction transaction);
    void RemoveRange(IEnumerable<Transaction> transactions);
}
