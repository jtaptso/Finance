using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Transactions.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Transactions.Queries.GetTransactionsByMonth;

public record GetTransactionsByMonthQuery(int Year, int Month) : IRequest<IReadOnlyList<TransactionDto>>;

public class GetTransactionsByMonthHandler : IRequestHandler<GetTransactionsByMonthQuery, IReadOnlyList<TransactionDto>>
{
    private readonly IUnitOfWork _uow;

    public GetTransactionsByMonthHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<TransactionDto>> Handle(GetTransactionsByMonthQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _uow.Transactions.GetByMonthAsync(request.Year, request.Month, cancellationToken);
        return transactions.Select(t => t.ToDto()).ToList();
    }
}
