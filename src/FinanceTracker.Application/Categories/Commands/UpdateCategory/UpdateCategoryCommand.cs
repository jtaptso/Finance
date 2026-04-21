using FinanceTracker.Application.Common.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FinanceTracker.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    CategoryType Type,
    string? Icon,
    string? Color,
    Guid? ParentCategoryId,
    IReadOnlyList<string>? Keywords
) : IRequest<Result>;

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

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public UpdateCategoryCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _uow.Categories.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result.Failure($"Category {request.Id} not found.");

        category.Name = request.Name;
        category.Type = request.Type;
        category.Icon = request.Icon;
        category.Color = request.Color;
        category.ParentCategoryId = request.ParentCategoryId;

        // Replace keywords
        category.Keywords.Clear();
        if (request.Keywords is not null)
        {
            foreach (var keyword in request.Keywords)
            {
                category.Keywords.Add(new CategoryKeyword { Id = Guid.NewGuid(), Keyword = keyword, CategoryId = category.Id });
            }
        }

        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
