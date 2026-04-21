namespace FinanceTracker.Application.Common.Interfaces;

/// <summary>
/// Attempts to match a transaction description to a category using stored keywords.
/// Returns null when no match is found.
/// </summary>
public interface ICategorizationService
{
    Task<(Guid? CategoryId, string? CategoryName)> SuggestCategoryAsync(
        string description,
        CancellationToken cancellationToken = default);
}
