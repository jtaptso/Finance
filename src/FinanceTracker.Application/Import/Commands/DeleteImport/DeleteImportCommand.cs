using FinanceTracker.Application.Common.Models;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Import.Commands.DeleteImport;

/// <summary>
/// Undoes an import — deletes the ImportHistory record and all its associated transactions.
/// </summary>
public record DeleteImportCommand(Guid ImportHistoryId) : IRequest<Result>;

public class DeleteImportCommandHandler : IRequestHandler<DeleteImportCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeleteImportCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteImportCommand request, CancellationToken cancellationToken)
    {
        var importHistory = await _uow.ImportHistories.GetByIdAsync(request.ImportHistoryId, cancellationToken);
        if (importHistory is null)
            return Result.Failure($"Import {request.ImportHistoryId} not found.");

        var transactions = await _uow.Transactions.GetByImportHistoryIdAsync(request.ImportHistoryId, cancellationToken);
        _uow.Transactions.RemoveRange(transactions);
        _uow.ImportHistories.Remove(importHistory);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
