using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface IImportHistoryRepository
{
    Task<ImportHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ImportHistory>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(ImportHistory importHistory);
    void Remove(ImportHistory importHistory);
}
