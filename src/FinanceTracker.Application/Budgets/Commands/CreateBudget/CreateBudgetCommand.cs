using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FinanceTracker.Application.Budgets.Commands.CreateBudget;

public record CreateBudgetCommand(
    Guid CategoryId,
    decimal Amount,
    BudgetPeriod Period,
    DateOnly StartDate,
    DateOnly? EndDate
) : IRequest<Guid>;

public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue);
    }
}

public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, Guid>
{
    private readonly IUnitOfWork _uow;

    public CreateBudgetCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = new Budget
        {
            Id = Guid.NewGuid(),
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Period = request.Period,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        _uow.Budgets.Add(budget);
        await _uow.SaveChangesAsync(cancellationToken);

        return budget.Id;
    }
}
