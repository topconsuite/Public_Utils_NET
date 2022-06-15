using System;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseRemoveCommand : ICommand
  {
    public Guid Id { get; set; }

    public BaseRemoveCommand(Guid id)
    {
      Id = id;
    }
  }
}
