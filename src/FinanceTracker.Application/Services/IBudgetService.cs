using FinanceTracker.Application.Common.Models;
using FinanceTracker.Application.Budgets.Commands.CreateBudget;
using FinanceTracker.Application.Budgets.Commands.UpdateBudget;
using FinanceTracker.Application.Budgets.DTOs;

namespace FinanceTracker.Application.Services;

public interface IBudgetService
{
    Task<IReadOnlyList<BudgetDto>> GetAllAsync(CancellationToken ct = default);
    Task<BudgetDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<BudgetVsActualDto>> GetVsActualAsync(int year, int month, CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateBudgetCommand command, CancellationToken ct = default);
    Task<Result> UpdateAsync(UpdateBudgetCommand command, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
