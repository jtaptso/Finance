using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistence.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly ApplicationDbContext _context;

    public BudgetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Budget>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .OrderBy(b => b.Category.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Budget>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Where(b => b.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Budget>> GetActiveByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Where(b => b.StartDate <= date && (b.EndDate == null || b.EndDate >= date))
            .ToListAsync(cancellationToken);
    }

    public void Add(Budget budget) => _context.Budgets.Add(budget);

    public void Update(Budget budget) => _context.Budgets.Update(budget);

    public void Remove(Budget budget) => _context.Budgets.Remove(budget);
}
