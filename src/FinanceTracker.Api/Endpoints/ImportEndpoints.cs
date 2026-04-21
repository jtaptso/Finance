using FinanceTracker.Application.Import.Commands.ConfirmImport;
using FinanceTracker.Application.Import.Commands.DeleteImport;
using FinanceTracker.Application.Import.Commands.ParseExcelFile;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Application.Import.Queries.GetImportDetail;
using FinanceTracker.Application.Import.Queries.GetImportHistory;
using MediatR;

namespace FinanceTracker.Api.Endpoints;

public static class ImportEndpoints
{
    public static IEndpointRouteBuilder MapImportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/import").WithTags("Import");

        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetImportHistoryQuery(), ct);
            return Results.Ok(result);
        })
        .WithName("GetImportHistory")
        .Produces<IReadOnlyList<ImportHistoryDto>>();

        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetImportDetailQuery(id), ct);
            return result.Import is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetImportDetail")
        .Produces(StatusCodes.Status404NotFound);

        // Step 1 — Parse Excel file, returns preview (nothing saved)
        group.MapPost("/parse", async (HttpRequest request, IMediator mediator, CancellationToken ct) =>
        {
            if (!request.HasFormContentType)
                return Results.BadRequest("Multipart form required.");

            var form = await request.ReadFormAsync(ct);
            var file = form.Files.GetFile("file");
            if (file is null)
                return Results.BadRequest("No file uploaded.");

            if (!Guid.TryParse(form["accountId"], out var accountId))
                return Results.BadRequest("Valid accountId is required.");

            using var stream = file.OpenReadStream();
            var preview = await mediator.Send(new ParseExcelFileCommand(stream, file.FileName, accountId), ct);
            return Results.Ok(preview);
        })
        .WithName("ParseExcelFile")
        .Produces<ImportPreviewDto>()
        .Produces(StatusCodes.Status400BadRequest)
        .DisableAntiforgery();

        // Step 2 — Confirm selected rows, persists data
        group.MapPost("/confirm", async (ConfirmImportCommand command, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);
            return Results.Ok(new { importId = result.Value });
        })
        .WithName("ConfirmImport")
        .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new DeleteImportCommand(id), ct);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);
            return Results.NoContent();
        })
        .WithName("DeleteImport")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
