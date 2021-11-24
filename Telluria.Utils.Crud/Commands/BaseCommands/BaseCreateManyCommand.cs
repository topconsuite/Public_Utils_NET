using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
    public class BaseCreateManyCommand<TEntity> : ICommand where TEntity : BaseEntity
    {
        public TEntity[] Data { get; set; }

        public BaseCreateManyCommand(TEntity[] data)
        {
            Data = data;
        }
    }
}
