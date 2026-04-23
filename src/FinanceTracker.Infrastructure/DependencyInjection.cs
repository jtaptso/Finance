using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Infrastructure.Persistence;
using FinanceTracker.Infrastructure.Persistence.Repositories;
using FinanceTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("FinanceTrackerDev"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IBudgetRepository, BudgetRepository>();
        services.AddScoped<IImportHistoryRepository, ImportHistoryRepository>();
        services.AddScoped<ICategorizationService, CategorizationService>();
        services.AddScoped<IExcelImportService, ExcelParsingService>();

        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<IImportService, ImportService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }

    /// <summary>
    /// Applies pending EF migrations for relational databases, or ensures the schema is created for InMemory.
    /// </summary>
    public static void MigrateDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        var providerName = context.Database.ProviderName ?? string.Empty;
        var isInMemory = providerName.Contains("InMemory", StringComparison.OrdinalIgnoreCase);
        var dbName = isInMemory ? "(in-memory)" : context.Database.GetDbConnection().Database;
        logger.LogWarning("=== DATABASE PROVIDER: {Provider} | DATABASE: {Database} ===", providerName, dbName);
        if (isInMemory)
            context.Database.EnsureCreated();
        else
            context.Database.Migrate();
    }
}
