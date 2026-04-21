using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Budgets.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Budgets.Queries.GetBudgetById;

public record GetBudgetByIdQuery(Guid Id) : IRequest<BudgetDto?>;

public class GetBudgetByIdHandler : IRequestHandler<GetBudgetByIdQuery, BudgetDto?>
{
    private readonly IUnitOfWork _uow;
    public GetBudgetByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<BudgetDto?> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var budget = await _uow.Budgets.GetByIdAsync(request.Id, cancellationToken);
        return budget?.ToDto();
    }
}
