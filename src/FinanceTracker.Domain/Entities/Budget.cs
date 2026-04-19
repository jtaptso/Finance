using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Entities;

public class Budget
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public BudgetPeriod Period { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
