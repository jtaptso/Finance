using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Infrastructure.Persistence.SeedData;

public static class DefaultCategoriesSeeder
{
    public static readonly Guid DefaultAccountId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

    public static void Seed(ApplicationDbContext context)
    {
        if (context.Categories.Any()) return;

        var categories = CreateDefaultCategories();
        context.Categories.AddRange(categories);

        if (!context.Accounts.Any())
        {
            context.Accounts.Add(CreateDefaultAccount());
        }

        context.SaveChanges();
    }

    private static Account CreateDefaultAccount()
    {
        return new Account
        {
            Id = DefaultAccountId,
            Name = "Main Account",
            BankName = "Deutsche Bank",
            Currency = "EUR",
            InitialBalance = 0m,
            IsActive = true
        };
    }

    private static List<Category> CreateDefaultCategories()
    {
        return
        [
            CreateCategory("Salary", CategoryType.Income, "#4CAF50",
                ["Gehalt", "PSG Germany"]),

            CreateCategory("Savings & Investments", CategoryType.Income, "#2196F3",
                ["BSPK BADENIA", "Trade Republic"]),

            CreateCategory("Loans & Mortgages", CategoryType.Expense, "#F44336",
                ["Santander", "Darlehensrate", "Baufinanzierung", "BHW Bausparkasse",
                 "Hanseatic Bank", "Kreditrate", "Finanzierungsnr"]),

            CreateCategory("Insurance", CategoryType.Expense, "#E91E63",
                ["Generali", "Zurich", "Versicherung", "Haftpflicht", "Lebensversicherung"]),

            CreateCategory("Transport", CategoryType.Expense, "#9C27B0",
                ["Ruhrbahn"]),

            CreateCategory("Car & Fuel", CategoryType.Expense, "#673AB7",
                ["OIL", "Mr. Wash", "Autoservice"]),

            CreateCategory("Groceries", CategoryType.Expense, "#FF9800",
                ["LIDL", "Trinkgut", "Kiosk"]),

            CreateCategory("Utilities", CategoryType.Expense, "#795548",
                ["Stadtwerke", "eon Energie"]),

            CreateCategory("Subscriptions", CategoryType.Expense, "#607D8B",
                ["Amazon", "AMZNPrime", "Kindle", "Audible"]),

            CreateCategory("Donations", CategoryType.Expense, "#009688",
                ["Spende", "EFG Essen"]),

            CreateCategory("Family Transfers", CategoryType.Expense, "#FF5722",
                ["FOTSO", "Taptso", "Pettang", "Hüttmannschule", "Essensgeld"]),

            CreateCategory("Tax", CategoryType.Expense, "#B71C1C",
                ["Finanzamt"]),

            CreateCategory("Credit Card Payments", CategoryType.Expense, "#37474F",
                ["Kreditkartenabrechnung", "Klarna"]),

            CreateCategory("Other", CategoryType.Expense, "#9E9E9E",
                ["PayPal", "DKB"]),
        ];
    }

    private static Category CreateCategory(string name, CategoryType type, string color, List<string> keywords)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type,
            Color = color,
            IsDefault = true,
            Keywords = keywords.Select(k => new CategoryKeyword
            {
                Id = Guid.NewGuid(),
                Keyword = k
            }).ToList()
        };

        return category;
    }
}
