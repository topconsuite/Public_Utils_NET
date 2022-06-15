using System;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseSoftDeleteCommand : ICommand
  {
    public Guid Id { get; set; }

    public BaseSoftDeleteCommand(Guid id)
    {
      Id = id;
    }
  }
}
