using FinanceTracker.Application.Common.Models;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Budgets.Commands.DeleteBudget;

public record DeleteBudgetCommand(Guid Id) : IRequest<Result>;

public class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeleteBudgetCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _uow.Budgets.GetByIdAsync(request.Id, cancellationToken);
        if (budget is null)
            return Result.Failure($"Budget {request.Id} not found.");

        _uow.Budgets.Remove(budget);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
