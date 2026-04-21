using AutoMapper;
using FinanceTracker.Application.Budgets.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Budgets.Queries.GetAllBudgets;

public record GetAllBudgetsQuery : IRequest<IReadOnlyList<BudgetDto>>;

public class GetAllBudgetsHandler : IRequestHandler<GetAllBudgetsQuery, IReadOnlyList<BudgetDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllBudgetsHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BudgetDto>> Handle(GetAllBudgetsQuery request, CancellationToken cancellationToken)
    {
        var budgets = await _uow.Budgets.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<BudgetDto>>(budgets);
    }
}
