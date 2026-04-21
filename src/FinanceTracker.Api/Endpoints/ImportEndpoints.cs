using FinanceTracker.Application.Import.Commands.ConfirmImport;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Application.Services;

namespace FinanceTracker.Api.Endpoints;

public static class ImportEndpoints
{
    public static IEndpointRouteBuilder MapImportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/import").WithTags("Import");

        group.MapGet("/", async (IImportService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetHistoryAsync(ct)))
        .WithName("GetImportHistory")
        .Produces<IReadOnlyList<ImportHistoryDto>>();

        group.MapGet("/{id:guid}", async (Guid id, IImportService svc, CancellationToken ct) =>
        {
            var result = await svc.GetDetailAsync(id, ct);
            return result.Import is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetImportDetail")
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/parse", async (HttpRequest request, IImportService svc, CancellationToken ct) =>
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
            var preview = await svc.ParseExcelAsync(stream, file.FileName, accountId, ct);
            return Results.Ok(preview);
        })
        .WithName("ParseExcelFile")
        .Produces<ImportPreviewDto>()
        .Produces(StatusCodes.Status400BadRequest)
        .DisableAntiforgery();

        group.MapPost("/confirm", async (ConfirmImportCommand command, IImportService svc, CancellationToken ct) =>
        {
            var result = await svc.ConfirmAsync(command, ct);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);
            return Results.Ok(new { importId = result.Value });
        })
        .WithName("ConfirmImport")
        .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", async (Guid id, IImportService svc, CancellationToken ct) =>
        {
            var result = await svc.DeleteAsync(id, ct);
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
