using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Transactions.DTOs;

public record TransactionDto(
    Guid Id,
    DateOnly Date,
    TransactionType TransactionType,
    string TransactionTypeName,
    decimal Amount,
    string Currency,
    string Description,
    string? Notes,
    bool IsRecurring,
    Guid AccountId,
    string AccountName,
    Guid? CategoryId,
    string? CategoryName,
    string? CategoryColor,
    Guid? ImportHistoryId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
