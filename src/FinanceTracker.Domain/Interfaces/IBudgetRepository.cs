using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Budget>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Budget>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Budget>> GetActiveByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
    void Add(Budget budget);
    void Update(Budget budget);
    void Remove(Budget budget);
}
