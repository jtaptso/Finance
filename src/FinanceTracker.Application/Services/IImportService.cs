using FinanceTracker.Application.Common.Models;
using FinanceTracker.Application.Import.Commands.ConfirmImport;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Application.Transactions.DTOs;

namespace FinanceTracker.Application.Services;

public interface IImportService
{
    Task<IReadOnlyList<ImportHistoryDto>> GetHistoryAsync(CancellationToken ct = default);
    Task<(ImportHistoryDto? Import, IReadOnlyList<TransactionDto> Transactions)> GetDetailAsync(Guid importHistoryId, CancellationToken ct = default);
    Task<ImportPreviewDto> ParseExcelAsync(Stream fileStream, string fileName, Guid accountId, CancellationToken ct = default);
    Task<Result<Guid>> ConfirmAsync(ConfirmImportCommand command, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid importHistoryId, CancellationToken ct = default);
}
