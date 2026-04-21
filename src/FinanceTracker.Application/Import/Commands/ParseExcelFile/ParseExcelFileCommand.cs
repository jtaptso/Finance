using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Import.DTOs;
using MediatR;

namespace FinanceTracker.Application.Import.Commands.ParseExcelFile;

/// <summary>
/// Step 2: Parse the uploaded Excel file and return a preview — nothing is saved.
/// </summary>
public record ParseExcelFileCommand(
    Stream FileStream,
    string FileName,
    Guid AccountId
) : IRequest<ImportPreviewDto>;

public class ParseExcelFileCommandHandler : IRequestHandler<ParseExcelFileCommand, ImportPreviewDto>
{
    private readonly IExcelImportService _excelImportService;

    public ParseExcelFileCommandHandler(IExcelImportService excelImportService)
    {
        _excelImportService = excelImportService;
    }

    public async Task<ImportPreviewDto> Handle(ParseExcelFileCommand request, CancellationToken cancellationToken)
    {
        return await _excelImportService.ParseAsync(
            request.FileStream,
            request.FileName,
            request.AccountId,
            cancellationToken);
    }
}
