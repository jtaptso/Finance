using FinanceTracker.Application.Common.Models;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Domain.Enums;
using FluentValidation;

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
);

public class ConfirmImportCommandValidator : AbstractValidator<ConfirmImportCommand>
{
    public ConfirmImportCommandValidator()
    {
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.AccountId).NotEmpty();
        RuleFor(x => x.SelectedRows).NotEmpty().WithMessage("At least one row must be selected.");
    }
}

