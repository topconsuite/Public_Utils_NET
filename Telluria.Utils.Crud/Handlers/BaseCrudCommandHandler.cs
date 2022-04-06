using System.Collections.Generic;
using System.Threading.Tasks;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Commands.BaseCommands;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Repositories;

namespace Telluria.Utils.Crud.Handlers
{
  public class BaseCrudCommandHandler<TEntity> : IBaseCrudCommandHandler<TEntity> where TEntity : BaseEntity
  {
    protected IBaseCrudRepository<TEntity> _repository { get; set; }

    public BaseCrudCommandHandler(IBaseCrudRepository<TEntity> repository)
    {
      _repository = repository;
    }

    public virtual async Task<IListCommandResult<TEntity>> HandleAsync(BaseListCommand<TEntity> command)
    {
      try
      {
        var result = await _repository.ListAsync(command.Page, command.PerPage, false, command.Where, command.Includes);
        return new ListCommandResult<TEntity>(CommandResultStatus.SUCCESS, "List command executed with success", result);
      }
      catch (System.Exception e)
      {
        return new ListCommandResult<TEntity>(CommandResultStatus.ERROR, e.Message, null, null, null, e);
      }
    }

    public virtual async Task<IListCommandResult<TEntity>> HandleAsync(BaseListAllCommand<TEntity> command)
    {
      try
      {
        var result = await _repository.ListAllAsync(command.Page, command.PerPage, false, command.Where, command.Includes);
        return new ListCommandResult<TEntity>(CommandResultStatus.SUCCESS, "ListAll command executed with success", result);
      }
      catch (System.Exception e)
      {
        return new ListCommandResult<TEntity>(CommandResultStatus.ERROR, e.Message, null, null, null, e);
      }
    }

    public virtual async Task<ICommandResult<TEntity>> HandleAsync(BaseGetCommand<TEntity> command)
    {
      try
      {
        var result = await _repository.GetAsync(command.Id, false, command.Includes);
        var message = result != null ? "Get command executed with success" : $"Id '{command.Id}' not found";
        return new CommandResult<TEntity>(CommandResultStatus.SUCCESS, message, result);
      }
      catch (System.Exception e)
      {
        return new CommandResult<TEntity>(CommandResultStatus.ERROR, e.Message, null, null, null, e);
      }
    }

    public virtual async Task<ICommandResult<TEntity>> HandleAsync(BaseCreateCommand<TEntity> command)
    {
      try
      {
        await _repository.AddAsync(command.Data);
        await _repository.Commit();

        var result = await _repository.GetAsync(command.Data.Id, false, command.Includes);

        return new CommandResult<TEntity>(CommandResultStatus.SUCCESS, "Post command executed with success", result);
      }
      catch (System.Exception e)
      {
        return new CommandResult<TEntity>(CommandResultStatus.ERROR, e.Message, null, null, null, e);
      }
    }

    public virtual async Task<ICommandResult<IEnumerable<TEntity>>> HandleAsync(BaseCreateManyCommand<TEntity> command)
    {
      try
      {
        await _repository.AddAsync(command.Data);
        await _repository.Commit();
        return new CommandResult<IEnumerable<TEntity>>(CommandResultStatus.SUCCESS, "PostMany command executed with success", command.Data);
      }
      catch (System.Exception e)
      {
        return new CommandResult<IEnumerable<TEntity>>(CommandResultStatus.ERROR, e.Message, null, null, null, e);
      }
    }

    public virtual async Task<ICommandResult<TEntity>> HandleAsync(BaseUpdateCommand<TEntity> command)
    {
      try
      {
        await _repository.UpdateAsync(command.Data);
        await _repository.Commit();

        var result = await _repository.GetAsync(command.Data.Id, false, command.Includes);

        return new CommandResult<TEntity>(CommandResultStatus.SUCCESS, "Patch command executed with success", result);
      }
      catch (System.Exception e)
      {
        return new CommandResult<TEntity>(CommandResultStatus.ERROR, e.Message, null, null, null, e);
      }
    }

    public virtual async Task<ICommandResult> HandleAsync(BaseSoftDeleteCommand<TEntity> command)
    {
      try
      {
        var entity = await _repository.GetAsync(command.Id);
        await _repository.SoftDeleteAsync(entity);
        await _repository.Commit();
        return new CommandResult(CommandResultStatus.SUCCESS, "SoftDelete command executed with success");
      }
      catch (System.Exception e)
      {
        return new CommandResult(CommandResultStatus.ERROR, e.Message, null, null, e);
      }
    }

    public virtual async Task<ICommandResult> HandleAsync(BaseRemoveCommand<TEntity> command)
    {
      try
      {
        var entity = await _repository.GetAsync(command.Id);
        await _repository.RemoveAsync(entity);
        await _repository.Commit();
        return new CommandResult(CommandResultStatus.SUCCESS, "Remove command executed with success");
      }
      catch (System.Exception e)
      {
        return new CommandResult(CommandResultStatus.ERROR, e.Message, null, null, e);
      }
    }

    public virtual async Task<ICommandResult<TEntity>> HandleAsync(BaseFindCommand<TEntity> command)
    {
      try
      {
        var result = await _repository.FindAsync(false, command.Where, command.Includes);
        var message = result != null ? "Find command executed with success" : "Not found";
        return new CommandResult<TEntity>(CommandResultStatus.SUCCESS, message, result);
      }
      catch (System.Exception e)
      {
        return new CommandResult<TEntity>(CommandResultStatus.ERROR, e.Message, null, null, null, e);
      }
    }
  }
}
