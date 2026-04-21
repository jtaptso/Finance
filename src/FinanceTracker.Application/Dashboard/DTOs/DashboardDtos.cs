namespace FinanceTracker.Application.Dashboard.DTOs;

public record MonthlyOverviewDto(
    int Year,
    int Month,
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal NetBalance,
    int TransactionCount
);

public record CategoryBreakdownItemDto(
    Guid CategoryId,
    string CategoryName,
    string? CategoryColor,
    decimal Amount,
    double Percentage
);

public record IncomeVsExpensesItemDto(
    int Year,
    int Month,
    decimal Income,
    decimal Expenses,
    decimal Net
);

public record CashFlowTrendDto(
    IReadOnlyList<IncomeVsExpensesItemDto> Months
);
