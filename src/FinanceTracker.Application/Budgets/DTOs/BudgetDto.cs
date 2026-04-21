using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Budgets.DTOs;

public record BudgetDto(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string? CategoryColor,
    decimal Amount,
    BudgetPeriod Period,
    DateOnly StartDate,
    DateOnly? EndDate
);

public record BudgetVsActualDto(
    Guid BudgetId,
    Guid CategoryId,
    string CategoryName,
    string? CategoryColor,
    decimal BudgetedAmount,
    decimal SpentAmount,
    decimal RemainingAmount,
    double PercentageUsed,
    BudgetPeriod Period
);
