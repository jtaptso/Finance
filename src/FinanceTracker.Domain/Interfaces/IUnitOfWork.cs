namespace FinanceTracker.Domain.Interfaces;

public interface IUnitOfWork
{
    ITransactionRepository Transactions { get; }
    ICategoryRepository Categories { get; }
    IAccountRepository Accounts { get; }
    IBudgetRepository Budgets { get; }
    IImportHistoryRepository ImportHistories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
