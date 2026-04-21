using FinanceTracker.Application.Common.Models;

namespace FinanceTracker.Application.Import.Commands.DeleteImport;

/// <summary>
/// Undoes an import — deletes the ImportHistory record and all its associated transactions.
/// </summary>
public record DeleteImportCommand(Guid ImportHistoryId);

