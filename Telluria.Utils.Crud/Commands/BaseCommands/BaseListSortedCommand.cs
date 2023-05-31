using System;
using System.Linq.Expressions;
using System.Threading;
using elluria.Utils.Crud.GraphQL.InputTypes;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands;

public class BaseListSortedCommand<TEntity> : IListCommand
  where TEntity : BaseEntity
{
  public BaseListSortedCommand(
    uint page,
    uint perPage,
    Expression<Func<TEntity, bool>> where,
    SortClauses sort,
    string[] includes,
    CancellationToken cancellationToken)
  {
    Page = page;
    PerPage = perPage;
    Where = where;
    Sort = sort;
    Includes = includes;
    CancellationToken = cancellationToken;
  }

  public Expression<Func<TEntity, bool>> Where { get; set; }
  public string[] Includes { get; set; }
  public SortClauses Sort { get; set; }
  public CancellationToken CancellationToken { get; set; }
  public uint Page { get; set; }
  public uint PerPage { get; set; }
}
