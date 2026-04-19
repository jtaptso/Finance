using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetAllWithKeywordsAsync(CancellationToken cancellationToken = default);
    Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    void Add(Category category);
    void Update(Category category);
    void Remove(Category category);
}
