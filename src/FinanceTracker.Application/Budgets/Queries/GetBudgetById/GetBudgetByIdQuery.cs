using AutoMapper;
using FinanceTracker.Application.Budgets.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Budgets.Queries.GetBudgetById;

public record GetBudgetByIdQuery(Guid Id) : IRequest<BudgetDto?>;

public class GetBudgetByIdHandler : IRequestHandler<GetBudgetByIdQuery, BudgetDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetBudgetByIdHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<BudgetDto?> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var budget = await _uow.Budgets.GetByIdAsync(request.Id, cancellationToken);
        return budget is null ? null : _mapper.Map<BudgetDto>(budget);
    }
}
