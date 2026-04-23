using ClosedXML.Excel;
using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;
using System.Globalization;
using System.Text;

namespace FinanceTracker.Infrastructure.Services;

public class ExcelParsingService : IExcelImportService
{
    private static readonly string[] DateFormats = ["dd.MM.yyyy", "d.M.yyyy"];

    private static readonly Dictionary<string, TransactionType> TypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SEPA Lastschrifteinzug"] = TransactionType.DirectDebit,
        ["SEPA Echtzeitüberweisung"] = TransactionType.InstantTransfer,
        ["SEPA Dauerauftrag"] = TransactionType.StandingOrder,
        ["SEPA Überweisung"] = TransactionType.BankTransfer,
        ["SEPA Überweisung (Dauerauftrag)"] = TransactionType.StandingOrder,
        ["SEPA Lastschrift"] = TransactionType.DirectDebit,
        ["SEPA Lastschrift (ELV)"] = TransactionType.DirectDebit,
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
        var extension = Path.GetExtension(fileName);

        List<ImportPreviewRowDto> rows = extension.Equals(".csv", StringComparison.OrdinalIgnoreCase)
            ? await ParseCsvRowsAsync(fileStream, cancellationToken)
            : await ParseExcelRowsAsync(fileStream, cancellationToken);

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

    private async Task<List<ImportPreviewRowDto>> ParseExcelRowsAsync(Stream fileStream, CancellationToken cancellationToken)
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

            var mappedRow = await MapRowAsync(rowNum, dateCell, typeCell, amountCell, descCell, cancellationToken);
            rows.Add(mappedRow);
        }

        return rows;
    }

    private async Task<List<ImportPreviewRowDto>> ParseCsvRowsAsync(Stream fileStream, CancellationToken cancellationToken)
    {
        var rows = new List<ImportPreviewRowDto>();

        if (fileStream.CanSeek)
            fileStream.Position = 0;
        using var reader = new StreamReader(fileStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);

        int dataRowNumber = 0;
        while (true)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line is null)
                break;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (IsNonDataCsvLine(line))
                continue;

            var parts = line.Split(';');
            if (parts.Length < 12)
                continue;

            var csvRow = new CsvImportRow(
                BookingDate: parts.ElementAtOrDefault(0)?.Trim() ?? string.Empty,
                TransactionType: parts.ElementAtOrDefault(2)?.Trim() ?? string.Empty,
                Payee: parts.ElementAtOrDefault(3)?.Trim() ?? string.Empty,
                Purpose: parts.ElementAtOrDefault(4)?.Trim() ?? string.Empty,
                Amount: parts.ElementAtOrDefault(11)?.Trim() ?? string.Empty
            );

            dataRowNumber++;
            var description = string.IsNullOrWhiteSpace(csvRow.Purpose)
                ? csvRow.Payee
                : csvRow.Purpose;

            if (string.IsNullOrWhiteSpace(csvRow.BookingDate) && string.IsNullOrWhiteSpace(description))
                continue;

            var mappedRow = await MapRowAsync(
                dataRowNumber,
                csvRow.BookingDate,
                csvRow.TransactionType,
                csvRow.Amount,
                description,
                cancellationToken);

            rows.Add(mappedRow);
        }

        return rows;
    }

    private static bool IsNonDataCsvLine(string line)
    {
        var trimmed = line.Trim();
        if (trimmed.StartsWith("Umsätze", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("Konto;", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("AktivKonto;", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("Letzter Kontostand", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("Vorgemerkte und noch nicht gebuchte Umsätze", StringComparison.OrdinalIgnoreCase) ||
            trimmed.StartsWith("Buchungstag;", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return !trimmed.Contains(';');
    }

    private async Task<ImportPreviewRowDto> MapRowAsync(
        int rowNumber,
        string rawDate,
        string rawType,
        string rawAmount,
        string description,
        CancellationToken cancellationToken)
    {
        DateOnly? date = null;
        bool hasError = false;
        string? errorMsg = null;

        if (!DateOnly.TryParseExact(rawDate, DateFormats, CultureInfo.GetCultureInfo("de-DE"), DateTimeStyles.None, out var parsedDate))
        {
            hasError = true;
            errorMsg = $"Cannot parse date '{rawDate}'";
        }
        else
        {
            date = parsedDate;
        }

        decimal? amount = null;
        if (!TryParseAmount(rawAmount, out var parsedAmount))
        {
            hasError = true;
            errorMsg ??= $"Cannot parse amount '{rawAmount}'";
        }
        else
        {
            amount = parsedAmount;
        }

        var txType = MapTransactionType(rawType);

        bool isDuplicate = false;
        Guid? categoryId = null;
        string? categoryName = null;

        if (!hasError && date.HasValue && amount.HasValue)
        {
            isDuplicate = await _uow.Transactions.ExistsAsync(date.Value, amount.Value, description, cancellationToken);
            var suggestion = await _categorization.SuggestCategoryAsync(description, cancellationToken);
            categoryId = suggestion.CategoryId;
            categoryName = suggestion.CategoryName;
        }

        return new ImportPreviewRowDto(
            rowNumber,
            date,
            txType,
            amount,
            description,
            description,
            categoryId,
            categoryName,
            isDuplicate,
            hasError,
            errorMsg
        );
    }

    private static TransactionType MapTransactionType(string rawType)
    {
        if (TypeMap.TryGetValue(rawType, out var mapped))
            return mapped;

        if (rawType.StartsWith("SEPA Überweisung", StringComparison.OrdinalIgnoreCase))
            return TransactionType.BankTransfer;

        if (rawType.StartsWith("SEPA Lastschrift", StringComparison.OrdinalIgnoreCase))
            return TransactionType.DirectDebit;

        return TransactionType.Other;
    }

    private static bool TryParseAmount(string rawAmount, out decimal amount)
    {
        var normalized = rawAmount.Trim();

        if (decimal.TryParse(normalized, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.GetCultureInfo("de-DE"), out amount))
            return true;

        normalized = normalized.Replace(".", string.Empty).Replace(',', '.');
        return decimal.TryParse(normalized, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out amount);
    }

    private sealed record CsvImportRow(
        string BookingDate,
        string TransactionType,
        string Payee,
        string Purpose,
        string Amount);
}
