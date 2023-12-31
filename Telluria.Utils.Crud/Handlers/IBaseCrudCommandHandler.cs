using System.Collections.Generic;
using Telluria.Utils.Crud.Commands.BaseCommands;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Repositories;
using Telluria.Utils.Crud.Validation;

namespace Telluria.Utils.Crud.Handlers;

public interface IBaseCrudCommandHandler<TEntity, TValidator, TRepository> :
  IListCommandHandler<BaseListCommand<TEntity>, TEntity>,
  IListCommandHandler<BaseListSortedCommand<TEntity>, TEntity>,
  IListCommandHandler<BaseListAllCommand<TEntity>, TEntity>,
  IListCommandHandler<BaseListAllSortedCommand<TEntity>, TEntity>,
  ICommandHandler<BaseGetCommand, TEntity>,
  ICommandHandler<BaseFindCommand<TEntity>, TEntity>,
  ICommandHandler<BaseCreateCommand<TEntity>, TEntity>,
  ICommandHandler<BaseCreateManyCommand<TEntity>, IEnumerable<TEntity>>,
  ICommandHandler<BaseUpdateCommand<TEntity>, TEntity>,
  ICommandHandler<BaseSoftDeleteCommand>,
  ICommandHandler<BaseRemoveCommand>
  where TEntity : BaseEntity
  where TValidator : BaseEntityValidator<TEntity>, new()
  where TRepository : IBaseCrudRepository<TEntity>
{
}
