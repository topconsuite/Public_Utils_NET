using System;
using System.Threading;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseSoftDeleteCommand : ICommand
  {
    public CancellationToken CancellationToken { get; set; }
    public Guid Id { get; set; }

    public BaseSoftDeleteCommand(Guid id, CancellationToken cancellationToken)
    {
      Id = id;
      CancellationToken = cancellationToken;
    }
  }
}
