using System;
using System.Linq.Expressions;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseListCommand<TEntity> : IListCommand
    where TEntity : BaseEntity
  {
    public Expression<Func<TEntity, bool>> Where { get; set; }
    public string[] Includes { get; set; }

    public uint Page { get; set; }

    public uint PerPage { get; set; }

    public BaseListCommand(uint page, uint perPage, Expression<Func<TEntity, bool>> where, params string[] includes)
    {
      Page = page;
      PerPage = perPage;
      Where = where;
      Includes = includes;
    }
  }
}
