using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Accounts.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Accounts.Queries.GetAccountById;

public record GetAccountByIdQuery(Guid Id) : IRequest<AccountDto?>;

public class GetAccountByIdHandler : IRequestHandler<GetAccountByIdQuery, AccountDto?>
{
    private readonly IUnitOfWork _uow;
    public GetAccountByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<AccountDto?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await _uow.Accounts.GetByIdAsync(request.Id, cancellationToken);
        return account?.ToDto();
    }
}
