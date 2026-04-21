using FinanceTracker.Application.Dashboard.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Dashboard.Queries.GetCategoryBreakdown;

public record GetCategoryBreakdownQuery(int Year, int Month) : IRequest<IReadOnlyList<CategoryBreakdownItemDto>>;

public class GetCategoryBreakdownHandler : IRequestHandler<GetCategoryBreakdownQuery, IReadOnlyList<CategoryBreakdownItemDto>>
{
    private readonly IUnitOfWork _uow;

    public GetCategoryBreakdownHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IReadOnlyList<CategoryBreakdownItemDto>> Handle(GetCategoryBreakdownQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _uow.Transactions.GetByMonthAsync(request.Year, request.Month, cancellationToken);

        var expenses = transactions.Where(t => t.Amount < 0).ToList();
        var totalExpenses = expenses.Sum(t => Math.Abs(t.Amount));

        if (totalExpenses == 0)
            return [];

        var grouped = expenses
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
                    Math.Round((double)(amount / totalExpenses * 100), 1)
                );
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        return grouped;
    }
}
