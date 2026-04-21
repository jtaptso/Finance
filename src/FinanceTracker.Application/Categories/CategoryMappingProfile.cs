using AutoMapper;
using FinanceTracker.Application.Categories.DTOs;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Categories;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ConstructUsing((src, ctx) => new CategoryDto(
                src.Id,
                src.Name,
                src.Type,
                src.Icon,
                src.Color,
                src.IsDefault,
                src.ParentCategoryId,
                src.ParentCategory != null ? src.ParentCategory.Name : null,
                src.Keywords.Select(k => k.Keyword).ToList()
            ));
    }
}
