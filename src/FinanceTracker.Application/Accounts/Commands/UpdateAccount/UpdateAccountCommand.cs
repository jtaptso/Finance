using FinanceTracker.Application.Common.Models;
using FluentValidation;

namespace FinanceTracker.Application.Accounts.Commands.UpdateAccount;

public record UpdateAccountCommand(
    Guid Id,
    string Name,
    string? BankName,
    string? AccountNumber,
    string Currency,
    decimal InitialBalance,
    bool IsActive
);

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(10);
        RuleFor(x => x.BankName).MaximumLength(100).When(x => x.BankName is not null);
        RuleFor(x => x.AccountNumber).MaximumLength(50).When(x => x.AccountNumber is not null);
    }
}

