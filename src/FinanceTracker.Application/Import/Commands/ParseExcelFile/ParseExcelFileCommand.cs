using FinanceTracker.Application.Import.DTOs;

namespace FinanceTracker.Application.Import.Commands.ParseExcelFile;

/// <summary>
/// Step 2: Parse the uploaded Excel file and return a preview — nothing is saved.
/// </summary>
public record ParseExcelFileCommand(
    Stream FileStream,
    string FileName,
    Guid AccountId
);

