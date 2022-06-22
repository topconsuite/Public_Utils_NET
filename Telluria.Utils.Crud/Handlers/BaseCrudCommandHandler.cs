using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Commands.BaseCommands;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Errors;
using Telluria.Utils.Crud.Repositories;
using Telluria.Utils.Crud.Validation;

namespace Telluria.Utils.Crud.Handlers
{
  public abstract class BaseCrudCommandHandler<TEntity, TValidator, TRepository> : IBaseCrudCommandHandler<TEntity, TValidator, TRepository>
    where TEntity : BaseEntity
    where TValidator : BaseEntityValidator<TEntity>, new()
    where TRepository : IBaseCrudRepository<TEntity>
  {
    protected enum EBaseCrudCommands
    {
      FIND,
      LIST,
      LIST_ALL,
      GET,
      GET_ALL,
      CREATE,
      CREATE_MANY,
      UPDATE,
      SOFT_DELETE,
      REMOVE,
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "<Pendente>")]
    protected readonly TRepository _repository;

    protected BaseCrudCommandHandler(TRepository repository)
    {
      _repository = repository;
    }

    protected ICommandResult GetDefaultError(Exception exception)
    {
      var result = new CommandResult(
        ECommandResultStatus.ERROR,
        EServerErrorExtensions.GetErrorMessage(EServerError.INTERNAL_SERVER_ERROR) + " Exception: " + exception.Message,
        EServerError.INTERNAL_SERVER_ERROR.ToString(),
        null
      );

      return result;
    }

    protected string GetDefaultSuccessMessage(EBaseCrudCommands command)
    {
      var commandName = command switch
      {
        EBaseCrudCommands.FIND => nameof(EBaseCrudCommands.FIND),
        EBaseCrudCommands.LIST => nameof(EBaseCrudCommands.LIST),
        EBaseCrudCommands.LIST_ALL => nameof(EBaseCrudCommands.LIST_ALL),
        EBaseCrudCommands.GET => nameof(EBaseCrudCommands.GET),
        EBaseCrudCommands.GET_ALL => nameof(EBaseCrudCommands.GET_ALL),
        EBaseCrudCommands.CREATE => nameof(EBaseCrudCommands.CREATE),
        EBaseCrudCommands.CREATE_MANY => nameof(EBaseCrudCommands.CREATE_MANY),
        EBaseCrudCommands.UPDATE => nameof(EBaseCrudCommands.UPDATE),
        EBaseCrudCommands.SOFT_DELETE => nameof(EBaseCrudCommands.SOFT_DELETE),
        EBaseCrudCommands.REMOVE => nameof(EBaseCrudCommands.REMOVE),
        _ => ""
      };

      return $"{commandName} command executed with success";
    }

    protected abstract ICommandResult HandleErrors(Exception exception);

    protected abstract string GetSuccessMessage(EBaseCrudCommands command);

    public virtual async Task<IListCommandResult<TEntity>> HandleAsync(BaseListCommand<TEntity> command)
    {
      try
      {
        var result = await _repository.ListAsync(command.Page, command.PerPage, false, command.Where, command.Includes, command.CancellationToken);

        return new ListCommandResult<TEntity>(ECommandResultStatus.SUCCESS, GetSuccessMessage(EBaseCrudCommands.LIST), result);
      }
      catch (Exception e)
      {
        return HandleErrors(e).ToListCommandResult<TEntity>();
      }
    }

    public virtual async Task<IListCommandResult<TEntity>> HandleAsync(BaseListAllCommand<TEntity> command)
    {
      try
      {
        var result = await _repository.ListAllAsync(command.Page, command.PerPage, false, command.Where, command.Includes, command.CancellationToken);

        return new ListCommandResult<TEntity>(ECommandResultStatus.SUCCESS, GetSuccessMessage(EBaseCrudCommands.LIST_ALL), result);
      }
      catch (Exception e)
      {
        return HandleErrors(e).ToListCommandResult<TEntity>();
      }
    }

    public virtual async Task<ICommandResult<TEntity>> HandleAsync(BaseGetCommand command)
    {
      try
      {
        var result = await _repository.GetAsync(command.Id, false, command.Includes, command.CancellationToken);
        var message = result != null ? GetSuccessMessage(EBaseCrudCommands.GET) : $"Id '{command.Id}' not found";

        return new CommandResult<TEntity>(ECommandResultStatus.SUCCESS, message, result);
      }
      catch (Exception e)
      {
        return HandleErrors(e).ToCommandResult<TEntity>();
      }
    }

    public virtual async Task<ICommandResult<TEntity>> HandleAsync(BaseCreateCommand<TEntity> command)
    {
      try
      {
        var validationResult = Validator.Validate<TValidator, TEntity>(command.Data, BaseEntityValidations.CREATE);

        if (!validationResult.IsValid)
        {
          return new CommandResult<TEntity>(
            ECommandResultStatus.ALERT,
            EClientErrorExtensions.GetErrorMessage(EClientError.BAD_REQUEST),
            null!,
            EClientError.BAD_REQUEST.ToString(),
            validationResult.Errors
          );
        }

        await _repository.AddAsync(command.Data, command.CancellationToken);
        await _repository.Commit(command.CancellationToken);

        var result = await _repository.GetAsync(command.Data.Id, false, command.Includes, command.CancellationToken);

        return new CommandResult<TEntity>(ECommandResultStatus.SUCCESS, GetSuccessMessage(EBaseCrudCommands.CREATE), result);
      }
      catch (Exception e)
      {
        return HandleErrors(e).ToCommandResult<TEntity>();
      }
    }

