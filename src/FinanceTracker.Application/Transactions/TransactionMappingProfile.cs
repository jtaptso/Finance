using AutoMapper;
using FinanceTracker.Application.Transactions.DTOs;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Transactions;

public class TransactionMappingProfile : Profile
{
    public TransactionMappingProfile()
    {
        CreateMap<Transaction, TransactionDto>()
            .ConstructUsing((src, ctx) => new TransactionDto(
                src.Id,
                src.Date,
                src.TransactionType,
                src.TransactionType.ToString(),
                src.Amount,
                src.Currency,
                src.Description,
                src.Notes,
                src.IsRecurring,
                src.AccountId,
                src.Account != null ? src.Account.Name : string.Empty,
                src.CategoryId,
                src.Category != null ? src.Category.Name : null,
                src.Category != null ? src.Category.Color : null,
                src.ImportHistoryId,
                src.CreatedAt,
                src.UpdatedAt
            ));
    }
}
