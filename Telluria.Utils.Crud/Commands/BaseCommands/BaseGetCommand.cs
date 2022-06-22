using System;
using System.Threading;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseGetCommand : ICommand
  {
    public CancellationToken CancellationToken { get; set; }
    public Guid Id { get; set; }
    public string[] Includes { get; set; }

    public BaseGetCommand(Guid id, string[] includes, CancellationToken cancellationToken)
    {
      Id = id;
      Includes = includes;
      CancellationToken = cancellationToken;
    }
  }
}
