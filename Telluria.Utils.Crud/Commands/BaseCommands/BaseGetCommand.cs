using System;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
    public class BaseGetCommand<TEntity> : ICommand where TEntity : BaseEntity
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
