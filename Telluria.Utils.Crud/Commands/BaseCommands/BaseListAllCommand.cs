using System;
using System.Linq.Expressions;
using System.Threading;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands;

public class BaseListAllCommand<TEntity> : IListCommand
  where TEntity : BaseEntity
{
  public BaseListAllCommand(
    uint page,
    uint perPage,
    Expression<Func<TEntity, bool>> where,
    string[] includes,
    CancellationToken cancellationToken)
  {
    Page = page;
    PerPage = perPage;
    Where = where;
    Includes = includes;
    CancellationToken = cancellationToken;
  }

  public Expression<Func<TEntity, bool>> Where { get; set; }
  public string[] Includes { get; set; }
  public CancellationToken CancellationToken { get; set; }
  public uint Page { get; set; }
  public uint PerPage { get; set; }
}
