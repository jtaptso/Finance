using FinanceTracker.Application.Common.Models;
using FinanceTracker.Application.Transactions.Commands.CreateTransaction;
using FinanceTracker.Application.Transactions.Commands.UpdateTransaction;
using FinanceTracker.Application.Transactions.DTOs;

namespace FinanceTracker.Application.Services;

public interface ITransactionService
{
    Task<IReadOnlyList<TransactionDto>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TransactionDto>> GetByMonthAsync(int year, int month, CancellationToken ct = default);
    Task<TransactionDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateTransactionCommand command, CancellationToken ct = default);
    Task<Result> UpdateAsync(UpdateTransactionCommand command, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
