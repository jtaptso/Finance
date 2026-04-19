namespace FinanceTracker.Domain.Entities;

public class CategoryKeyword
{
    public Guid Id { get; set; }
    public string Keyword { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
