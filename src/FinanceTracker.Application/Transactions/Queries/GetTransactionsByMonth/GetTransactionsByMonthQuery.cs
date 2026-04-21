using AutoMapper;
using FinanceTracker.Application.Transactions.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Transactions.Queries.GetTransactionsByMonth;

public record GetTransactionsByMonthQuery(int Year, int Month) : IRequest<IReadOnlyList<TransactionDto>>;

public class GetTransactionsByMonthHandler : IRequestHandler<GetTransactionsByMonthQuery, IReadOnlyList<TransactionDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetTransactionsByMonthHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TransactionDto>> Handle(GetTransactionsByMonthQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _uow.Transactions.GetByMonthAsync(request.Year, request.Month, cancellationToken);
        return _mapper.Map<IReadOnlyList<TransactionDto>>(transactions);
    }
}
