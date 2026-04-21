using FinanceTracker.Application.Common.Models;
using FinanceTracker.Domain.Enums;
using FluentValidation;

namespace FinanceTracker.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    CategoryType Type,
    string? Icon,
    string? Color,
    Guid? ParentCategoryId,
    IReadOnlyList<string>? Keywords
);

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Icon).MaximumLength(50).When(x => x.Icon is not null);
        RuleFor(x => x.Color).MaximumLength(20).When(x => x.Color is not null);
    }
}
