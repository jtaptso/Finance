using FinanceTracker.Application.Common.Models;

namespace FinanceTracker.Application.Transactions.Commands.DeleteTransaction;

public record DeleteTransactionCommand(Guid Id);

