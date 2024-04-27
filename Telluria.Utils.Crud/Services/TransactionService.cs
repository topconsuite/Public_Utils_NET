using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Telluria.Utils.Crud.Services;

public class TransactionService : ITransactionService
{
  // Definição de um delegate para o evento
  public delegate Task TransactionEventHandlerAsync(object sender, EventArgs e);

  // Evento que outros objetos podem ouvir
  public event TransactionEventHandlerAsync TransactionCompletedAsync;

  // Método que dispara o evento
  protected virtual async Task OnTransactionCompleted()
  {
    if (TransactionCompletedAsync != null)
    {
      // Disparar o evento para todos os inscritos
      foreach (var handler in TransactionCompletedAsync.GetInvocationList().OfType<TransactionEventHandlerAsync>())
      {
        await handler(this, EventArgs.Empty);
      }
    }
  }

  public bool HasTransaction { get; set; }

  public async Task ExecuteTransactionAsync(Func<Task> transactionalOperation, TransactionScopeAsyncFlowOption option = TransactionScopeAsyncFlowOption.Enabled)
  {
    if (HasTransaction)
      throw new Exception("Não é permitido o encadeamento de transações!");

    HasTransaction = true;

    var transactionOptions = new TransactionOptions
    {
      Timeout = TimeSpan.FromHours(1)
    };

    try
    {
      using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions, option))
      {
        // Execute the transactional operations
        await transactionalOperation();

        // If all operations were successful, complete the transaction
        scope.Complete();
      }
    }
    finally
    {
      HasTransaction = false;

      // Prepara mensagens
      await OnTransactionCompleted();
    }
  }

  public async Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> transactionalOperation, TransactionScopeAsyncFlowOption option = TransactionScopeAsyncFlowOption.Enabled)
  {
    if (HasTransaction)
      throw new Exception("Não é permitido o encadeamento de transações!");

    HasTransaction = true;

    T result;

    try
    {
      using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, option))
      {
        // Execute the transactional operations
        result = await transactionalOperation();

        // If all operations were successful, complete the transaction
        scope.Complete();
      }
    }
    finally
    {
      HasTransaction = false;

      // Prepara mensagens
      await OnTransactionCompleted();
    }

    return result;
  }
}
