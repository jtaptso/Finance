using FinanceTracker.Application.Dashboard.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Dashboard.Queries.GetCashFlowTrend;

public record GetCashFlowTrendQuery(int Months = 6) : IRequest<CashFlowTrendDto>;

public class GetCashFlowTrendHandler : IRequestHandler<GetCashFlowTrendQuery, CashFlowTrendDto>
{
    private readonly IUnitOfWork _uow;

    public GetCashFlowTrendHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<CashFlowTrendDto> Handle(GetCashFlowTrendQuery request, CancellationToken cancellationToken)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = new List<IncomeVsExpensesItemDto>();

        for (int i = request.Months - 1; i >= 0; i--)
        {
            var targetDate = now.AddMonths(-i);
            var transactions = await _uow.Transactions.GetByMonthAsync(targetDate.Year, targetDate.Month, cancellationToken);

            var income = transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
            var expenses = transactions.Where(t => t.Amount < 0).Sum(t => Math.Abs(t.Amount));

            result.Add(new IncomeVsExpensesItemDto(targetDate.Year, targetDate.Month, income, expenses, income - expenses));
        }

        return new CashFlowTrendDto(result);
    }
}
