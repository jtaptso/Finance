using FinanceTracker.Application.Common.Models;
using FinanceTracker.Domain.Enums;
using FluentValidation;

namespace FinanceTracker.Application.Budgets.Commands.UpdateBudget;

public record UpdateBudgetCommand(
    Guid Id,
    decimal Amount,
    BudgetPeriod Period,
    DateOnly StartDate,
    DateOnly? EndDate
);

public class UpdateBudgetCommandValidator : AbstractValidator<UpdateBudgetCommand>
{
    public UpdateBudgetCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.EndDate.HasValue);
    }
}

