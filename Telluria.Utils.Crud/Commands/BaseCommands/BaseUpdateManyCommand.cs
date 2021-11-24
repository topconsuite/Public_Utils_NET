using System;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
    public class BaseUpdateManyCommand<TEntity> : ICommand where TEntity : BaseEntity
    {
        public TEntity[] Data { get; set; }

        public BaseUpdateManyCommand(TEntity[] data)
        {
            Data = data;
        }
    }
}
