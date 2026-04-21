using FinanceTracker.Application.Common.Models;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Accounts.Commands.DeleteAccount;

public record DeleteAccountCommand(Guid Id) : IRequest<Result>;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeleteAccountCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _uow.Accounts.GetByIdAsync(request.Id, cancellationToken);
        if (account is null)
            return Result.Failure($"Account {request.Id} not found.");

        _uow.Accounts.Remove(account);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
