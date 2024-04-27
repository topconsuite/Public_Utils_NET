using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Telluria.Utils.Crud.Services;

public interface ITransactionService
{
  bool HasTransaction { get; set; }
  event TransactionService.TransactionEventHandlerAsync TransactionCompletedAsync;
  Task ExecuteTransactionAsync(Func<Task> transactionalOperation, TransactionScopeAsyncFlowOption option = TransactionScopeAsyncFlowOption.Enabled);
  Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> transactionalOperation, TransactionScopeAsyncFlowOption option = TransactionScopeAsyncFlowOption.Enabled);
}
