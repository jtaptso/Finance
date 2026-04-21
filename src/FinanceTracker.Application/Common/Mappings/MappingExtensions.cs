using FinanceTracker.Application.Accounts.DTOs;
using FinanceTracker.Application.Budgets.DTOs;
using FinanceTracker.Application.Categories.DTOs;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Application.Transactions.DTOs;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Common.Mappings;

public static class MappingExtensions
{
    public static TransactionDto ToDto(this Transaction src) => new(
        src.Id,
        src.Date,
        src.TransactionType,
        src.TransactionType.ToString(),
        src.Amount,
        src.Currency,
        src.Description,
        src.Notes,
        src.IsRecurring,
        src.AccountId,
        src.Account != null ? src.Account.Name : string.Empty,
        src.CategoryId,
        src.Category?.Name,
        src.Category?.Color,
        src.ImportHistoryId,
        src.CreatedAt,
        src.UpdatedAt
    );

    public static CategoryDto ToDto(this Category src) => new(
        src.Id,
        src.Name,
        src.Type,
        src.Icon,
        src.Color,
        src.IsDefault,
        src.ParentCategoryId,
        src.ParentCategory?.Name,
        src.Keywords.Select(k => k.Keyword).ToList()
    );

    public static AccountDto ToDto(this Account src) => new(
        src.Id,
        src.Name,
        src.BankName,
        src.AccountNumber,
        src.Currency,
        src.InitialBalance,
        src.CurrentBalance,
        src.IsActive
    );

    public static BudgetDto ToDto(this Budget src) => new(
        src.Id,
        src.CategoryId,
        src.Category != null ? src.Category.Name : string.Empty,
        src.Category?.Color,
        src.Amount,
        src.Period,
        src.StartDate,
        src.EndDate
    );

    public static ImportHistoryDto ToDto(this ImportHistory src) => new(
        src.Id,
        src.FileName,
        src.ImportedAt,
        src.TotalRows,
        src.ImportedRows,
        src.SkippedRows,
        src.Month,
        src.Status,
        src.ErrorMessage,
        src.AccountId,
        src.Account != null ? src.Account.Name : string.Empty
    );
}
