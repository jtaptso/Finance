using FinanceTracker.Application.Common.Models;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FinanceTracker.Application.Budgets.Commands.UpdateBudget;

public record UpdateBudgetCommand(
    Guid Id,
    decimal Amount,
    BudgetPeriod Period,
    DateOnly StartDate,
    DateOnly? EndDate
) : IRequest<Result>;

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

public class UpdateBudgetCommandHandler : IRequestHandler<UpdateBudgetCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public UpdateBudgetCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _uow.Budgets.GetByIdAsync(request.Id, cancellationToken);
        if (budget is null)
            return Result.Failure($"Budget {request.Id} not found.");

        budget.Amount = request.Amount;
        budget.Period = request.Period;
        budget.StartDate = request.StartDate;
        budget.EndDate = request.EndDate;

        _uow.Budgets.Update(budget);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
