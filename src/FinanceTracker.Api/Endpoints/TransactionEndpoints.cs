using FinanceTracker.Application.Transactions.Commands.CreateTransaction;
using FinanceTracker.Application.Transactions.Commands.DeleteTransaction;
using FinanceTracker.Application.Transactions.Commands.UpdateTransaction;
using FinanceTracker.Application.Transactions.DTOs;
using FinanceTracker.Application.Transactions.Queries.GetTransactionById;
using FinanceTracker.Application.Transactions.Queries.GetTransactionsByMonth;
using FinanceTracker.Application.Transactions.Queries.GetTransactionsList;
using FinanceTracker.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Endpoints;

public static class TransactionEndpoints
{
    public static IEndpointRouteBuilder MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transactions").WithTags("Transactions");

        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetTransactionsListQuery(), ct);
            return Results.Ok(result);
        })
        .WithName("GetAllTransactions")
        .Produces<IReadOnlyList<TransactionDto>>();

        group.MapGet("/month/{year:int}/{month:int}", async (int year, int month, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetTransactionsByMonthQuery(year, month), ct);
            return Results.Ok(result);
        })
        .WithName("GetTransactionsByMonth")
        .Produces<IReadOnlyList<TransactionDto>>();

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetTransactionByIdQuery(id), ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetTransactionById")
        .Produces<TransactionDto>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateTransactionCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var id = await mediator.Send(command, ct);
            return Results.CreatedAtRoute("GetTransactionById", new { id }, new { id });
        })
        .WithName("CreateTransaction")
        .Produces(StatusCodes.Status201Created);

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateTransactionRequest request, IMediator mediator, CancellationToken ct) =>
        {
            var command = new UpdateTransactionCommand(
                id,
                request.Date,
                request.TransactionType,
                request.Amount,
                request.Description,
                request.AccountId,
                request.CategoryId,
                request.Notes,
                request.IsRecurring,
                request.Currency);

            var result = await mediator.Send(command, ct);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);

            return Results.NoContent();
        })
        .WithName("UpdateTransaction")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteTransactionCommand(id), ct);
            if (!result.Succeeded)
                return Results.NotFound(result.Errors);

            return Results.NoContent();
        })
        .WithName("DeleteTransaction")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}

public record UpdateTransactionRequest(
    DateOnly Date,
    TransactionType TransactionType,
    decimal Amount,
    string Description,
    Guid AccountId,
    Guid? CategoryId,
    string? Notes,
    bool IsRecurring,
    string Currency = "EUR"
);
