using FinanceTracker.Application.Dashboard.DTOs;

namespace FinanceTracker.Application.Services;

public interface IDashboardService
{
    Task<MonthlyOverviewDto> GetMonthlyOverviewAsync(int year, int month, CancellationToken ct = default);
    Task<IReadOnlyList<CategoryBreakdownItemDto>> GetCategoryBreakdownAsync(int year, int month, CancellationToken ct = default);
    Task<CashFlowTrendDto> GetCashFlowTrendAsync(int months = 6, CancellationToken ct = default);
}
