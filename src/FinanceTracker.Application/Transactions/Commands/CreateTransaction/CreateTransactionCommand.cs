using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

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
) : IRequest<Guid>;

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

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Guid>
{
    private readonly IUnitOfWork _uow;

    public CreateTransactionCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Date = request.Date,
            TransactionType = request.TransactionType,
            Amount = request.Amount,
            Currency = request.Currency,
            Description = request.Description,
            AccountId = request.AccountId,
            CategoryId = request.CategoryId,
            Notes = request.Notes,
            IsRecurring = request.IsRecurring,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _uow.Transactions.Add(transaction);
        await _uow.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}
