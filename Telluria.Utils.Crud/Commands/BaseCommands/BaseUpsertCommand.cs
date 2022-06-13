using System;
using System.Linq.Expressions;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseUpsertCommand<TEntity> : ICommand
    where TEntity : BaseEntity
  {
    public TEntity Data { get; set; }
    public Expression<Func<TEntity, object>> Match { get; set; }
    public Expression<Func<TEntity, TEntity, TEntity>> Updater { get; set; }
    public string[] Includes { get; set; }

    public BaseUpsertCommand(
      TEntity data, Expression<Func<TEntity, object>> match,
      Expression<Func<TEntity, TEntity, TEntity>> updater,
      params string[] includes)
    {
      Data = data;
      Match = match;
      Updater = updater;
      Includes = includes;
    }
  }
}
