using FinanceTracker.Application.Import.DTOs;
using FinanceTracker.Application.Transactions.DTOs;

namespace FinanceTracker.Application.Import.Queries.GetImportDetail;

public record GetImportDetailQuery(Guid ImportHistoryId);

