using System;
using System.Threading.Tasks;
using System.Transactions;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Services;

public interface ITransactionService : IDisposable
{
  bool HasTransaction { get; }
  void AddChange(IntegrationMessage message);
  TransactionScope BeginTransaction(TransactionScopeAsyncFlowOption option = TransactionScopeAsyncFlowOption.Enabled);
  Task SendMessageAsync();
  void Complete();
  void Dispose();
}
