using FinanceTracker.Application.Categories.Commands.CreateCategory;
using FinanceTracker.Application.Categories.Commands.UpdateCategory;
using FinanceTracker.Application.Categories.DTOs;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", async (ICategoryService svc, CancellationToken ct) =>
            Results.Ok(await svc.GetAllAsync(ct)))
        .WithName("GetAllCategories")
        .Produces<IReadOnlyList<CategoryDto>>();

        group.MapGet("/{id:guid}", async (Guid id, ICategoryService svc, CancellationToken ct) =>
        {
            var result = await svc.GetByIdAsync(id, ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetCategoryById")
        .Produces<CategoryDto>()
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (CreateCategoryCommand command, ICategoryService svc, CancellationToken ct) =>
        {
            var id = await svc.CreateAsync(command, ct);
            return Results.CreatedAtRoute("GetCategoryById", new { id }, new { id });
        })
        .WithName("CreateCategory")
        .Produces(StatusCodes.Status201Created);

        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateCategoryRequest request, ICategoryService svc, CancellationToken ct) =>
        {
            var command = new UpdateCategoryCommand(id, request.Name, request.Type, request.Icon, request.Color, request.ParentCategoryId, request.Keywords);
            var result = await svc.UpdateAsync(command, ct);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);
            return Results.NoContent();
        })
        .WithName("UpdateCategory")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", async (Guid id, ICategoryService svc, CancellationToken ct) =>
        {
            var result = await svc.DeleteAsync(id, ct);
            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);
            return Results.NoContent();
        })
        .WithName("DeleteCategory")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}

public record UpdateCategoryRequest(
    string Name,
    CategoryType Type,
    string? Icon,
    string? Color,
    Guid? ParentCategoryId,
    IReadOnlyList<string>? Keywords
);
