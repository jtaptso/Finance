using FinanceTracker.Application.Categories.Commands.CreateCategory;
using FinanceTracker.Application.Categories.Commands.UpdateCategory;
using FinanceTracker.Application.Categories.DTOs;
using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Common.Models;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    public CategoryService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _uow.Categories.GetAllWithKeywordsAsync(ct);
        return categories.Select(c => c.ToDto()).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _uow.Categories.GetByIdAsync(id, ct);
        return category?.ToDto();
    }

    public async Task<Guid> CreateAsync(CreateCategoryCommand cmd, CancellationToken ct = default)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = cmd.Name,
            Type = cmd.Type,
            Icon = cmd.Icon,
            Color = cmd.Color,
            ParentCategoryId = cmd.ParentCategoryId,
            IsDefault = false,
            Keywords = cmd.Keywords?
                .Select(k => new CategoryKeyword { Id = Guid.NewGuid(), Keyword = k })
                .ToList() ?? []
        };
        _uow.Categories.Add(category);
        await _uow.SaveChangesAsync(ct);
        return category.Id;
    }

    public async Task<Result> UpdateAsync(UpdateCategoryCommand cmd, CancellationToken ct = default)
    {
        var category = await _uow.Categories.GetByIdAsync(cmd.Id, ct);
        if (category is null)
            return Result.Failure($"Category {cmd.Id} not found.");

        category.Name = cmd.Name;
        category.Type = cmd.Type;
        category.Icon = cmd.Icon;
        category.Color = cmd.Color;
        category.ParentCategoryId = cmd.ParentCategoryId;

        category.Keywords.Clear();
        if (cmd.Keywords is not null)
        {
            foreach (var keyword in cmd.Keywords)
                category.Keywords.Add(new CategoryKeyword { Id = Guid.NewGuid(), Keyword = keyword, CategoryId = category.Id });
        }

        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _uow.Categories.GetByIdAsync(id, ct);
        if (category is null)
            return Result.Failure($"Category {id} not found.");

        if (await _uow.Categories.HasTransactionsAsync(id, ct))
            return Result.Failure("Cannot delete a category that has associated transactions. Reassign them first.");

        _uow.Categories.Remove(category);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
