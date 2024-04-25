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
  private List<IntegrationMessage> _messages;

  public bool HasTransaction { get; set; }

  public TransactionService(IMessageBrokerService messageService)
  {
    _messageService = messageService;
  }

  public void AddChange(IntegrationMessage message)
  {
    _messages.Add(message);
  }

  private async Task SendMessageAsync()
  {
    foreach (var message in _messages)
    {
      await _messageService.SendIntegrationMessageAsync(message);
    }
  }

  public async Task ExecuteTransactionAsync(Func<Task> transactionalOperation, TransactionScopeAsyncFlowOption option = TransactionScopeAsyncFlowOption.Enabled)
  {
    if (HasTransaction)
      throw new Exception("Não é permitido o encadeamento de transações!");

    var transactionOptions = new TransactionOptions
    {
      Timeout = TimeSpan.FromHours(1)
    };

    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions, option))
    {
      HasTransaction = true;

      _messages = new List<IntegrationMessage>();

      try
      {
        // Execute the transactional operations
        await transactionalOperation();

        // If all operations were successful, complete the transaction
        scope.Complete();
      }
      finally
      {
        HasTransaction = false;

        Task.Run(SendMessageAsync);
      }
    }
  }

  public async Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> transactionalOperation, TransactionScopeAsyncFlowOption option = TransactionScopeAsyncFlowOption.Enabled)
  {
    if (HasTransaction)
      throw new Exception("Não é permitido o encadeamento de transações!");

    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
    {
      HasTransaction = true;

      _messages = new List<IntegrationMessage>();

      T result;

      try
      {
        // Execute the transactional operations
        result = await transactionalOperation();

        // If all operations were successful, complete the transaction
        scope.Complete();
      }
      finally
      {
        HasTransaction = false;

        Task.Run(SendMessageAsync);
      }

      return result;
    }
  }
}
