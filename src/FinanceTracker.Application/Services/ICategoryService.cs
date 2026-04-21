using FinanceTracker.Application.Common.Models;
using FinanceTracker.Application.Categories.Commands.CreateCategory;
using FinanceTracker.Application.Categories.Commands.UpdateCategory;
using FinanceTracker.Application.Categories.DTOs;

namespace FinanceTracker.Application.Services;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateCategoryCommand command, CancellationToken ct = default);
    Task<Result> UpdateAsync(UpdateCategoryCommand command, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
