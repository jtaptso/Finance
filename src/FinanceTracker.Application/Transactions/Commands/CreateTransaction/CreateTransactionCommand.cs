using FinanceTracker.Domain.Enums;
using FluentValidation;

namespace FinanceTracker.Application.Transactions.Commands.CreateTransaction;

public record CreateTransactionCommand(
    DateOnly Date,
    TransactionType TransactionType,
    decimal Amount,
    string Description,
    Guid AccountId,
    Guid? CategoryId,
    string? Notes,
    bool IsRecurring = false,
    string Currency = "EUR"
);

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Amount).NotEqual(0).WithMessage("Amount cannot be zero.");
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(3);
        RuleFor(x => x.Notes).MaximumLength(1000).When(x => x.Notes is not null);
    }
}

