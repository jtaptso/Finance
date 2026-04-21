using ClosedXML.Excel;
using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Infrastructure.Services;

public class ExcelParsingService : IExcelImportService
{
    private static readonly Dictionary<string, TransactionType> TypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SEPA Lastschrifteinzug"] = TransactionType.DirectDebit,
        ["SEPA Echtzeitüberweisung"] = TransactionType.InstantTransfer,
        ["SEPA Dauerauftrag"] = TransactionType.StandingOrder,
        ["SEPA Überweisung"] = TransactionType.BankTransfer,
        ["Kartenzahlung"] = TransactionType.CardPayment
    };

    private readonly ICategorizationService _categorization;
    private readonly IUnitOfWork _uow;

    public ExcelParsingService(ICategorizationService categorization, IUnitOfWork uow)
    {
        _categorization = categorization;
        _uow = uow;
    }

    public async Task<ImportPreviewDto> ParseAsync(
        Stream fileStream,
        string fileName,
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook(fileStream);
        var sheet = workbook.Worksheets.First();

        // Detect header row by looking for "Date" in column A (rows 1–10)
        int headerRow = 1;
        for (int r = 1; r <= 10; r++)
        {
            var cell = sheet.Cell(r, 1).GetString().Trim();
            if (cell.Equals("Date", StringComparison.OrdinalIgnoreCase) ||
                cell.Equals("Datum", StringComparison.OrdinalIgnoreCase))
            {
                headerRow = r;
                break;
            }
        }

        var rows = new List<ImportPreviewRowDto>();
        int dataStart = headerRow + 1;
        int lastRow = sheet.LastRowUsed()?.RowNumber() ?? dataStart;

        for (int r = dataStart; r <= lastRow; r++)
        {
            var rowNum = r - dataStart + 1;
            var dateCell = sheet.Cell(r, 1).GetString().Trim();
            var typeCell = sheet.Cell(r, 2).GetString().Trim();
            var amountCell = sheet.Cell(r, 3).GetString().Trim();
            var descCell = sheet.Cell(r, 4).GetString().Trim();

            if (string.IsNullOrWhiteSpace(dateCell) && string.IsNullOrWhiteSpace(descCell))
                continue;

            DateOnly? date = null;
            bool hasError = false;
            string? errorMsg = null;

            if (!DateOnly.TryParseExact(dateCell, "dd.MM.yyyy", out var parsedDate))
            {
                hasError = true;
                errorMsg = $"Cannot parse date '{dateCell}'";
            }
            else
            {
                date = parsedDate;
            }

            decimal? amount = null;
            if (!decimal.TryParse(amountCell.Replace(',', '.'), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var parsedAmount))
            {
                hasError = true;
                errorMsg ??= $"Cannot parse amount '{amountCell}'";
            }
            else
            {
                amount = parsedAmount;
            }

            var txType = TypeMap.TryGetValue(typeCell, out var mapped) ? mapped : TransactionType.Other;

            bool isDuplicate = false;
            Guid? categoryId = null;
            string? categoryName = null;

            if (!hasError && date.HasValue && amount.HasValue)
            {
                isDuplicate = await _uow.Transactions.ExistsAsync(date.Value, amount.Value, descCell, cancellationToken);
                var suggestion = await _categorization.SuggestCategoryAsync(descCell, cancellationToken);
                categoryId = suggestion.CategoryId;
                categoryName = suggestion.CategoryName;
            }

            rows.Add(new ImportPreviewRowDto(
                rowNum,
                date,
                txType,
                amount,
                descCell,
                descCell,
                categoryId,
                categoryName,
                isDuplicate,
                hasError,
                errorMsg
            ));
        }

        return new ImportPreviewDto(
            fileName,
            accountId,
            rows.Count,
            rows.Count(r => !r.HasParseError && !r.IsDuplicate),
            rows.Count(r => r.IsDuplicate),
            rows.Count(r => r.HasParseError),
            rows
        );
    }
}
