using FinanceTracker.Application.Dashboard.DTOs;
using FinanceTracker.Application.Dashboard.Queries.GetCashFlowTrend;
using FinanceTracker.Application.Dashboard.Queries.GetCategoryBreakdown;
using FinanceTracker.Application.Dashboard.Queries.GetMonthlyOverview;
using MediatR;

namespace FinanceTracker.Api.Endpoints;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboard").WithTags("Dashboard");

        group.MapGet("/overview/{year:int}/{month:int}", async (int year, int month, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetMonthlyOverviewQuery(year, month), ct);
            return Results.Ok(result);
        })
        .WithName("GetMonthlyOverview")
        .Produces<MonthlyOverviewDto>();

        group.MapGet("/categories/{year:int}/{month:int}", async (int year, int month, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetCategoryBreakdownQuery(year, month), ct);
            return Results.Ok(result);
        })
        .WithName("GetCategoryBreakdown")
        .Produces<IReadOnlyList<CategoryBreakdownItemDto>>();

        group.MapGet("/cash-flow", async (IMediator mediator, CancellationToken ct, int months = 6) =>
        {
            var result = await mediator.Send(new GetCashFlowTrendQuery(months), ct);
            return Results.Ok(result);
        })
        .WithName("GetCashFlowTrend")
        .Produces<CashFlowTrendDto>();

        return app;
    }
}
