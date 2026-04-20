using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;

    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Transaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Transaction>> GetByMonthAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.Date.Year == year && t.Date.Month == month)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Transaction>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Transaction>> GetByImportHistoryIdAsync(Guid importHistoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.ImportHistoryId == importHistoryId)
            .OrderByDescending(t => t.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(DateOnly date, decimal amount, string description, CancellationToken cancellationToken = default)
    {
        return await _context.Transactions
            .AnyAsync(t => t.Date == date && t.Amount == amount && t.Description == description, cancellationToken);
    }

    public void Add(Transaction transaction) => _context.Transactions.Add(transaction);

    public void AddRange(IEnumerable<Transaction> transactions) => _context.Transactions.AddRange(transactions);

    public void Update(Transaction transaction) => _context.Transactions.Update(transaction);

    public void Remove(Transaction transaction) => _context.Transactions.Remove(transaction);

    public void RemoveRange(IEnumerable<Transaction> transactions) => _context.Transactions.RemoveRange(transactions);
}
