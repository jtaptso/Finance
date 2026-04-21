using FinanceTracker.Application.Dashboard.DTOs;

namespace FinanceTracker.Application.Dashboard.Queries.GetCashFlowTrend;

public record GetCashFlowTrendQuery(int Months = 6);

