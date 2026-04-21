using FinanceTracker.Application.Common.Models;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FinanceTracker.Application.Transactions.Commands.UpdateTransaction;

public record UpdateTransactionCommand(
    Guid Id,
    DateOnly Date,
    TransactionType TransactionType,
    decimal Amount,
    string Description,
    Guid AccountId,
    Guid? CategoryId,
    string? Notes,
    bool IsRecurring,
    string Currency = "EUR"
) : IRequest<Result>;

public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommand>
{
    public UpdateTransactionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Amount).NotEqual(0).WithMessage("Amount cannot be zero.");
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(3);
        RuleFor(x => x.Notes).MaximumLength(1000).When(x => x.Notes is not null);
    }
}

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public UpdateTransactionCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _uow.Transactions.GetByIdAsync(request.Id, cancellationToken);
        if (transaction is null)
            return Result.Failure($"Transaction {request.Id} not found.");

        transaction.Date = request.Date;
        transaction.TransactionType = request.TransactionType;
        transaction.Amount = request.Amount;
        transaction.Currency = request.Currency;
        transaction.Description = request.Description;
        transaction.AccountId = request.AccountId;
        transaction.CategoryId = request.CategoryId;
        transaction.Notes = request.Notes;
        transaction.IsRecurring = request.IsRecurring;
        transaction.UpdatedAt = DateTime.UtcNow;

        _uow.Transactions.Update(transaction);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
