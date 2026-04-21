using FinanceTracker.Application.Common.Models;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IUnitOfWork _uow;

    public DeleteCategoryCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _uow.Categories.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result.Failure($"Category {request.Id} not found.");

        if (await _uow.Categories.HasTransactionsAsync(request.Id, cancellationToken))
            return Result.Failure("Cannot delete a category that has associated transactions. Reassign them first.");

        _uow.Categories.Remove(category);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
