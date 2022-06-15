using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseCreateCommand<TEntity> : ICommand
    where TEntity : BaseEntity
  {
    public TEntity Data { get; set; }
    public string[] Includes { get; set; }

    public BaseCreateCommand(TEntity data, params string[] includes)
    {
      Data = data;
      Includes = includes;
    }
  }
}
