using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Import.DTOs;

public record ImportPreviewRowDto(
    int RowNumber,
    DateOnly? Date,
    TransactionType TransactionType,
    decimal? Amount,
    string Description,
    string OriginalDescription,
    Guid? SuggestedCategoryId,
    string? SuggestedCategoryName,
    bool IsDuplicate,
    bool HasParseError,
    string? ParseErrorMessage
);

public record ImportPreviewDto(
    string FileName,
    Guid AccountId,
    int TotalRows,
    int ReadyRows,
    int DuplicateRows,
    int ErrorRows,
    IReadOnlyList<ImportPreviewRowDto> Rows
);

public record ImportHistoryDto(
    Guid Id,
    string FileName,
    DateTime ImportedAt,
    int TotalRows,
    int ImportedRows,
    int SkippedRows,
    DateOnly Month,
    ImportStatus Status,
    string? ErrorMessage,
    Guid AccountId,
    string AccountName
);

public record ColumnMappingDto(
    int DateColumnIndex,
    int TransactionTypeColumnIndex,
    int AmountColumnIndex,
    int DescriptionColumnIndex,
    int HeaderRowIndex
);
