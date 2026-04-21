using FinanceTracker.Application.Budgets.DTOs;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Budgets.Queries.GetBudgetVsActual;

public record GetBudgetVsActualQuery(int Year, int Month) : IRequest<IReadOnlyList<BudgetVsActualDto>>;

public class GetBudgetVsActualHandler : IRequestHandler<GetBudgetVsActualQuery, IReadOnlyList<BudgetVsActualDto>>
{
    private readonly IUnitOfWork _uow;

    public GetBudgetVsActualHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IReadOnlyList<BudgetVsActualDto>> Handle(GetBudgetVsActualQuery request, CancellationToken cancellationToken)
    {
        var date = new DateOnly(request.Year, request.Month, 1);
        var budgets = await _uow.Budgets.GetActiveByDateAsync(date, cancellationToken);

        var result = new List<BudgetVsActualDto>();

        foreach (var budget in budgets)
        {
            var (from, to) = budget.Period == BudgetPeriod.Monthly
                ? (date, date.AddMonths(1).AddDays(-1))
                : (new DateOnly(request.Year, 1, 1), new DateOnly(request.Year, 12, 31));

            var transactions = await _uow.Transactions.GetByCategoryAndDateRangeAsync(
                budget.CategoryId, from, to, cancellationToken);

            var spent = transactions
                .Where(t => t.Amount < 0)
                .Sum(t => Math.Abs(t.Amount));

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
}
