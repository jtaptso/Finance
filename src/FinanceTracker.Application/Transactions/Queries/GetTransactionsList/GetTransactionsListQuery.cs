using AutoMapper;
using FinanceTracker.Application.Transactions.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Transactions.Queries.GetTransactionsList;

public record GetTransactionsListQuery : IRequest<IReadOnlyList<TransactionDto>>;

public class GetTransactionsListHandler : IRequestHandler<GetTransactionsListQuery, IReadOnlyList<TransactionDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetTransactionsListHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TransactionDto>> Handle(GetTransactionsListQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _uow.Transactions.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<TransactionDto>>(transactions);
    }
}
