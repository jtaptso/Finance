using FinanceTracker.Application.Budgets.Commands.CreateBudget;
using FinanceTracker.Application.Budgets.Commands.UpdateBudget;
using FinanceTracker.Application.Budgets.DTOs;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Endpoints;

public static class BudgetEndpoints
{
    public static IEndpointRouteBuilder MapBudgetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/budgets").WithTags("Budgets");

        group.MapGet("/", async (IBudgetService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetAllAsync(ct)))
        .WithName("GetAllBudgets")
        .Produces<IReadOnlyList<BudgetDto>>();

        group.MapGet("/{id:guid}", async (Guid id, IBudgetService svc, CancellationToken ct) =>
        {
            var result = await svc.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetBudgetById")
        .Produces<BudgetDto>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/vs-actual/{year:int}/{month:int}", async (int year, int month, IBudgetService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetVsActualAsync(year, month, ct)))
        .WithName("GetBudgetVsActual")
        .Produces<IReadOnlyList<BudgetVsActualDto>>();

        group.MapPost("/", async (CreateBudgetCommand command, IBudgetService svc, CancellationToken ct) =>
        {
            var id = await svc.CreateAsync(command, ct);
            return Results.CreatedAtRoute("GetBudgetById", new { id }, new { id });
        })
        .WithName("CreateBudget")
        .Produces(StatusCodes.Status201Created);

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateBudgetRequest request, IBudgetService svc, CancellationToken ct) =>
        {
            var command = new UpdateBudgetCommand(id, request.Amount, request.Period, request.StartDate, request.EndDate);
            var result = await svc.UpdateAsync(command, ct);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);
            return Results.NoContent();
        })
        .WithName("UpdateBudget")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", async (Guid id, IBudgetService svc, CancellationToken ct) =>
        {
            var result = await svc.DeleteAsync(id, ct);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);
            return Results.NoContent();
        })
        .WithName("DeleteBudget")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}

public record UpdateBudgetRequest(
    decimal Amount,
    BudgetPeriod Period,
    DateOnly StartDate,
    DateOnly? EndDate
);
