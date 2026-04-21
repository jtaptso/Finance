using AutoMapper;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Application.Transactions.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Import.Queries.GetImportDetail;

public record GetImportDetailQuery(Guid ImportHistoryId) : IRequest<(ImportHistoryDto? Import, IReadOnlyList<TransactionDto> Transactions)>;

public class GetImportDetailHandler : IRequestHandler<GetImportDetailQuery, (ImportHistoryDto? Import, IReadOnlyList<TransactionDto> Transactions)>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetImportDetailHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<(ImportHistoryDto? Import, IReadOnlyList<TransactionDto> Transactions)> Handle(
        GetImportDetailQuery request, CancellationToken cancellationToken)
    {
        var importHistory = await _uow.ImportHistories.GetByIdAsync(request.ImportHistoryId, cancellationToken);
        if (importHistory is null)
            return (null, []);

        var transactions = await _uow.Transactions.GetByImportHistoryIdAsync(request.ImportHistoryId, cancellationToken);

        return (
            _mapper.Map<ImportHistoryDto>(importHistory),
            _mapper.Map<IReadOnlyList<TransactionDto>>(transactions)
        );
    }
}
