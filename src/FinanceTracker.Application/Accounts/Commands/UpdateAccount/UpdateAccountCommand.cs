using FinanceTracker.Application.Common.Models;
using FinanceTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FinanceTracker.Application.Accounts.Commands.UpdateAccount;

public record UpdateAccountCommand(
    Guid Id,
    string Name,
    string? BankName,
    string? AccountNumber,
    string Currency,
    decimal InitialBalance,
    bool IsActive
) : IRequest<Result>;

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

public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public UpdateAccountCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _uow.Accounts.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
            return Result.Failure($"Account {request.Id} not found.");

        account.Name = request.Name;
        account.BankName = request.BankName;
        account.AccountNumber = request.AccountNumber;
        account.Currency = request.Currency;
        account.InitialBalance = request.InitialBalance;
        account.IsActive = request.IsActive;

        _uow.Accounts.Update(account);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
