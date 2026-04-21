using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Import.Queries.GetImportHistory;

public record GetImportHistoryQuery : IRequest<IReadOnlyList<ImportHistoryDto>>;

public class GetImportHistoryHandler : IRequestHandler<GetImportHistoryQuery, IReadOnlyList<ImportHistoryDto>>
{
    private readonly IUnitOfWork _uow;
    public GetImportHistoryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<ImportHistoryDto>> Handle(GetImportHistoryQuery request, CancellationToken cancellationToken)
    {
        var imports = await _uow.ImportHistories.GetAllAsync(cancellationToken);
        return imports.Select(i => i.ToDto()).ToList();
    }
}
