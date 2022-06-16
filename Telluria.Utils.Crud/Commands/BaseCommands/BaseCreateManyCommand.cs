using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseCreateManyCommand<TEntity> : ICommand
    where TEntity : BaseEntity
  {
    public TEntity[] Data { get; set; }
    public string[] Includes { get; set; }

    public BaseCreateManyCommand(TEntity[] data, params string[] includes)
    {
      Data = data;
      Includes = includes;
    }
  }
}
