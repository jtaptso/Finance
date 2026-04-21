using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Accounts.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Accounts.Queries.GetAllAccounts;

public record GetAllAccountsQuery : IRequest<IReadOnlyList<AccountDto>>;

public class GetAllAccountsHandler : IRequestHandler<GetAllAccountsQuery, IReadOnlyList<AccountDto>>
{
    private readonly IUnitOfWork _uow;
    public GetAllAccountsHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<AccountDto>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
    {
        var accounts = await _uow.Accounts.GetAllAsync(cancellationToken);
        return accounts.Select(a => a.ToDto()).ToList();
    }
}
