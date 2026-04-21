namespace FinanceTracker.Application.Accounts.DTOs;

public record AccountDto(
    Guid Id,
    string Name,
    string? BankName,
    string? AccountNumber,
    string Currency,
    decimal InitialBalance,
    decimal CurrentBalance,
    bool IsActive
);
