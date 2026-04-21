using FinanceTracker.Application.Budgets.Commands.CreateBudget;
using FinanceTracker.Application.Budgets.Commands.UpdateBudget;
using FinanceTracker.Application.Budgets.DTOs;
using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Common.Models;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Infrastructure.Services;

public class BudgetService : IBudgetService
{
    private readonly IUnitOfWork _uow;
    public BudgetService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<BudgetDto>> GetAllAsync(CancellationToken ct = default)
    {
        var budgets = await _uow.Budgets.GetAllAsync(ct);
        return budgets.Select(b => b.ToDto()).ToList();
    }

    public async Task<BudgetDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var budget = await _uow.Budgets.GetByIdAsync(id, ct);
        return budget?.ToDto();
    }

    public async Task<IReadOnlyList<BudgetVsActualDto>> GetVsActualAsync(int year, int month, CancellationToken ct = default)
    {
        var date = new DateOnly(year, month, 1);
        var budgets = await _uow.Budgets.GetActiveByDateAsync(date, ct);
        var result = new List<BudgetVsActualDto>();

        foreach (var budget in budgets)
        {
            var (from, to) = budget.Period == BudgetPeriod.Monthly
                ? (date, date.AddMonths(1).AddDays(-1))
                : (new DateOnly(year, 1, 1), new DateOnly(year, 12, 31));

            var transactions = await _uow.Transactions.GetByCategoryAndDateRangeAsync(budget.CategoryId, from, to, ct);
            var spent = transactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
            var remaining = budget.Amount - spent;
            var pct = budget.Amount > 0 ? (double)(spent / budget.Amount * 100) : 0;

            result.Add(new BudgetVsActualDto(
                budget.Id,
                budget.CategoryId,
                budget.Category?.Name ?? string.Empty,
                budget.Category?.Color,
                budget.Amount,
                spent,
                remaining,
                Math.Round(pct, 1),
                budget.Period
            ));
        }

        return result;
    }

    public async Task<Guid> CreateAsync(CreateBudgetCommand cmd, CancellationToken ct = default)
    {
        var budget = new Budget
        {
            Id = Guid.NewGuid(),
            CategoryId = cmd.CategoryId,
            Amount = cmd.Amount,
            Period = cmd.Period,
            StartDate = cmd.StartDate,
            EndDate = cmd.EndDate
        };
        _uow.Budgets.Add(budget);
        await _uow.SaveChangesAsync(ct);
        return budget.Id;
    }

    public async Task<Result> UpdateAsync(UpdateBudgetCommand cmd, CancellationToken ct = default)
    {
        var budget = await _uow.Budgets.GetByIdAsync(cmd.Id, ct);
        if (budget is null)
            return Result.Failure($"Budget {cmd.Id} not found.");

        budget.Amount = cmd.Amount;
        budget.Period = cmd.Period;
        budget.StartDate = cmd.StartDate;
        budget.EndDate = cmd.EndDate;

        _uow.Budgets.Update(budget);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var budget = await _uow.Budgets.GetByIdAsync(id, ct);
        if (budget is null)
            return Result.Failure($"Budget {id} not found.");

        _uow.Budgets.Remove(budget);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
