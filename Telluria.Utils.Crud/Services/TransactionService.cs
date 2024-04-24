using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Telluria.Utils.Crud.Entities;
using Topcon.Tech.Domain.Interfaces.Services;

namespace Telluria.Utils.Crud.Services;

public class TransactionService : ITransactionService
{
  private readonly IMessageBrokerService _messageService;

  private TransactionScope _transactionScope;
  private List<IntegrationMessage> _messages;
  private bool _disposed = false;
  private bool _completed = false;

  public bool HasTransaction => _transactionScope != null;

  public TransactionService(IMessageBrokerService messageService)
  {
    _transactionScope = null;
    _messageService = messageService;
    _messages = new List<IntegrationMessage>();
  }

  public TransactionScope BeginTransaction(TransactionScopeAsyncFlowOption option = TransactionScopeAsyncFlowOption.Enabled)
  {
    if (_transactionScope != null)
      throw new Exception("Transaction already started");

    _messages = new List<IntegrationMessage>();
    return _transactionScope = new TransactionScope(option);
  }

  public void AddChange(IntegrationMessage message)
  {
    _messages.Add(message);
  }

  public void Complete()
  {
    if (!_completed)
    {
      _transactionScope.Complete();
      _completed = true;
    }
  }

  public async Task SendMessageAsync()
  {
    if (_completed)
    {
      foreach (var message in _messages)
      {
        await _messageService.SendIntegrationMessageAsync(message);
      }
    }
  }

  public void Dispose()
  {
    if (!_disposed && !_completed && _transactionScope != null)
    {
      _transactionScope.Dispose();

      _disposed = true;
    }
  }
}
