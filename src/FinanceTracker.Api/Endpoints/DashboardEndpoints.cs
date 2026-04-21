using FinanceTracker.Application.Dashboard.DTOs;
using FinanceTracker.Application.Services;

namespace FinanceTracker.Api.Endpoints;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboard").WithTags("Dashboard");

        group.MapGet("/overview/{year:int}/{month:int}", async (int year, int month, IDashboardService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetMonthlyOverviewAsync(year, month, ct)))
        .WithName("GetMonthlyOverview")
        .Produces<MonthlyOverviewDto>();

        group.MapGet("/categories/{year:int}/{month:int}", async (int year, int month, IDashboardService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetCategoryBreakdownAsync(year, month, ct)))
        .WithName("GetCategoryBreakdown")
        .Produces<IReadOnlyList<CategoryBreakdownItemDto>>();

        group.MapGet("/cash-flow", async (IDashboardService svc, CancellationToken ct, int months = 6) =>
            Results.Ok(await svc.GetCashFlowTrendAsync(months, ct)))
        .WithName("GetCashFlowTrend")
        .Produces<CashFlowTrendDto>();

        return app;
    }
}
