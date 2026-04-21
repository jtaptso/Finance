using FinanceTracker.Application.Common.Models;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Transactions.Commands.DeleteTransaction;

public record DeleteTransactionCommand(Guid Id) : IRequest<Result>;

public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeleteTransactionCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _uow.Transactions.GetByIdAsync(request.Id, cancellationToken);
        if (transaction is null)
            return Result.Failure($"Transaction {request.Id} not found.");

        _uow.Transactions.Remove(transaction);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
