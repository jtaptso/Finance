using FinanceTracker.Application.Common.Models;
using FinanceTracker.Application.Accounts.Commands.CreateAccount;
using FinanceTracker.Application.Accounts.Commands.UpdateAccount;
using FinanceTracker.Application.Accounts.DTOs;

namespace FinanceTracker.Application.Services;

public interface IAccountService
{
    Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken ct = default);
    Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateAccountCommand command, CancellationToken ct = default);
    Task<Result> UpdateAsync(UpdateAccountCommand command, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
