using System;
using System.Threading.Tasks;
using System.Transactions;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Services;

public interface ITransactionService
{
  bool HasTransaction { get; set; }
  void AddChange(IntegrationMessage message);
  Task ExecuteTransactionAsync(Func<Task> transactionalOperation, TransactionScopeAsyncFlowOption option = TransactionScopeAsyncFlowOption.Enabled);
  Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> transactionalOperation, TransactionScopeAsyncFlowOption option = TransactionScopeAsyncFlowOption.Enabled);
}
