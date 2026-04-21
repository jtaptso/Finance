using AutoMapper;
using FinanceTracker.Application.Budgets.DTOs;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Budgets;

public class BudgetMappingProfile : Profile
{
    public BudgetMappingProfile()
    {
        CreateMap<Budget, BudgetDto>()
            .ConstructUsing((src, ctx) => new BudgetDto(
                src.Id,
                src.CategoryId,
                src.Category != null ? src.Category.Name : string.Empty,
                src.Category?.Color,
                src.Amount,
                src.Period,
                src.StartDate,
                src.EndDate
            ));
    }
}
