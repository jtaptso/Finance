using FinanceTracker.Api.Endpoints;
using FinanceTracker.Application;
using FinanceTracker.Infrastructure;
using FinanceTracker.Infrastructure.Persistence.SeedData;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

// Seed dev data
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<FinanceTracker.Infrastructure.Persistence.ApplicationDbContext>();
    DefaultCategoriesSeeder.Seed(db);
}

app.MapOpenApi();
app.MapScalarApiReference();

app.MapTransactionEndpoints();
app.MapCategoryEndpoints();
app.MapAccountEndpoints();
app.MapBudgetEndpoints();
app.MapImportEndpoints();
app.MapDashboardEndpoints();

app.Run();
