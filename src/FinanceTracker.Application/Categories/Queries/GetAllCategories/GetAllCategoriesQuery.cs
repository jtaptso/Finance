using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Categories.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;

public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly IUnitOfWork _uow;
    public GetAllCategoriesHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _uow.Categories.GetAllWithKeywordsAsync(cancellationToken);
        return categories.Select(c => c.ToDto()).ToList();
    }
}