    public virtual async Task<ICommandResult<IEnumerable<TEntity>>> HandleAsync(BaseCreateManyCommand<TEntity> command)
    {
      try
      {
        var validationResults = command.Data.Select(entity => Validator.Validate<TValidator, TEntity>(entity, BaseEntityValidations.CREATE));

        if (validationResults.Any(t => !t.IsValid))
        {
          return new CommandResult<IEnumerable<TEntity>>(
            ECommandResultStatus.ALERT,
            EClientErrorExtensions.GetErrorMessage(EClientError.BAD_REQUEST),
            null!,
            EClientError.BAD_REQUEST.ToString(),
            validationResults.FirstOrDefault(t => !t.IsValid).Errors
          );
        }

        await _repository.AddAsync(command.Data, command.CancellationToken);
        await _repository.Commit(command.CancellationToken);

        var createdIds = command.Data.Select(t => t.Id);
        var result = await _repository.ListAsync(false, t => createdIds.Contains(t.Id), command.Includes, command.CancellationToken);

        return new CommandResult<IEnumerable<TEntity>>(ECommandResultStatus.SUCCESS, GetSuccessMessage(EBaseCrudCommands.CREATE_MANY), result);
      }
      catch (Exception e)
      {
        return HandleErrors(e).ToEnumerableCommandResult<TEntity>();
      }
    }

    public virtual async Task<ICommandResult<TEntity>> HandleAsync(BaseUpdateCommand<TEntity> command)
    {
      try
      {
        var validationResult = Validator.Validate<TValidator, TEntity>(command.Data, BaseEntityValidations.UPDATE);

        if (!validationResult.IsValid)
        {
          return new CommandResult<TEntity>(
            ECommandResultStatus.ALERT,
            EClientErrorExtensions.GetErrorMessage(EClientError.BAD_REQUEST),
            null!,
            EClientError.BAD_REQUEST.ToString(),
            validationResult.Errors
          );
        }

        await _repository.UpdateAsync(command.Data, command.CancellationToken);
        await _repository.Commit(command.CancellationToken);

        var result = await _repository.GetAsync(command.Data.Id, false, command.Includes, command.CancellationToken);

        return new CommandResult<TEntity>(ECommandResultStatus.SUCCESS, GetSuccessMessage(EBaseCrudCommands.UPDATE), result);
      }
      catch (Exception e)
      {
        return HandleErrors(e).ToCommandResult<TEntity>();
      }
    }

    public virtual async Task<ICommandResult> HandleAsync(BaseSoftDeleteCommand command)
    {
      try
      {
        var entity = await _repository.GetAsync(command.Id, false, null, command.CancellationToken);

        if (entity == null)
        {
          return new CommandResult<TEntity>(
            ECommandResultStatus.ALERT,
            EClientErrorExtensions.GetErrorMessage(EClientError.NOT_FOUND),
            null!,
            EClientError.NOT_FOUND.ToString(),
            null
          );
        }

        await _repository.SoftDeleteAsync(entity, command.CancellationToken);
        await _repository.Commit(command.CancellationToken);

        return new CommandResult(ECommandResultStatus.SUCCESS, GetSuccessMessage(EBaseCrudCommands.SOFT_DELETE));
      }
      catch (Exception e)
      {
        return HandleErrors(e);
      }
    }

    public virtual async Task<ICommandResult> HandleAsync(BaseRemoveCommand command)
    {
      try
      {
        var entity = await _repository.GetAsync(command.Id, false, null, command.CancellationToken);

        if (entity == null)
        {
          return new CommandResult<TEntity>(
            ECommandResultStatus.ALERT,
            EClientErrorExtensions.GetErrorMessage(EClientError.NOT_FOUND),
            null!,
            EClientError.NOT_FOUND.ToString(),
            null
          );
        }

        await _repository.RemoveAsync(entity, command.CancellationToken);
        await _repository.Commit(command.CancellationToken);

        return new CommandResult(ECommandResultStatus.SUCCESS, GetSuccessMessage(EBaseCrudCommands.REMOVE));
      }
      catch (Exception e)
      {
        return HandleErrors(e);
      }
    }

    public virtual async Task<ICommandResult<TEntity>> HandleAsync(BaseFindCommand<TEntity> command)
    {
      try
      {
        var result = await _repository.FindAsync(false, command.Where, command.Includes, command.CancellationToken);
        var message = result != null ? GetSuccessMessage(EBaseCrudCommands.FIND) : "Not found";

        return new CommandResult<TEntity>(ECommandResultStatus.SUCCESS, message, result);
      }
      catch (Exception e)
      {
        return HandleErrors(e).ToCommandResult<TEntity>();
      }
    }
  }
}
