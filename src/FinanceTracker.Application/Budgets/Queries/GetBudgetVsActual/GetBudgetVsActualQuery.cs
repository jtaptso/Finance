using FinanceTracker.Application.Budgets.DTOs;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Budgets.Queries.GetBudgetVsActual;

public record GetBudgetVsActualQuery(int Year, int Month);

