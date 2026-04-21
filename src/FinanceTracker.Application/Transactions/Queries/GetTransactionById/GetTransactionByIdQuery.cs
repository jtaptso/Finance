using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Transactions.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Transactions.Queries.GetTransactionById;

public record GetTransactionByIdQuery(Guid Id) : IRequest<TransactionDto?>;

public class GetTransactionByIdHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    private readonly IUnitOfWork _uow;
    public GetTransactionByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<TransactionDto?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _uow.Transactions.GetByIdAsync(request.Id, cancellationToken);
        return transaction?.ToDto();
    }
}
