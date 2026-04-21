using AutoMapper;
using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Import;

public class ImportMappingProfile : Profile
{
    public ImportMappingProfile()
    {
        CreateMap<ImportHistory, ImportHistoryDto>()
            .ConstructUsing((src, ctx) => new ImportHistoryDto(
                src.Id,
                src.FileName,
                src.ImportedAt,
                src.TotalRows,
                src.ImportedRows,
                src.SkippedRows,
                src.Month,
                src.Status,
                src.ErrorMessage,
                src.AccountId,
                src.Account != null ? src.Account.Name : string.Empty
            ));
    }
}
