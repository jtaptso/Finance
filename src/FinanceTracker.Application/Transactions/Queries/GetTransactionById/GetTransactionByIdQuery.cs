using AutoMapper;
using FinanceTracker.Application.Transactions.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Transactions.Queries.GetTransactionById;

public record GetTransactionByIdQuery(Guid Id) : IRequest<TransactionDto?>;

public class GetTransactionByIdHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetTransactionByIdHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<TransactionDto?> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _uow.Transactions.GetByIdAsync(request.Id, cancellationToken);
        return transaction is null ? null : _mapper.Map<TransactionDto>(transaction);
    }
}
