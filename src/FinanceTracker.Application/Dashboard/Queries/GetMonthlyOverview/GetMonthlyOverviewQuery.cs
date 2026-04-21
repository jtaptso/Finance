using FinanceTracker.Application.Dashboard.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Dashboard.Queries.GetMonthlyOverview;

public record GetMonthlyOverviewQuery(int Year, int Month) : IRequest<MonthlyOverviewDto>;

public class GetMonthlyOverviewHandler : IRequestHandler<GetMonthlyOverviewQuery, MonthlyOverviewDto>
{
    private readonly IUnitOfWork _uow;

    public GetMonthlyOverviewHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<MonthlyOverviewDto> Handle(GetMonthlyOverviewQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _uow.Transactions.GetByMonthAsync(request.Year, request.Month, cancellationToken);

        var income = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
        var expenses = transactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

        return new MonthlyOverviewDto(
            request.Year,
            request.Month,
            income,
            expenses,
            income - expenses,
            transactions.Count
        );
    }
}
