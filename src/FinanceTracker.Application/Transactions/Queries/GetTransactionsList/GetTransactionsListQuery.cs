using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Transactions.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Transactions.Queries.GetTransactionsList;

public record GetTransactionsListQuery : IRequest<IReadOnlyList<TransactionDto>>;

public class GetTransactionsListHandler : IRequestHandler<GetTransactionsListQuery, IReadOnlyList<TransactionDto>>
{
    private readonly IUnitOfWork _uow;
    public GetTransactionsListHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<TransactionDto>> Handle(GetTransactionsListQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _uow.Transactions.GetAllAsync(cancellationToken);
        return transactions.Select(t => t.ToDto()).ToList();
    }
}
