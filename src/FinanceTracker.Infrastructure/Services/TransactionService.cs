using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Common.Models;
using FinanceTracker.Application.Services;
using FinanceTracker.Application.Transactions.Commands.CreateTransaction;
using FinanceTracker.Application.Transactions.Commands.UpdateTransaction;
using FinanceTracker.Application.Transactions.DTOs;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Infrastructure.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _uow;
    public TransactionService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<TransactionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var transactions = await _uow.Transactions.GetAllAsync(ct);
        return transactions.Select(t => t.ToDto()).ToList();
    }

    public async Task<IReadOnlyList<TransactionDto>> GetByMonthAsync(int year, int month, CancellationToken ct = default)
    {
        var transactions = await _uow.Transactions.GetByMonthAsync(year, month, ct);
        return transactions.Select(t => t.ToDto()).ToList();
    }

    public async Task<TransactionDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var transaction = await _uow.Transactions.GetByIdAsync(id, ct);
        return transaction?.ToDto();
    }

    public async Task<Guid> CreateAsync(CreateTransactionCommand cmd, CancellationToken ct = default)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Date = cmd.Date,
            TransactionType = cmd.TransactionType,
            Amount = cmd.Amount,
            Currency = cmd.Currency,
            Description = cmd.Description,
            AccountId = cmd.AccountId,
            CategoryId = cmd.CategoryId,
            Notes = cmd.Notes,
            IsRecurring = cmd.IsRecurring,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _uow.Transactions.Add(transaction);
        await _uow.SaveChangesAsync(ct);
        return transaction.Id;
    }

    public async Task<Result> UpdateAsync(UpdateTransactionCommand cmd, CancellationToken ct = default)
    {
        var transaction = await _uow.Transactions.GetByIdAsync(cmd.Id, ct);
        if (transaction is null)
            return Result.Failure($"Transaction {cmd.Id} not found.");

        transaction.Date = cmd.Date;
        transaction.TransactionType = cmd.TransactionType;
        transaction.Amount = cmd.Amount;
        transaction.Currency = cmd.Currency;
        transaction.Description = cmd.Description;
        transaction.AccountId = cmd.AccountId;
        transaction.CategoryId = cmd.CategoryId;
        transaction.Notes = cmd.Notes;
        transaction.IsRecurring = cmd.IsRecurring;
        transaction.UpdatedAt = DateTime.UtcNow;

        _uow.Transactions.Update(transaction);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var transaction = await _uow.Transactions.GetByIdAsync(id, ct);
        if (transaction is null)
            return Result.Failure($"Transaction {id} not found.");

        _uow.Transactions.Remove(transaction);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
