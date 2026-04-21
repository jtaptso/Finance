using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Budgets.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Budgets.Queries.GetAllBudgets;

public record GetAllBudgetsQuery : IRequest<IReadOnlyList<BudgetDto>>;

public class GetAllBudgetsHandler : IRequestHandler<GetAllBudgetsQuery, IReadOnlyList<BudgetDto>>
{
    private readonly IUnitOfWork _uow;
    public GetAllBudgetsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<BudgetDto>> Handle(GetAllBudgetsQuery request, CancellationToken cancellationToken)
    {
        var budgets = await _uow.Budgets.GetAllAsync(cancellationToken);
        return budgets.Select(b => b.ToDto()).ToList();
    }
}
