using FinanceTracker.Application.Dashboard.DTOs;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _uow;
    public DashboardService(IUnitOfWork uow) => _uow = uow;

    public async Task<MonthlyOverviewDto> GetMonthlyOverviewAsync(int year, int month, CancellationToken ct = default)
    {
        var transactions = await _uow.Transactions.GetByMonthAsync(year, month, ct);
        var income = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
        var expenses = transactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
        return new MonthlyOverviewDto(year, month, income, expenses, income - expenses, transactions.Count);
    }

    public async Task<IReadOnlyList<CategoryBreakdownItemDto>> GetCategoryBreakdownAsync(int year, int month, CancellationToken ct = default)
    {
        var transactions = await _uow.Transactions.GetByMonthAsync(year, month, ct);
        var expenses = transactions.Where(t => t.Amount < 0).ToList();
        var totalExpenses = expenses.Sum(t => Math.Abs(t.Amount));

        if (totalExpenses == 0)
            return [];

        return expenses
            .GroupBy(t => new
            {
                CategoryId = t.CategoryId ?? Guid.Empty,
                CategoryName = t.Category?.Name ?? "Uncategorized",
                CategoryColor = t.Category?.Color
            })
            .Select(g =>
            {
                var amount = g.Sum(t => Math.Abs(t.Amount));
                return new CategoryBreakdownItemDto(
                    g.Key.CategoryId,
                    g.Key.CategoryName,
                    g.Key.CategoryColor,
                    amount,
                    Math.Round((double)(amount / totalExpenses * 100), 1));
            })
            .OrderByDescending(x => x.Amount)
            .ToList();
    }

    public async Task<CashFlowTrendDto> GetCashFlowTrendAsync(int months = 6, CancellationToken ct = default)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = new List<IncomeVsExpensesItemDto>();

        for (int i = months - 1; i >= 0; i--)
        {
            var targetDate = now.AddMonths(-i);
            var transactions = await _uow.Transactions.GetByMonthAsync(targetDate.Year, targetDate.Month, ct);
            var income = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
            var expenses = transactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));
            result.Add(new IncomeVsExpensesItemDto(targetDate.Year, targetDate.Month, income, expenses, income - expenses));
        }

        return new CashFlowTrendDto(result);
    }
}
