using System.Collections.Generic;
using System.Threading.Tasks;
using Telluria.Utils.Crud.CommandResults;
using Telluria.Utils.Crud.Commands.BaseCommands;
using Telluria.Utils.Crud.Repositories;
using Telluria.Utils.Crud.Entities;

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
            var result = await _repository.ListAsync(command.Page, command.PerPage, false, command.Where, command.Includes);
            return new ListCommandResult<TEntity>(true, "List command executed with success", result, null);
        }

        public virtual async Task<IListCommandResult<TEntity>> HandleAsync(BaseListAllCommand<TEntity> command)
        {
            var result = await _repository.ListAllAsync(command.Page, command.PerPage, false, command.Where, command.Includes);
            return new ListCommandResult<TEntity>(true, "ListAll command executed with success", result, null);
        }

        public virtual async Task<ICommandResult<TEntity>> HandleAsync(BaseGetCommand<TEntity> command)
        {
            var result = await _repository.GetAsync(command.Id, false, command.Includes);
            return new CommandResult<TEntity>(true, "Get command executed with success", result, null);
        }

        public virtual async Task<ICommandResult<TEntity>> HandleAsync(BaseCreateCommand<TEntity> command)
        {
            await _repository.AddAsync(command.Data);
            await _repository.Commit();
            return new CommandResult<TEntity>(true, "Post command executed with success", command.Data, null);
        }

        public virtual async Task<ICommandResult<IEnumerable<TEntity>>> HandleAsync(BaseCreateManyCommand<TEntity> command)
        {
            await _repository.AddAsync(command.Data);
            await _repository.Commit();
            return new CommandResult<IEnumerable<TEntity>>(true, "PostMany command executed with success", command.Data, null);
        }

        public virtual async Task<ICommandResult> HandleAsync(BaseUpdateCommand<TEntity> command)
        {
            await _repository.UpdateAsync(command.Data);
            await _repository.Commit();
            return new CommandResult(true, "Patch command executed with success", null);
        }

        public virtual async Task<ICommandResult> HandleAsync(BaseUpdateManyCommand<TEntity> command)
        {
            await _repository.UpdateAsync(command.Data);
            await _repository.Commit();
            return new CommandResult(true, "PatchMany command executed with success", null);
        }

        public virtual async Task<ICommandResult> HandleAsync(BaseSoftDeleteCommand<TEntity> command)
        {
            var entity = await _repository.GetAsync(command.Id);
            await _repository.SoftDeleteAsync(entity);
            await _repository.Commit();
            return new CommandResult(true, "SoftDelete command executed with success", null);
        }

        public virtual async Task<ICommandResult> HandleAsync(BaseRemoveCommand<TEntity> command)
        {
            var entity = await _repository.GetAsync(command.Id);
            await _repository.RemoveAsync(entity);
            await _repository.Commit();
            return new CommandResult(true, "Remove command executed with success", null);
        }

        public virtual async Task<ICommandResult<TEntity>> HandleAsync(BaseFindCommand<TEntity> command)
        {
            var result = await _repository.FindAsync(false, command.Where, command.Includes);
            return new CommandResult<TEntity>(true, "Find command executed with success", result, null);
        }
    }
}