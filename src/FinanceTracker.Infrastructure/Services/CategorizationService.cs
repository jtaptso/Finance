using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Infrastructure.Services;

public class CategorizationService : ICategorizationService
{
    private readonly IUnitOfWork _uow;

    public CategorizationService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<(Guid? CategoryId, string? CategoryName)> SuggestCategoryAsync(
        string description,
        CancellationToken cancellationToken = default)
    {
        var categories = await _uow.Categories.GetAllWithKeywordsAsync(cancellationToken);
        var upper = description.ToUpperInvariant();

        foreach (var category in categories)
        {
            foreach (var kw in category.Keywords)
            {
                if (upper.Contains(kw.Keyword.ToUpperInvariant()))
                    return (category.Id, category.Name);
            }
        }

        return (null, null);
    }
}
