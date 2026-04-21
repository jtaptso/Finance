using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FinanceTracker.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    CategoryType Type,
    string? Icon,
    string? Color,
    Guid? ParentCategoryId,
    IReadOnlyList<string>? Keywords
) : IRequest<Guid>;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Icon).MaximumLength(50).When(x => x.Icon is not null);
        RuleFor(x => x.Color).MaximumLength(20).When(x => x.Color is not null);
    }
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IUnitOfWork _uow;

    public CreateCategoryCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Type = request.Type,
            Icon = request.Icon,
            Color = request.Color,
            ParentCategoryId = request.ParentCategoryId,
            IsDefault = false,
            Keywords = request.Keywords?
                .Select(k => new CategoryKeyword { Id = Guid.NewGuid(), Keyword = k })
                .ToList() ?? []
        };

        _uow.Categories.Add(category);
        await _uow.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
