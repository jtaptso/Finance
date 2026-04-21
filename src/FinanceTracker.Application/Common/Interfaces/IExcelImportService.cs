using FinanceTracker.Application.Import.DTOs;

namespace FinanceTracker.Application.Common.Interfaces;

/// <summary>
/// Parses an Excel (.xlsx) byte stream into an import preview without persisting anything.
/// </summary>
public interface IExcelImportService
{
    Task<ImportPreviewDto> ParseAsync(
        Stream fileStream,
        string fileName,
        Guid accountId,
        CancellationToken cancellationToken = default);
}
