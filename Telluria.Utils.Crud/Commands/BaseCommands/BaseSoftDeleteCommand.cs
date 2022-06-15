using System;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
  public class BaseSoftDeleteCommand<TEntity> : ICommand
    where TEntity : BaseEntity
  {
    public Guid Id { get; set; }

    public BaseSoftDeleteCommand(Guid id)
    {
      Id = id;
    }
  }
}
