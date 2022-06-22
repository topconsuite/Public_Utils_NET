using System;
using System.Threading;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseRemoveCommand : ICommand
  {
    public CancellationToken CancellationToken { get; set; }
    public Guid Id { get; set; }

    public BaseRemoveCommand(Guid id, CancellationToken cancellationToken)
    {
      Id = id;
      CancellationToken = cancellationToken;
    }
  }
}
