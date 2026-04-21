using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Categories.DTOs;

public record CategoryDto(
    Guid Id,
    string Name,
    CategoryType Type,
    string? Icon,
    string? Color,
    bool IsDefault,
    Guid? ParentCategoryId,
    string? ParentCategoryName,
    IReadOnlyList<string> Keywords
);
