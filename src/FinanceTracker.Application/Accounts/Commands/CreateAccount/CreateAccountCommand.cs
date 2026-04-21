using FluentValidation;

namespace FinanceTracker.Application.Accounts.Commands.CreateAccount;

public record CreateAccountCommand(
    string Name,
    string? BankName,
    string? AccountNumber,
    string Currency,
    decimal InitialBalance
);

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        RuleFor(x => x.BankName).MaximumLength(100).When(x => x.BankName is not null);
        RuleFor(x => x.AccountNumber).MaximumLength(50).When(x => x.AccountNumber is not null);
    }
}

