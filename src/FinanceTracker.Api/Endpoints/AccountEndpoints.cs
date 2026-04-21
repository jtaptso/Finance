using FinanceTracker.Application.Accounts.Commands.CreateAccount;
using FinanceTracker.Application.Accounts.Commands.DeleteAccount;
using FinanceTracker.Application.Accounts.Commands.UpdateAccount;
using FinanceTracker.Application.Accounts.DTOs;
using FinanceTracker.Application.Accounts.Queries.GetAccountById;
using FinanceTracker.Application.Accounts.Queries.GetAllAccounts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Endpoints;

public static class AccountEndpoints
{
    public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/accounts").WithTags("Accounts");

        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetAllAccountsQuery(), ct);
            return Results.Ok(result);
        })
        .WithName("GetAllAccounts")
        .Produces<IReadOnlyList<AccountDto>>();

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetAccountByIdQuery(id), ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetAccountById")
        .Produces<AccountDto>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateAccountCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var id = await mediator.Send(command, ct);
            return Results.CreatedAtRoute("GetAccountById", new { id }, new { id });
        })
        .WithName("CreateAccount")
        .Produces(StatusCodes.Status201Created);

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateAccountRequest request, IMediator mediator, CancellationToken ct) =>
        {
            var command = new UpdateAccountCommand(id, request.Name, request.BankName, request.AccountNumber, request.Currency, request.InitialBalance, request.IsActive);
            var result = await mediator.Send(command, ct);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);
            return Results.NoContent();
        })
        .WithName("UpdateAccount")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteAccountCommand(id), ct);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);
            return Results.NoContent();
        })
        .WithName("DeleteAccount")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}

public record UpdateAccountRequest(
    string Name,
    string? BankName,
    string? AccountNumber,
    string Currency,
    decimal InitialBalance,
    bool IsActive
);
