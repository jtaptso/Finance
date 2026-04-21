using AutoMapper;
using FinanceTracker.Application.Categories.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;

public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllCategoriesHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _uow.Categories.GetAllWithKeywordsAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
    }
}
