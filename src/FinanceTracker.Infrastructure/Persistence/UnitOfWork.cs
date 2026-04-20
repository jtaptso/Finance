using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(
        ApplicationDbContext context,
        ITransactionRepository transactions,
        ICategoryRepository categories,
        IAccountRepository accounts,
        IBudgetRepository budgets,
        IImportHistoryRepository importHistories)
    {
        _context = context;
        Transactions = transactions;
        Categories = categories;
        Accounts = accounts;
        Budgets = budgets;
        ImportHistories = importHistories;
    }

    public ITransactionRepository Transactions { get; }
    public ICategoryRepository Categories { get; }
    public IAccountRepository Accounts { get; }
    public IBudgetRepository Budgets { get; }
    public IImportHistoryRepository ImportHistories { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
