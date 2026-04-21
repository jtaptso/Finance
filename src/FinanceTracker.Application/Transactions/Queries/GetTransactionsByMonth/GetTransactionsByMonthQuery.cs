using FinanceTracker.Application.Transactions.DTOs;

namespace FinanceTracker.Application.Transactions.Queries.GetTransactionsByMonth;

public record GetTransactionsByMonthQuery(int Year, int Month);

