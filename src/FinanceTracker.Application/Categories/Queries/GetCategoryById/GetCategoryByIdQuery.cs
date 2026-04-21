using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Categories.DTOs;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto?>;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly IUnitOfWork _uow;
    public GetCategoryByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _uow.Categories.GetByIdAsync(request.Id, cancellationToken);
        return category?.ToDto();
    }
}
