using System;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseGetCommand : ICommand
  {
    public Guid Id { get; set; }
    public string[] Includes { get; set; }

    public BaseGetCommand(Guid id, params string[] includes)
    {
      Id = id;
      Includes = includes;
    }
  }
}
