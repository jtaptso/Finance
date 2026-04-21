using FinanceTracker.Domain.Enums;
using FluentValidation;

namespace FinanceTracker.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    CategoryType Type,
    string? Icon,
    string? Color,
    Guid? ParentCategoryId,
    IReadOnlyList<string>? Keywords
);

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Icon).MaximumLength(50).When(x => x.Icon is not null);
        RuleFor(x => x.Color).MaximumLength(20).When(x => x.Color is not null);
    }
}
