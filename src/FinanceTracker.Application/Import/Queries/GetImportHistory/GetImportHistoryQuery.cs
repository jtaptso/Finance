using AutoMapper;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Import.Queries.GetImportHistory;

public record GetImportHistoryQuery : IRequest<IReadOnlyList<ImportHistoryDto>>;

public class GetImportHistoryHandler : IRequestHandler<GetImportHistoryQuery, IReadOnlyList<ImportHistoryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetImportHistoryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ImportHistoryDto>> Handle(GetImportHistoryQuery request, CancellationToken cancellationToken)
    {
        var imports = await _uow.ImportHistories.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ImportHistoryDto>>(imports);
    }
}
