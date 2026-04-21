using FinanceTracker.Application.Accounts.Commands.CreateAccount;
using FinanceTracker.Application.Accounts.Commands.UpdateAccount;
using FinanceTracker.Application.Accounts.DTOs;
using FinanceTracker.Application.Common.Mappings;
using FinanceTracker.Application.Common.Models;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _uow;
    public AccountService(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<AccountDto>> GetAllAsync(CancellationToken ct = default)
    {
        var accounts = await _uow.Accounts.GetAllAsync(ct);
        return accounts.Select(a => a.ToDto()).ToList();
    }

    public async Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var account = await _uow.Accounts.GetByIdAsync(id, ct);
        return account?.ToDto();
    }

    public async Task<Guid> CreateAsync(CreateAccountCommand cmd, CancellationToken ct = default)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = cmd.Name,
            BankName = cmd.BankName,
            AccountNumber = cmd.AccountNumber,
            Currency = cmd.Currency,
            InitialBalance = cmd.InitialBalance,
            IsActive = true
        };
        _uow.Accounts.Add(account);
        await _uow.SaveChangesAsync(ct);
        return account.Id;
    }

    public async Task<Result> UpdateAsync(UpdateAccountCommand cmd, CancellationToken ct = default)
    {
        var account = await _uow.Accounts.GetByIdAsync(cmd.Id, ct);
        if (account is null)
            return Result.Failure($"Account {cmd.Id} not found.");

        account.Name = cmd.Name;
        account.BankName = cmd.BankName;
        account.AccountNumber = cmd.AccountNumber;
        account.Currency = cmd.Currency;
        account.InitialBalance = cmd.InitialBalance;
        account.IsActive = cmd.IsActive;

        _uow.Accounts.Update(account);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var account = await _uow.Accounts.GetByIdAsync(id, ct);
        if (account is null)
            return Result.Failure($"Account {id} not found.");

        _uow.Accounts.Remove(account);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
