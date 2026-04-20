using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Keywords)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.SubCategories)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetAllWithKeywordsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Keywords)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AnyAsync(t => t.CategoryId == categoryId, cancellationToken);
    }

    public void Add(Category category) => _context.Categories.Add(category);

    public void Update(Category category) => _context.Categories.Update(category);

    public void Remove(Category category) => _context.Categories.Remove(category);
}
