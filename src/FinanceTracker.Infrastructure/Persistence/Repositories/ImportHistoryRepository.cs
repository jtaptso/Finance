using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistence.Repositories;

public class ImportHistoryRepository : IImportHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public ImportHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ImportHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ImportHistories
            .Include(i => i.Account)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ImportHistory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ImportHistories
            .Include(i => i.Account)
            .OrderByDescending(i => i.ImportedAt)
            .ToListAsync(cancellationToken);
    }

    public void Add(ImportHistory importHistory) => _context.ImportHistories.Add(importHistory);

    public void Remove(ImportHistory importHistory) => _context.ImportHistories.Remove(importHistory);
}
