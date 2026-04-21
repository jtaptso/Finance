using AutoMapper;
using FinanceTracker.Application.Accounts.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Accounts.Queries.GetAccountById;

public record GetAccountByIdQuery(Guid Id) : IRequest<AccountDto?>;

public class GetAccountByIdHandler : IRequestHandler<GetAccountByIdQuery, AccountDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAccountByIdHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<AccountDto?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await _uow.Accounts.GetByIdAsync(request.Id, cancellationToken);
        return account is null ? null : _mapper.Map<AccountDto>(account);
    }
}
