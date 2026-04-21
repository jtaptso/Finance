using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Common.Models;
using FinanceTracker.Application.Import.Commands.ConfirmImport;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Application.Services;
using FinanceTracker.Application.Transactions.DTOs;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Infrastructure.Services;

public class ImportService : IImportService
{
    private readonly IUnitOfWork _uow;
    private readonly IExcelImportService _excelImportService;

    public ImportService(IUnitOfWork uow, IExcelImportService excelImportService)
    {
        _uow = uow;
        _excelImportService = excelImportService;
    }

    public async Task<IReadOnlyList<ImportHistoryDto>> GetHistoryAsync(CancellationToken ct = default)
    {
        var imports = await _uow.ImportHistories.GetAllAsync(ct);
        return imports.Select(i => i.ToDto()).ToList();
    }

    public async Task<(ImportHistoryDto? Import, IReadOnlyList<TransactionDto> Transactions)> GetDetailAsync(Guid importHistoryId, CancellationToken ct = default)
    {
        var importHistory = await _uow.ImportHistories.GetByIdAsync(importHistoryId, ct);
        if (importHistory is null)
            return (null, []);

        var transactions = await _uow.Transactions.GetByImportHistoryIdAsync(importHistoryId, ct);
        return (importHistory.ToDto(), transactions.Select(t => t.ToDto()).ToList());
    }

    public Task<ImportPreviewDto> ParseExcelAsync(Stream fileStream, string fileName, Guid accountId, CancellationToken ct = default)
        => _excelImportService.ParseAsync(fileStream, fileName, accountId, ct);

    public async Task<Result<Guid>> ConfirmAsync(ConfirmImportCommand cmd, CancellationToken ct = default)
    {
        var importHistory = new ImportHistory
        {
            Id = Guid.NewGuid(),
            FileName = cmd.FileName,
            AccountId = cmd.AccountId,
            ImportedAt = DateTime.UtcNow,
            TotalRows = cmd.TotalRows,
            ImportedRows = cmd.SelectedRows.Count,
            SkippedRows = cmd.TotalRows - cmd.SelectedRows.Count,
            Month = cmd.SelectedRows.Count > 0
                ? new DateOnly(cmd.SelectedRows[0].Date.Year, cmd.SelectedRows[0].Date.Month, 1)
                : DateOnly.FromDateTime(DateTime.UtcNow),
            Status = ImportStatus.Completed
        };

        var transactions = cmd.SelectedRows.Select(row => new Transaction
        {
            Id = Guid.NewGuid(),
            Date = row.Date,
            TransactionType = row.TransactionType,
            Amount = row.Amount,
            Description = row.Description,
            OriginalDescription = row.OriginalDescription,
            CategoryId = row.CategoryId,
            AccountId = cmd.AccountId,
            ImportHistoryId = importHistory.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        _uow.ImportHistories.Add(importHistory);
        _uow.Transactions.AddRange(transactions);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(importHistory.Id);
    }

    public async Task<Result> DeleteAsync(Guid importHistoryId, CancellationToken ct = default)
    {
        var importHistory = await _uow.ImportHistories.GetByIdAsync(importHistoryId, ct);
        if (importHistory is null)
            return Result.Failure($"Import {importHistoryId} not found.");

        var transactions = await _uow.Transactions.GetByImportHistoryIdAsync(importHistoryId, ct);
        _uow.Transactions.RemoveRange(transactions);
        _uow.ImportHistories.Remove(importHistory);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}
