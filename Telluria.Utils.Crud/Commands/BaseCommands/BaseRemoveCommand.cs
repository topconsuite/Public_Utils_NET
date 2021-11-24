using System;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
    public class BaseRemoveCommand<TEntity> : ICommand where TEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public BaseRemoveCommand(Guid id)
        {
            Id = id;
        }
    }
}
