using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FinanceTracker.Application.Accounts.Commands.CreateAccount;

public record CreateAccountCommand(
    string Name,
    string? BankName,
    string? AccountNumber,
    string Currency,
    decimal InitialBalance
) : IRequest<Guid>;

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

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
{
    private readonly IUnitOfWork _uow;

    public CreateAccountCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            BankName = request.BankName,
            AccountNumber = request.AccountNumber,
            Currency = request.Currency,
            InitialBalance = request.InitialBalance,
            IsActive = true
        };

        _uow.Accounts.Add(account);
        await _uow.SaveChangesAsync(cancellationToken);

        return account.Id;
    }
}
