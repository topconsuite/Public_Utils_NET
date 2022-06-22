using System.Threading;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseUpdateCommand<TEntity> : ICommand
    where TEntity : BaseEntity
  {
    public CancellationToken CancellationToken { get; set; }
    public TEntity Data { get; set; }
    public string[] Includes { get; set; }

    public BaseUpdateCommand(TEntity data, string[] includes, CancellationToken cancellationToken)
    {
      Data = data;
      Includes = includes;
      CancellationToken = cancellationToken;
    }
  }
}
