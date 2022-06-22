using System;
using System.Linq.Expressions;
using System.Threading;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseFindCommand<TEntity> : ICommand
    where TEntity : BaseEntity
  {
    public CancellationToken CancellationToken { get; set; }
    public Expression<Func<TEntity, bool>> Where { get; set; }
    public string[] Includes { get; set; }

    public BaseFindCommand(Expression<Func<TEntity, bool>> where, string[] includes, CancellationToken cancellationToken)
    {
      Where = where;
      Includes = includes;
      CancellationToken = cancellationToken;
    }
  }
}
