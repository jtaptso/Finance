using FinanceTracker.Application.Common.Models;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace FinanceTracker.Application.Import.Commands.ConfirmImport;

/// <summary>
/// Confirmed import row — sent from the UI after the user reviews the preview and checks which rows to import.
/// </summary>
public record ConfirmedImportRowDto(
    int RowNumber,
    DateOnly Date,
    TransactionType TransactionType,
    decimal Amount,
    string Description,
    string OriginalDescription,
    Guid? CategoryId
);

/// <summary>
/// Step 4: Persist the confirmed rows and create an ImportHistory record.
/// </summary>
public record ConfirmImportCommand(
    string FileName,
    Guid AccountId,
    int TotalRows,
    IReadOnlyList<ConfirmedImportRowDto> SelectedRows
) : IRequest<Result<Guid>>;

public class ConfirmImportCommandValidator : AbstractValidator<ConfirmImportCommand>
{
    public ConfirmImportCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.SelectedRows).NotEmpty().WithMessage("At least one row must be selected.");
    }
}

public class ConfirmImportCommandHandler : IRequestHandler<ConfirmImportCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;

    public ConfirmImportCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(ConfirmImportCommand request, CancellationToken cancellationToken)
    {
        var importHistory = new ImportHistory
        {
            Id = Guid.NewGuid(),
            FileName = request.FileName,
            AccountId = request.AccountId,
            ImportedAt = DateTime.UtcNow,
            TotalRows = request.TotalRows,
            ImportedRows = request.SelectedRows.Count,
            SkippedRows = request.TotalRows - request.SelectedRows.Count,
            Month = request.SelectedRows.Count > 0
                ? new DateOnly(request.SelectedRows[0].Date.Year, request.SelectedRows[0].Date.Month, 1)
                : DateOnly.FromDateTime(DateTime.UtcNow),
            Status = ImportStatus.Completed
        };

        var transactions = request.SelectedRows.Select(row => new Transaction
        {
            Id = Guid.NewGuid(),
            Date = row.Date,
            TransactionType = row.TransactionType,
            Amount = row.Amount,
            Description = row.Description,
            OriginalDescription = row.OriginalDescription,
            CategoryId = row.CategoryId,
            AccountId = request.AccountId,
            ImportHistoryId = importHistory.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        _uow.ImportHistories.Add(importHistory);
        _uow.Transactions.AddRange(transactions);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(importHistory.Id);
    }
}
