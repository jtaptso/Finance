using AutoMapper;
using FinanceTracker.Application.Accounts.DTOs;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Accounts;

public class AccountMappingProfile : Profile
{
    public AccountMappingProfile()
    {
        CreateMap<Account, AccountDto>()
            .ConstructUsing((src, ctx) => new AccountDto(
                src.Id,
                src.Name,
                src.BankName,
                src.AccountNumber,
                src.Currency,
                src.InitialBalance,
                src.CurrentBalance,
                src.IsActive
            ));
    }
}
