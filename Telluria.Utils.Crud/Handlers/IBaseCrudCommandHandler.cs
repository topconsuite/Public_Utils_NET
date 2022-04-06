using System.Collections.Generic;
using Telluria.Utils.Crud.Commands.BaseCommands;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Handlers
{
  public interface IBaseCrudCommandHandler<TEntity> :
    ICommandHandler<BaseFindCommand<TEntity>, TEntity>,
    IListCommandHandler<BaseListCommand<TEntity>, TEntity>,
    IListCommandHandler<BaseListAllCommand<TEntity>, TEntity>,
    ICommandHandler<BaseGetCommand<TEntity>, TEntity>,
    ICommandHandler<BaseCreateCommand<TEntity>, TEntity>,
    ICommandHandler<BaseCreateManyCommand<TEntity>, IEnumerable<TEntity>>,
    ICommandHandler<BaseUpdateCommand<TEntity>, TEntity>,
    ICommandHandler<BaseSoftDeleteCommand<TEntity>>,
    ICommandHandler<BaseRemoveCommand<TEntity>>
    where TEntity : BaseEntity
  {
  }
}
